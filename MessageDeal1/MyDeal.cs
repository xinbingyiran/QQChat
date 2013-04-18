using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

namespace MessageDeal1
{

    internal struct FindIndex
    {
        public string find;
        public int index;
    };
    public class MyDeal : IMessageDeal
    {

        private List<KeyValuePair<string, string>> _learning;


        private bool _autoreplay;

        public static readonly string lineSep = @"<br/>";

        private readonly Dictionary<string, FindIndex> _currentIndex;

        private readonly string _filePath;
        private bool _saveFlag;
        private readonly object _saveLock;
        private System.Timers.Timer _timer;

        public string Setting
        {
            get
            {
                return (Enabled ? "1" : "0") + (_autoreplay ? "1" : "0");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 2)
                {
                    Enabled = value[0] == '1';
                    _autoreplay = value[1] == '1';
                }
            }
        }
        public string IName
        {
            get { return "学话鹦鹉"; }
        }

        public bool Enabled
        {
            get;
            set;
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"重载","reload"},
            {"启用","start"},
            {"停用","stop"},
            {"状态","status"},
            {"关于","about"},
        };

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"-学 问题 答案","学说话[问题不能有空格]"},
            {"-忘 问题","忘记学过的话"},
            {"-列[ 开始位置]","列出学会的问题,从开始位置开始的相临五条内容"},
            {"-查 问题关键字","查看学会的问题,从问题查答案"},
            {"-反 回答关键字","查看学会的问题,从答案查问题"},
            {"-问/答 问题","问问题"},
            {"-状[ 查/启/停]","自动回答问题状态查询/启用/停用"},
        };

        public Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        public Dictionary<string, string> Filters
        {
            get
            {
                return _filters;
            }
        }

        public MyDeal()
        {
            _saveLock = new object();
            _saveFlag = false;
            _learning = new List<KeyValuePair<string, string>>();
            _currentIndex = new Dictionary<string, FindIndex>();
            _autoreplay = true;
            var assemblay = this.GetType().Assembly;
            _filePath = assemblay.Location;
            _filePath = _filePath.Substring(0, _filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filePath = _filePath + this.GetType().FullName + ".db";
            new Task(() =>
                {
                    LoadFromFile();
                    _timer = new System.Timers.Timer();
                    _timer.AutoReset = true;
                    _timer.Interval = 5000;
                    _timer.Elapsed += _timer_Elapsed;
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
                string[] lines = File.ReadAllLines(_filePath);
                Dictionary<string, string> dir = new Dictionary<string, string>();
                foreach (string line in lines)
                {
                    string[] items = line.Split(new char[] { ' ' }, 2, StringSplitOptions.None);
                    if (items.Length == 2 && items[0].Length >= 2)
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
                    var lines = _learning.Select(ele => ele.Key + ' ' + ele.Value);
                    File.WriteAllLines(_filePath, lines);
                    _saveFlag = false;
                }
            }
        }

        public string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            return DealWDString(message);
        }

        private string DealWDString(string message)
        {
            if (!Enabled)
            {
                return null;
            }
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }
            if (message.Length < 2 || message[0] != '-')
            {
                if (_autoreplay)
                {
                    return GetReturnMessage(message);
                }
            }
            message = message.Trim().Replace(Environment.NewLine, lineSep);
            var tag = message[1];
            var submessage = message.Substring(2).Trim();
            switch (tag)
            {
                case '学':
                    {
                        var wd = submessage.Split(new char[] { ' ' }, StringSplitOptions.None);
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
                case '问':
                case '答':
                    if (String.IsNullOrEmpty(submessage))
                        return "呵呵，想问查什么呢。";
                    if (submessage.Length < 2)
                    {
                        return "呵呵，问题请至少输入两个字哦。";
                    }
                    return GetReturnMessage(submessage) ?? "这个问题的答案还没人教过我哎。";
                case '状':
                    {
                        if (submessage.Contains('查'))
                        {
                            return "当前自动回答问题状态为" + (_autoreplay ? "启用" : "停用");
                        }
                        else if (submessage.Contains('启'))
                        {
                            _autoreplay = true;
                            return "当前自动回答问题状态为启用";
                        }
                        else if (submessage.Contains('停'))
                        {
                            _autoreplay = false;
                            return "当前自动回答问题状态为停用";
                        }
                    }
                    break;
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

        public string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            return DealWDString(message);
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "reload")
            {
                LoadFromFile();
                MessageBox.Show("已完成重载，共" + _learning.Count + "条数据", "操作提示");
            }
            else if (menuName == "start")
            {
                _autoreplay = true;
                MessageBox.Show("当前自动回答问题状态为启用", "状态指示");
            }
            else if (menuName == "stop")
            {
                _autoreplay = false;
                MessageBox.Show("当前自动回答问题状态为停用", "状态指示");
            }
            else if (menuName == "status")
            {
                MessageBox.Show("当前自动回答问题状态为" + (_autoreplay ? "启用" : "停用"), "状态指示");
            }
            else if (menuName == "about")
            {
                MessageBox.Show("学话鹦鹉。\r\n能学会一些对话信息。\r\n当前信息条数：" + _learning.Count, "关于插件");
            }
        }

        public string StatusChanged(Dictionary<string, object> info, string newStatus)
        {
            //if (Enabled)
            //{
            //    QQStatus status = QQStatus.GetQQStatusByInternal(newStatus);
            //    if (status != null && status != QQStatus.StatusOffline)
            //    {
            //        return string.Format("{0} 你好，你现在的状态是:{1}", TranslateMessageUser.UserNick, status.Status);
            //    }
            //}
            return null;
        }

        public string Input(Dictionary<string, object> info)
        {
            //if (Enabled)
            //{
            //    return string.Format("{0} 你好，我正在等待你的输入。", TranslateMessageUser.UserNick);
            //}
            return null;
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
