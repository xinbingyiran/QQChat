using MessageDeal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaveAJoke
{
    public class MyAPI : TMessage
    {

        public override string Setting
        {
            get
            {
                return (Enabled ? "1" : "0");
            }
            set
            {
                Enabled = value == "1";
            }
        }

        public override string PluginName
        {
            get { return "随机笑话"; }
        }

        private Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"重载","reload"},
        };

        public override Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"笑话","随机获取一则笑话"}
        };

        public override Dictionary<string, string> Filters
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
            new Task(() =>
                {
                    LoadPara();
                }).Start();
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
                var len = lines.Length;
                for (int i = 0; i < len; i++)
                {
                    try
                    {
                        jokes.Add(JsonConvert.DeserializeObject<string[]>(lines[i]));
                    }
                    catch (Exception ex)
                    {
                        if (OnMessage != null)
                        {
                            LastMessage = ex.Message + "@" + (i + 1);
                            OnMessage(this, EventArgs.Empty);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            _count = jokes.Count;
        }

        public override string DealMessage(string messageType, Dictionary<string, object> info, string message)
        {
            if (messageType != MessageType.MessageFriend && messageType != MessageType.MessageGroup)
            {
                return null;
            }
            if (message.Trim() == "笑话")
            {
                if (_count > 0)
                {
                    var items = jokes[r.Next(_count)];
                    return string.IsNullOrEmpty(items[0]) ? items[1] : items[0] + "\r\n" + items[1];//0 title,1 content
                }
                return null;
            }
            return null;
        }

        public override event EventHandler<EventArgs> OnMessage;

        public override void MenuClicked(string menuName)
        {
            if (menuName == "reload")
            {
                LoadPara();
                LastMessage = AboutMessage;
                if (OnMessage != null)
                {
                    OnMessage(this, EventArgs.Empty);
                }
            }
        }

        public override string AboutMessage
        {
            get
            {
                return "这是一个笑话插件\r\n当你输入签到，会随机回复一条笑话。\r\n当前笑话数量为：" + _count;
            }
        }

    }
}
