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

        private static readonly string lineSep = @"<br/>";

        private int _currentIndex;

        private string _filePath;

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
            {"关于","about"},
        };

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"-学 问题 答案","学说话[问题不能有空格]"},
            {"-忘 问题","忘记学过的话"},
            {"-列[ 开始位置]","列出学会的问题,从开始位置开始的相临五条内容"},
            {"-查 问题关键字","查看学会的问题,从问题查答案"},
            {"-反 回答关键字","查看学会的问题,从答案查问题"},
            {"-问 问题","问问题"},
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
                    if (items.Length == 2 && items[0].Length >= 2)
                    {
                        if (_learning.ContainsKey(items[0]))
                        {
                            _learning[items[0]] += lineSep + items[1];
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
            if (message.Length < 2 || message[0] != '-')
                return null;
            message = message.Trim().Replace(Environment.NewLine, lineSep);
            var tag = message[1];
            message = message.Substring(2).Trim();
            switch (tag)
            {
                case '学':
                    {
                        var wd = message.Split(new char[] { ' ' }, StringSplitOptions.None);
                        if (wd.Length != 2)
                        {
                            return null;
                        }
                        if (wd[0].Length < 2)
                        {
                            return null;
                        }
                        if (_learning.ContainsKey(wd[0]))
                            _learning[wd[0]] = wd[1];
                        else
                            _learning.Add(wd[0], wd[1]);
                        SaveToFile();
                        return "已经学会此问题：" + wd[0];
                    }
                case '忘':
                    {
                        if (message.Length < 2)
                        {
                            return "呵呵，问题请至少输入两个字哦。";
                        }
                        if (_learning.ContainsKey(message))
                            _learning.Remove(message);
                        SaveToFile();
                        return "已经忘记此问题：" + message;
                    }
                case '列':
                    {
                        int count = 5;
                        try
                        {
                            _currentIndex = Convert.ToInt32(message);
                        }
                        catch (Exception)
                        {
                        }
                        StringBuilder sb = new StringBuilder();
                        if (_currentIndex >= _learning.Count)
                        {
                            _currentIndex = 0;
                        }
                        sb.AppendLine("问题列表：" + _currentIndex);
                        foreach (KeyValuePair<string, string> filter in _learning.Skip(_currentIndex).Take(count))
                        {
                            sb.AppendFormat("{0}  -->  {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                        }
                        _currentIndex += count;
                        return sb.ToString();
                    }
                case '查':
                    {
                        var list = new Dictionary<string,string>();
                        foreach (KeyValuePair<string,string> item in _learning)
                        {
                            if (item.Key.Contains(message))
                            {
                                list.Add(item.Key,item.Value);
                            }
                            if (list.Count == 5)
                                break;
                        }
                        if (list.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (KeyValuePair<string, string> filter in list)
                            {
                                sb.AppendFormat("{0}  -->  {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                            }
                            return sb.ToString();
                        }
                        else
                        {
                            return "没有找到合适的结果。";
                        }
                    }
                    break;
                case '反':
                    {
                        var list = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, string> item in _learning)
                        {
                            if (item.Value.Contains(message))
                            {
                                list.Add(item.Key, item.Value);
                            }
                            if (list.Count == 5)
                                break;
                        }
                        if (list.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (KeyValuePair<string, string> filter in list)
                            {
                                sb.AppendFormat("{0}  -->  {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                            }
                            return sb.ToString();
                        }
                        else
                        {
                            return "没有找到合适的结果。";
                        }
                    }
                    break;
                case '问':
                    {
                        foreach (string key in _learning.Keys)
                        {
                            var itemindex = message.IndexOf(key);
                            if (itemindex >= 0 && itemindex <= 5)
                            {
                                return _learning[key].Replace(lineSep, Environment.NewLine);
                            }
                        }
                        return "这个问题的答案还没人教过我哎。";
                    }
                default:
                    break;
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
}
