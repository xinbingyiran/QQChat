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
    public class MessageDeal1 : IMessageDeal
    {

        private Dictionary<string, string> _learning;

        private string filename;

        private bool _enabled = false;

        public string IName
        {
            get { return "示例1"; }
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"设置","setting"},
            {"关于","about"},
        };

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"@问：问题 答：答案","教我学说话[问题不能有空格]"},
            {"@删：问题","删除我学过的话"},
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

        public MessageDeal1()
        {
            _learning = new Dictionary<string, string>();
            var assemblay = this.GetType().Assembly;
            filename = assemblay.Location;
            filename = filename.Substring(0, filename.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            filename = filename + this.GetType().FullName + ".db";
        }

        public void LoadFromFile()
        {
            _learning.Clear();
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                string[] items = line.Split(new char[] { ' ' }, 2, StringSplitOptions.None);
                if (items.Length == 2 && !_learning.ContainsKey(items[0]))
                {
                    _learning.Add(items[0], items[1]);
                }
            }
        }

        public void SaveToFile()
        {
            var lines = _learning.Select(ele => ele.Key + ' ' + ele.Value);
            File.WriteAllLines(filename, lines);
        }

        public string DealFriendMessage(string message)
        {
            if (_enabled)
            {
                Regex r = new Regex(@"^@问：(\S*)\s*答：(.*)$");
                Match m = r.Match(message);
                if (m.Success)
                {
                    if (_learning.ContainsKey(m.Groups[1].Value))
                        _learning[m.Groups[1].Value] = m.Groups[2].Value;
                    else
                        _learning.Add(m.Groups[1].Value, m.Groups[2].Value);
                    SaveToFile();
                    return "我学会了，你可以问我" + m.Groups[1];
                }
            }
            return null;
        }

        public string DealGroupMessage(string message)
        {
            if (_enabled)
            {
                return message + " from [nick]([card])";
            }
            return null;
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "setting")
            {
                _enabled = !_enabled;
                MessageBox.Show("现在的状态是" + (_enabled ? "启用" : "停用"), "状态指示");
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这只是一个示例", "关于");
            }
        }

        public string StatusChanged(string newStatus)
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

        public string Input()
        {
            if (_enabled)
            {
                return string.Format("{0} 你好，我正在等待你的输入。", TranslateMessageUser.UserNick);
            }
            return null;
        }
    }
}
