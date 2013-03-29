using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace MessageDeal1
{
    public class MyDeal : IMessageDeal
    {

        private Dictionary<string, string> _learning;

        private string _filePath;

        private bool _enabled = false;

        public string IName
        {
            get { return "示例1"; }
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"状态","status"},
            {"关于","about"},
        };

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"@学 问题 答案","学说话[问题不能有空格]"},
            {"@忘 问题","忘记学过的话"},
            {"@查","查看我学会的话"},
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
            _learning = new Dictionary<string, string>();
            var assemblay = this.GetType().Assembly;
            _filePath = assemblay.Location;
            _filePath = _filePath.Substring(0, _filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filePath = _filePath + this.GetType().FullName + ".db";
            LoadFromFile();
        }

        public void LoadFromFile()
        {
            _learning.Clear();
            try
            {
                string[] lines = File.ReadAllLines(_filePath);
                foreach (string line in lines)
                {
                    string[] items = line.Split(new char[] { ' ' }, 2, StringSplitOptions.None);
                    if (items.Length == 2 && !_learning.ContainsKey(items[0]))
                    {
                        _learning.Add(items[0], items[1]);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void SaveToFile()
        {
            var lines = _learning.Select(ele => ele.Key + ' ' + ele.Value);
            File.WriteAllLines(_filePath, lines);
        }

        public string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            return DealWDString(message);
        }

        private string DealWDString(string message)
        {
            if (_enabled)
            {
                if (string.IsNullOrEmpty(message))
                {
                    return null;
                }
                message = message.Trim();
                Regex r = new Regex(@"^@学\s*(\S*)\s*(.*)$");
                Match m = r.Match(message);
                if (m.Success)
                {
                    if (m.Groups[1].Value.Length < 2)
                    {
                        return "呵呵，问题请至少输入两个字哦。";
                    }
                    if (_learning.ContainsKey(m.Groups[1].Value))
                        _learning[m.Groups[1].Value] = m.Groups[2].Value;
                    else
                        _learning.Add(m.Groups[1].Value, m.Groups[2].Value);
                    SaveToFile();
                    return "已经学会此问题：" + m.Groups[1];
                }
                r = new Regex(@"^@忘\s*(\S*)$"); 
                m = r.Match(message);
                if (m.Success)
                {
                    if (m.Groups[1].Value.Length < 2)
                    {
                        return "呵呵，问题请至少输入两个字哦。";
                    }
                    if (_learning.ContainsKey(m.Groups[1].Value))
                        _learning.Remove(m.Groups[1].Value);
                    SaveToFile();
                    return "已经忘记此问题：" + m.Groups[1];
                }
                if (message == "@查")
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, string> filter in _learning)
                    {
                        sb.AppendFormat("{0}  ->  {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                    }
                    return "列表：" +Environment.NewLine + sb.ToString();
                }
                foreach (string key in _learning.Keys)
                {
                    if (message.Contains(key))
                    {
                        return _learning[key];
                    }
                }
            }
            return null;
        }

        public string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            return DealWDString(message);
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "status")
            {
                _enabled = !_enabled;
                MessageBox.Show("现在的状态是" + (_enabled ? "启用" : "停用"), "状态指示");
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这只是一个示例", "关于");
            }
        }

        public string StatusChanged(Dictionary<string, object> info, string newStatus)
        {
            if (_enabled)
            {
                QQStatus status = QQStatus.GetQQStatusByInternal(newStatus);
                if (status != null && status != QQStatus.StatusOffline)
                {
                    return string.Format("{0} 你好，你现在的状态是:{1}", TranslateMessageUser.UserNick, status.Status);
                }
            }
            return null;
        }

        public string Input(Dictionary<string, object> info)
        {
            if (_enabled)
            {
                return string.Format("{0} 你好，我正在等待你的输入。", TranslateMessageUser.UserNick);
            }
            return null;
        }
    }
}
