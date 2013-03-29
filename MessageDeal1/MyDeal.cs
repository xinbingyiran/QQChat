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

        private int _currentIndex;

        private string _filePath;

        public string IName
        {
            get { return "示例1"; }
        }

        public bool Enabled
        {
            get;
            set;
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"重载","reload"},
            {"关于","about"},
        };

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"@学 问题 答案","学说话[问题不能有空格]"},
            {"@忘 问题","忘记学过的话"},
            {"@查 from","查看我学会的话,从第几条开始的十条内容"},
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
                    if (items.Length == 2)
                    {
                        if (_learning.ContainsKey(items[0]))
                        {
                            _learning[items[0]] += " | " + items[1];
                        }
                        else
                        {
                            _learning.Add(items[0], items[1]);
                        }
                    }
                }
                if (_learning.Count != lines.Length)
                {
                    SaveToFile();
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
            if (!Enabled)
            {
                return null;
            }
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
                return "已经学会此问题：" + m.Groups[1].Value;
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
                return "已经忘记此问题：" + m.Groups[1].Value;
            }
            r = new Regex(@"^@查\s*(\d*)$");
            m = r.Match(message);
            if (m.Success)
            {
                int count = 10;
                try
                {
                    _currentIndex = Convert.ToInt32(m.Groups[1].Value);
                }
                catch (Exception) {
                }
                StringBuilder sb = new StringBuilder();
                if(_currentIndex >= _learning.Count)
                {
                    _currentIndex = 0;
                }
                sb.AppendLine("列表：" + _currentIndex);
                foreach (KeyValuePair<string, string> filter in _learning.Skip(_currentIndex).Take(count))
                {
                    sb.AppendFormat("{0}  ->  {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                }
                _currentIndex += count;
                return sb.ToString();
            }
            foreach (string key in _learning.Keys)
            {
                if (message.Contains(key))
                {
                    return _learning[key];
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
            if (menuName == "reload")
            {
                LoadFromFile();
                MessageBox.Show("已完成重载，共" + _learning.Count + "条数据", "操作提示");
            }
            else if (menuName == "about")
            {
                MessageBox.Show("学话鹦鹉。\r\n能学会一些对话信息。\r\n当前信息条数：" + _learning.Count, "关于插件");
            }
        }

        public string StatusChanged(Dictionary<string, object> info, string newStatus)
        {
            if (Enabled)
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
            if (Enabled)
            {
                return string.Format("{0} 你好，我正在等待你的输入。", TranslateMessageUser.UserNick);
            }
            return null;
        }
    }
}
