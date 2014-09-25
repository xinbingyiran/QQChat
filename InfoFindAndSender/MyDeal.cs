using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InfoFinAndSender
{

    public class MessageItem
    {
        public Int64 Time { get; set; }
        public String Message { get; set; }

        public override int GetHashCode()
        {
            return Time.GetHashCode() ^ Message.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as MessageItem;
            if (o != null)
            {
                return this.Time.Equals(o.Time) && this.Message.Equals(o.Message);
            }
            return false;
        }
    }
    public class MyDeal : TMessage
    {

        private List<MessageItem> _info;


        private bool _enablefind;
        private bool _autoreplay;
        private bool _enablestudy;
        private long _saveTime = 60;
        private long _saveTimeElapse = 0;

        public static readonly string lineSep = @"<br/>";

        private int _currentIndex;

        private readonly string _filePath;
        private bool _saveFlag;
        private readonly object _saveLock;
        private System.Timers.Timer _timer;

        public override string Setting
        {
            get
            {
                return (Enabled ? "1" : "0")
                    + (_enablefind ? "1" : "0")
                    + (_autoreplay ? "1" : "0")
                    + (_enablestudy ? "1" : "0");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 4)
                {
                    Enabled = value[0] == '1';
                    _enablefind = value[1] == '1';
                    _autoreplay = value[2] == '1';
                    _enablestudy = value[3] == '1';
                }
            }
        }
        public override string PluginName
        {
            get { return "信息服务"; }
        }

        public override Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"状态","status"},
        };

        public override Dictionary<string, string> Filters
        {
            get
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                if (_enablestudy)
                {
                    d.Add("-+/- 信息", "添加/删除信息【删除时匹配头部】");
                }
                if (_enablefind)
                {
                    d.Add("-c/C 部分信息", "列出三条查到的信息");
                    d.Add("-t/T 部分信息", "列出三条查到的信息【带发布时间】");
                }
                return d;
            }
        }

        public MyDeal()
        {
            _saveLock = new object();
            _saveFlag = false;
            _info = new List<MessageItem>();
            _currentIndex = 0;
            _autoreplay = true;
            _enablefind = true;
            var assemblay = this.GetType().Assembly;
            _filePath = assemblay.Location;
            _filePath = _filePath.Substring(0, _filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filePath = _filePath + this.GetType().FullName + ".db";
            new Task(() =>
                {
                    LoadFromFile();
                    _timer = new System.Timers.Timer();
                    _timer.AutoReset = true;
                    _timer.Interval = 1000;
                    _timer.Elapsed += _timer_Elapsed;
                    System.Threading.Thread.Sleep(new Random().Next((Int32)_timer.Interval));
                    _timer.Start();
                }).Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_saveTime > 0)
            {
                _saveTimeElapse++;
                if (_saveTimeElapse >= _saveTime)
                {
                    _saveTimeElapse = 0;
                    SaveToFile();
                }
            }
        }

        public void LoadFromFile()
        {
            _info.Clear();
            try
            {
                if (File.Exists(_filePath))
                {
                    string[] lines = File.ReadAllLines(_filePath);
                    HashSet<MessageItem> dir = new HashSet<MessageItem>();
                    foreach (string line in lines)
                    {
                        try
                        {
                            var item = JsonConvert.DeserializeObject<MessageItem>(line);
                            if (dir.Contains(item))
                            {
                                continue;
                            }
                            else
                            {
                                dir.Add(item);
                            }
                        }
                        catch (Exception) { }
                    }
                    _info = dir.ToList();
                    if (_info.Count != lines.Length)
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
                    File.WriteAllLines(_filePath, _info.Select(e => JsonConvert.SerializeObject(e)));
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
            message = message.Trim();
            var rm = GetReturnMessage(message);
            if (rm != null)
            {
                return rm;
            }
            if (message.Length < 2 || message[0] != '-')
            {
                return null;
            }
            message = message.Replace(Environment.NewLine, lineSep);
            var tag = message[1];
            var submessage = message.Substring(2).Trim();
            switch (tag)
            {
                case '+':
                    {
                        if (!_enablestudy)
                        {
                            return null;
                        }
                        if (submessage.Length < 2)
                        {
                            return "呵呵，信息长度太短了。";
                        }
                        if (_info.FirstOrDefault(e => e.Message == submessage) != null)
                        {
                            return null;
                        }
                        _info.Add(new MessageItem { Time = DateTime.Now.Ticks / TimeSpan.TicksPerMinute, Message = submessage });
                        SetSaveFlag();
                        return "好的，我已经记下了。";
                    }
                case '-':
                    {
                        if (!_enablestudy)
                        {
                            return null;
                        }
                        if (submessage.Length < 2)
                        {
                            return "呵呵，信息长度太短了。";
                        }
                        var a = _info.FindAll(e => e.Message.IndexOf(submessage) >= 0);
                        if (a == null || a.Count == 0)
                        {
                            return "没找到此信息。";
                        }
                        if (a.Count > 1)
                        {
                            return "多条匹配，请提供精确信息。";
                        }
                        _info.Remove(a[0]);
                        SetSaveFlag();
                        return "好的，我已经删除了。";
                    }
                case 't':
                case 'T':
                    {
                        if (!_enablefind)
                            return null;
                        if (submessage.Length < 2)
                        {
                            return "呵呵，信息长度太短了。";
                        }
                        int count = 3;
                        var list = new Dictionary<string, string>();
                        if (_currentIndex >= _info.Count)
                        {
                            _currentIndex = 0;
                        }
                        var sb = new StringBuilder("查找结果：" + Environment.NewLine);
                        for (; _currentIndex < _info.Count && count > 0; _currentIndex++)
                        {
                            var filter = _info[_currentIndex];
                            if (filter.Message.IndexOf(submessage) >= 0)
                            {
                                count--;
                                sb.AppendFormat("{1}【{0:yyyy-MM-dd HH:mm}】", new DateTime(filter.Time * TimeSpan.TicksPerMinute), filter.Message);
                                sb.AppendLine();
                                sb.AppendLine();
                            }
                        }
                        return sb.ToString().Replace(lineSep, Environment.NewLine).Trim();
                    }
                case 'c':
                case 'C':
                    {
                        if (!_enablefind)
                            return null;
                        if (submessage.Length < 2)
                        {
                            return "呵呵，信息长度太短了。";
                        }
                        int count = 3;
                        var list = new Dictionary<string, string>();
                        if (_currentIndex >= _info.Count)
                        {
                            _currentIndex = 0;
                        }
                        var sb = new StringBuilder("查找结果：" + Environment.NewLine);
                        for (; _currentIndex < _info.Count && count > 0; _currentIndex++)
                        {
                            var filter = _info[_currentIndex];
                            if (filter.Message.IndexOf(submessage) >= 0)
                            {
                                count--;
                                sb.AppendLine(filter.Message);
                                sb.AppendLine();
                            }
                        }
                        return sb.ToString().Replace(lineSep, Environment.NewLine).Trim();
                    }
                default:
                    break;
            }
            return null;
        }
        private string GetReturnMessage(string message)
        {
            if (!_autoreplay)
                return null;
            if (message.Length < 2)
            {
                return null;
            }
            int count = 1;
            if (_currentIndex >= _info.Count)
            {
                _currentIndex = 0;
            }
            var find = false;
            var sb = new StringBuilder();
            for (; _currentIndex < _info.Count && count > 0; _currentIndex++)
            {
                var filter = _info[_currentIndex];
                if (filter.Message.IndexOf(message) >= 0)
                {
                    count--;
                    sb.AppendLine(filter.Message);
                    find = true;
                }
            }
            if (find)
            {
                return sb.ToString().Replace(lineSep, Environment.NewLine).Trim();
            }
            return null;
        }

        public override void MenuClicked(string menuName)
        {
            LastMessage = null;
            if (menuName == "status")
            {
                LastMessage = "查找功能" + (_enablefind ? "启用" : "停用") + " 自动回复" + (_autoreplay ? "启用" : "停用") + " 学习功能" + (_enablestudy ? "启用" : "停用");
            }
            else if (menuName == "reload")
            {
                LoadFromFile();
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
                return "信息服务。\r\n信息查询服务，可定制自动发送。\r\n当前信息条数：" + _info.Count;
            }
        }

    }

}
