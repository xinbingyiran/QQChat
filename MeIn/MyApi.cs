using MessageDeal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MeIn
{
    internal struct meinItem
    {
        public Int64 mein;
        public Int64 score;
        public Int64 time;
        public string nick;
        public string mark;
    }

    internal struct iniItem
    {
        public Int32 min;
        public Int32 mintomax;
        public Int64 timespan;
        public string item;
        public string pdata;
    }

    public class MyApi : IMessageDeal
    {
        private Random r = new Random();
        private string _meinfilePath;
        private string _inifilePath;
        private Dictionary<string, meinItem> _meinAll;
        private iniItem _iniItem;
        public string IName
        {
            get { return "签到插件"; }
        }

        public bool Enabled
        {
            get;
            set;
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"重载","reload"},
            {"设置","setting"},
            {"关于","about"}
        };

        public Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"签到","个人签到，我的世界，你曾经来过。"}
        };


        public Dictionary<string, string> Filters
        {
            get { return _filters; }
        }

        public MyApi()
        {
            _meinAll = new Dictionary<string, meinItem>();
            var assemblay = this.GetType().Assembly;
            var filedir = assemblay.Location;
            filedir = filedir.Substring(0, filedir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _meinfilePath = filedir + this.GetType().FullName + ".db";
            _inifilePath = filedir + this.GetType().FullName + ".ini";
            LoadParas();
        }

        public void LoadParas()
        {
            try
            {
                _meinAll.Clear();
                string[] lines = File.ReadAllLines(_meinfilePath);
                Int64 loadtime = DateTime.Now.Ticks;
                foreach (string line in lines)
                {
                    KeyValuePair<string, meinItem> item;
                    try
                    {
                        item = JsonConvert.DeserializeObject<KeyValuePair<string, meinItem>>(line);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (_meinAll.ContainsKey(item.Key))
                    {
                        var newitem = item.Value;
                        newitem.score += _meinAll[item.Key].score;
                        _meinAll[item.Key] = newitem;
                    }
                    else
                    {
                        _meinAll.Add(item.Key, item.Value);
                    }
                }
                if (_meinAll.Count != lines.Length)
                {
                    SaveToFile();
                }
            }
            catch (Exception)
            {
            }
            try
            {
                string text = File.ReadAllText(_inifilePath);
                _iniItem = JsonConvert.DeserializeObject<iniItem>(text);
            }
            catch (Exception)
            {
                _iniItem = new iniItem
                {
                    item = "积分",
                    pdata = "[gmemo]",
                    min = 1,
                    mintomax = 14,
                    timespan = new TimeSpan(4, 0, 0).Ticks
                };
            }
        }

        public void SaveToFile()
        {
            var lines = new string[_meinAll.Count];
            int index = 0;
            foreach (KeyValuePair<string, meinItem> i in _meinAll)
            {
                lines[index] = JsonConvert.SerializeObject(i);
                index++;
            }
            File.WriteAllLines(_meinfilePath, lines);
        }

        public void SavePara()
        {
            File.WriteAllText(_inifilePath, JsonConvert.SerializeObject(_iniItem));
        }

        public string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            string p1 = "000000";
            string p2 = info[TranslateMessageUser.UserNum.Key].ToString();
            string nick = info[TranslateMessageUser.UserNick.Key] as string ?? "";
            string mark = info[TranslateMessageUser.UserMarkName.Key] as string ?? "";
            return DealMessage(message, p1, p2, nick, mark, false);
        }

        public string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            string p1 = info[TranslateMessageGroup.GroupNum.Key].ToString();
            string p2 = info[TranslateMessageGroup.MemberNum.Key].ToString();
            string nick = info[TranslateMessageGroup.MemberNick.Key] as string;
            string card = info[TranslateMessageGroup.MemberCard.Key] as string;
            return DealMessage(message, p1, p2, nick, card, true);
        }

        private string DealMessage(string message, string p1, string p2, string nick, string mark, bool isGroup)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            string name = string.Format("{0}[{1}]", isGroup ? (string.IsNullOrEmpty(mark) ? nick : mark) : nick, p2);
            if (message == "签到")
            {
                string uin = p1 + '|' + p2;
                long leave = 0;
                DateTime now = DateTime.Now;
                if (!_meinAll.ContainsKey(uin))
                {
                    //return string.Format(_unregStr, nick, _personStr);
                    _meinAll.Add(uin, new meinItem() { mein = 0, score = 0, time = DateTime.MinValue.Ticks, nick = nick, mark = mark });
                }
                else
                {

                    var ntime = _meinAll[uin].time + _iniItem.timespan;
                    if (ntime > now.Ticks )
                    {
                        leave = ntime - now.Ticks;
                        return string.Format(
    @"{0}，签到失败：
上次签到时间为{1:yyyy-MM-dd HH:mm:ss}
距下次可签到剩余{2}:{3:D2}:{4:D2}
共成功签到{5}次,获得{6}{7},
{8}",
                            name,//0
                            new DateTime(_meinAll[uin].time),//1
                            leave / TimeSpan.TicksPerHour,//2
                            leave % TimeSpan.TicksPerHour / TimeSpan.TicksPerMinute,//3
                            leave % TimeSpan.TicksPerMinute / TimeSpan.TicksPerSecond,//4
                            _meinAll[uin].mein,//5
                            _meinAll[uin].score,//6
                            _iniItem.item,//7
                            _iniItem.pdata//8
                            );
                    }
                }
                Int32 i = r.Next(_iniItem.mintomax) + _iniItem.min;
                _meinAll[uin] = new meinItem() { mein = _meinAll[uin].mein + 1, score = _meinAll[uin].score + i, time = now.Ticks, nick = nick, mark = mark };
                SaveToFile();
                leave = _iniItem.timespan;
                return string.Format(
@"{0}，签到成功：
{1:yyyy-MM-dd HH:mm:ss}获取{2}{3}，
距下次可签到剩余{4}:{5:D2}:{6:D2}
共成功签到{7}次,获得{8}{3},
感谢使用签到服务。
{9}",
                    name,//0
                    now,//1
                    i,//2
                    _iniItem.item,//3
                    leave / TimeSpan.TicksPerHour,//4
                    leave % TimeSpan.TicksPerHour / TimeSpan.TicksPerMinute,//5
                    leave % TimeSpan.TicksPerMinute / TimeSpan.TicksPerSecond,//6
                    _meinAll[uin].mein,
                    _meinAll[uin].score,
                    _iniItem.pdata
                    );
            }
            return null;
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "reload")
            {
                LoadParas();
            }
            else if (menuName == "setting")
            {
                setting s = new setting();
                s.SaveItem = _iniItem;
                if (s.ShowDialog() == DialogResult.OK)
                {
                    _iniItem = s.SaveItem;
                    SavePara();
                }
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这是一个签到插件\r\n当你输入签到，代表你曾经到过。", "软件说明");
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
