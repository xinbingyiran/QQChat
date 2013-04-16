using MessageDeal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HaveAJoke
{
    public class MyAPI : IMessageDeal
    {
        public string IName
        {
            get { return "随机回复笑话插件"; }
        }

        public bool Enabled
        {
            get;
            set;
        }

        private Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"重载","reload"},
            {"关于","about"}
        };

        public Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"笑话","随机获取一则笑话"}
        };

        public Dictionary<string, string> Filters
        {
            get { return _filters; }
        }

        private List<string[]> jokes;
        private Random r = new Random();
        private Int32 _count = 0;
        private string _filepath;

        public MyAPI()
        {
            jokes = new List<string[]>();
            var assemblay = this.GetType().Assembly;
            var filedir = assemblay.Location;
            filedir = filedir.Substring(0, filedir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filepath = filedir + this.GetType().FullName + ".db";
            LoadPara();
        }

        private void LoadPara()
        {
            jokes.Clear();
            if (!File.Exists(_filepath))
            {
                File.WriteAllText(_filepath, "[\"无标题\",\"无笑话\"]");
                jokes.Add(new string[] { "无标题", "无笑话" });
            }
            try
            {
                var lines = File.ReadAllLines(_filepath);
                foreach (var line in lines)
                {
                    jokes.Add(JsonConvert.DeserializeObject<string[]>(line));
                }
            }
            catch (Exception)
            {
            }
            _count = jokes.Count;
        }

        public string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            return DealMessage(message);
        }

        public string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            return DealMessage(message);
        }

        private string DealMessage(string message)
        {
            if (message.Trim() == "笑话")
            {
                if (_count > 0)
                {
                    var items = jokes[r.Next(_count)];
                    return string.IsNullOrEmpty(items[0]) ? items[1] : items[0] + "\r\n" + items[1];//0 title,1 content
                }
            }
            return null;
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "reload")
            {
                LoadPara();
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这是一个笑话插件\r\n当你输入签到，会随机回复一条笑话。\r\n当前笑话数量为：" + _count, "软件说明");
            }
        }

        public string StatusChanged(Dictionary<string, object> info, string newStatus)
        {
            return null;
        }

        public string Input(Dictionary<string, object> info)
        {
            return null;
        }
    }
}
