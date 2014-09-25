using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Learning
{

    internal struct FindIndex
    {
        public string find;
        public int index;
    };
    public class MyDeal : TMessage
    {

        private List<KeyValuePair<string, string>> _learning;


        private bool _autoreplay;
        private bool _enablestudy;
        private bool _enablefind;

        public static readonly string lineSep = @"<br/>";

        private readonly Dictionary<string, FindIndex> _currentIndex;

        private readonly string _filePath;
        private bool _saveFlag;
        private readonly object _saveLock;
        private System.Timers.Timer _timer;

        public override string Setting
        {
            get
            {
                return (Enabled ? "1" : "0")
                    + (_autoreplay ? "1" : "0")
                    + (_enablestudy ? "1" : "0")
                    + (_enablefind ? "1" : "0");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 4)
                {
                    Enabled = value[0] == '1';
                    _autoreplay = value[1] == '1';
                    _enablestudy = value[2] == '1';
                    _enablefind = value[3] == '1';
                }
            }
        }
        public override string PluginName
        {
            get { return "学话鹦鹉"; }
        }

        public override Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"学习功能启","start"},
            {"学习功能停","stop"},
            {"查找功能启","enable"},
            {"查找功能停","disable"},
            {"状态","status"},
        };

        public override Dictionary<string, string> Filters
        {
            get
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                if (_enablestudy)
                {
                    d.Add("-学/忘 问题[ 答案]", "学/忘问题[问题不能有空格]");
                }
                if (_enablefind)
                {
                    d.Add("-列[ 开始位置]", "列出五条学会的问题");
                    d.Add("-查/反 问题关键字", "查看学会的问题/答案");
                }
                return d;
            }
        }

        public MyDeal()
        {
            _saveLock = new object();
            _saveFlag = false;
            _learning = new List<KeyValuePair<string, string>>();
            _currentIndex = new Dictionary<string, FindIndex>();
            _autoreplay = true;
            _enablefind = true;
            _enablestudy = true;
            var assemblay = this.GetType().Assembly;
            _filePath = assemblay.Location;
            _filePath = _filePath.Substring(0, _filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filePath = _filePath + this.GetType().FullName + ".db";
            new Task(() =>
                {
                    LoadFromFile();
                    _timer = new System.Timers.Timer();
                    _timer.AutoReset = true;
                    _timer.Interval = 300000;
                    _timer.Elapsed += _timer_Elapsed;
                    System.Threading.Thread.Sleep(new Random().Next((Int32)_timer.Interval));
                    _timer.Start();
                }).Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveToFile();
        }

        public void LoadFromFile()
        {
            _learning.Clear();
            try
            {
                if (File.Exists(_filePath))
                {
                    string[] lines = File.ReadAllLines(_filePath);
                    Dictionary<string, string> dir = new Dictionary<string, string>();
                    foreach (string line in lines)
                    {
                        var items = JsonConvert.DeserializeObject<string[]>(line);
                        if (items != null && items.Length == 2 && items[0].Length > 1)
                        {
                            if (dir.ContainsKey(items[0]))
                            {
                                dir[items[0]] = dir[items[0]] + lineSep + items[1];
                            }
                            else
                            {
                                dir.Add(items[0], items[1]);
                            }
                        }
                    }
                    _learning = dir.ToList();
                    if (_learning.Count != lines.Length)
                    {
                        SetSaveFlag();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetSaveFlag()
        {
            _saveFlag = true;
        }

        public void SaveToFile()
        {
            lock (_saveLock)
            {
                if (_saveFlag)
                {
                    var lines = _learning.Select(ele => JsonConvert.SerializeObject(new string[] { ele.Key, ele.Value }));
                    File.WriteAllLines(_filePath, lines);
                    _saveFlag = false;
                }
            }
        }

        public override string DealMessage(string messageType, Dictionary<string, object> info, string message)
        {
            if (!Enabled)
            {
                return null;
            }
            if (messageType != MessageType.MessageFriend && messageType != MessageType.MessageGroup)
            {
                return null;
            }
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }
            if (_autoreplay)
            {
                var rm = GetReturnMessage(message);
                if (rm != null)
                {
                    return rm;
                }
            }
            if (message.Length < 2 || message[0] != '-')
            {
                return null;
            }
            message = message.Trim().Replace(Environment.NewLine, lineSep);
            var tag = message[1];
            var submessage = message.Substring(2).Trim();
            switch (tag)
            {
                case '学':
                    {
                        if (!_enablestudy)
                            return null;
                        var wd = submessage.Split(new char[] { ' ' }, 2, StringSplitOptions.None);
                        if (wd.Length != 2)
                        {
                            return null;
                        }
                        if (wd[0].Length < 2)
                        {
                            return null;
                        }
                        _learning.ReplacePair(wd[0], wd[1]);
                        SetSaveFlag();
                        return "已经学会此问题：" + wd[0];
                    }
                case '忘':
                    {
                        if (!_enablestudy)
                            return null;
                        if (submessage.Length < 2)
                        {
                            return "呵呵，问题请至少输入两个字哦。";
                        }
                        _learning.RemovePair(submessage);
                        SetSaveFlag();
                        return "已经忘记此问题：" + submessage;
                    }
                case '列':
                    {
                        if (!_enablefind)
                            return null;
                        int count = 5;
                        int index = 0;
                        try
                        {
                            index = Convert.ToInt32(submessage);
                        }
                        catch (Exception)
                        {
                            index = _currentIndex.FindIndex("列", null);
                        }
                        StringBuilder sb = new StringBuilder();
                        if (index >= _learning.Count)
                        {
                            index = 0;
                        }
                        sb.AppendLine("问题列表：@" + index);
                        for (; index < _learning.Count && count > 0; index++, count--)
                        {
                            var filter = _learning[index];
                            sb.AppendFormat("{0}  -->  {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                        }
                        _currentIndex.SetIndex("列", null, index);
                        return sb.ToString().Replace(lineSep, Environment.NewLine);
                    }
                case '查':
                    {
                        if (!_enablefind)
                            return null;
                        if (String.IsNullOrEmpty(submessage))
                            return "呵呵，想查什么呢。";
                        int count = 5;
                        var list = new Dictionary<string, string>();
                        int index = _currentIndex.FindIndex("查", submessage);
                        if (index >= _learning.Count)
                        {
                            index = 0;
                        }
                        var sb = new StringBuilder("查找结果：" + Environment.NewLine);
                        for (; index < _learning.Count && count > 0; index++)
                        {
                            var filter = _learning[index];
                            if (filter.Key.Contains(submessage))
                            {
                                count--;
                                sb.AppendFormat("{0}  -->  {1} @{2}{3}", filter.Key, filter.Value, index, Environment.NewLine);
                            }
                        }
                        _currentIndex.SetIndex("查", submessage, index);
                        return sb.ToString().Replace(lineSep, Environment.NewLine);
                    }
                case '反':
                    {
                        if (!_enablefind)
                            return null;
                        if (String.IsNullOrEmpty(submessage))
                            return "呵呵，想查什么呢。";
                        int count = 5;
                        var list = new Dictionary<string, string>();
                        int index = _currentIndex.FindIndex("反", submessage);
                        if (index >= _learning.Count)
                        {
                            index = 0;
                        }
                        var sb = new StringBuilder("反向查找结果：" + Environment.NewLine);
                        for (; index < _learning.Count && count > 0; index++)
                        {
                            var filter = _learning[index];
                            if (filter.Value.Contains(submessage))
                            {
                                count--;
                                sb.AppendFormat("{0}  -->  {1} @{2}{3}", filter.Key, filter.Value, index, Environment.NewLine);
                            }
                        }
                        _currentIndex.SetIndex("反", submessage, index);
                        return sb.ToString().Replace(lineSep, Environment.NewLine);
                    }
                default:
                    break;
            }
            return null;
        }

        private string GetReturnMessage(string message)
        {
            string retstr = null;
            int sub = int.MaxValue;
            for (int i = 0; i < _learning.Count; i++)
            {
                var itemindex = message.IndexOf(_learning[i].Key, System.StringComparison.Ordinal);
                if (itemindex >= 0 && itemindex <= 5)
                {
                    int si = message.Length - _learning[i].Key.Length;
                    if (si < sub)
                    {
                        retstr = _learning[i].Value.Replace(lineSep, Environment.NewLine);
                        sub = si;
                    }
                }
            }
            return retstr;
        }

        public override void MenuClicked(string menuName)
        {
            LastMessage = null;
            if (menuName == "start")
            {
                _enablestudy = true;
                LastMessage = "当前学习功能为启用";
            }
            else if (menuName == "stop")
            {
                _enablestudy = false;
                LastMessage = "当前学习功能为停用";
            }
            else if (menuName == "enable")
            {
                _enablefind = true;
                LastMessage = "当前查找功能为启用";
            }
            else if (menuName == "disable")
            {
                _enablefind = false;
                LastMessage = "当前查找功能为停用";
            }
            else if (menuName == "status")
            {
                LastMessage = "当前学习功能" + (_enablestudy ? "启用" : "停用") + " 查找功能" + (_enablefind ? "启用" : "停用");
            }
            if (LastMessage != null && OnMessage != null)
            {
                OnMessage(this, EventArgs.Empty);
            }
        }

        public override event EventHandler<EventArgs> OnMessage;

        public override void OnExited()
        {
            SaveToFile();
        }

        public override string AboutMessage
        {
            get
            {
                return "学话鹦鹉。\r\n能学会一些对话信息。\r\n当前信息条数：" + _learning.Count;
            }
        }

    }

    internal static class Extends
    {
        internal static void AppendPair(this List<KeyValuePair<string, string>> list, string key, string value)
        {
            int i = 0;
            for (; i < list.Count; i++)
            {
                if (list[i].Key == key)
                {
                    list[i] = new KeyValuePair<string, string>(key, list[i].Value + MyDeal.lineSep + value);
                    break;
                }
            }
            if (i == list.Count)
            {
                list.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        internal static void ReplacePair(this List<KeyValuePair<string, string>> list, string key, string value)
        {
            int i = 0;
            for (; i < list.Count; i++)
            {
                if (list[i].Key == key)
                {
                    list[i] = new KeyValuePair<string, string>(key, value);
                    break;
                }
            }
            if (i == list.Count)
            {
                list.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        internal static void RemovePair(this List<KeyValuePair<string, string>> list, string key)
        {
            int i = 0;
            for (; i < list.Count; i++)
            {
                if (list[i].Key == key)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        internal static int FindIndex(this Dictionary<string, FindIndex> dir, string key, string find)
        {
            if (dir.ContainsKey(key) && dir[key].find == find)
            {
                return dir[key].index;
            }
            else
            {
                return 0;
            }

        }

        internal static void SetIndex(this Dictionary<string, FindIndex> dir, string key, string find, int index)
        {
            if (dir.ContainsKey(key))
            {
                dir[key] = new FindIndex { find = find, index = index };
            }
            else
            {
                dir.Add(key, new FindIndex { find = find, index = index });
            }
        }
    }
}
