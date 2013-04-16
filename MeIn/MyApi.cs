using MessageDeal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MeIn
{
    internal struct meinItem
    {
        public string uin;
        public DateTime time;
        public Int64 mein;
        public Int64 score;
        public string nick;
        public string mark;
        public DateTime lasttime;
        public string lastsay;
    }

    internal struct iniItem
    {
        public Int32 min;
        public Int32 mintomax;
        public TimeSpan timespan;
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
        private bool _saveFlag;
        private object _saveLock;
        private System.Timers.Timer _timer;
        private bool _autoMein;

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
            {"记录启","start"},
            {"记录停","stop"},
            {"状态","status"},
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
            _saveLock = new object();
            _saveFlag = false;
            _autoMein = false;
            _meinAll = new Dictionary<string, meinItem>();
            var assemblay = this.GetType().Assembly;
            var filedir = assemblay.Location;
            filedir = filedir.Substring(0, filedir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _meinfilePath = filedir + this.GetType().FullName + ".db";
            _inifilePath = filedir + this.GetType().FullName + ".ini";
            LoadParas();
            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Interval = 5000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveToFile();
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
                    meinItem item;
                    try
                    {
                        item = JsonConvert.DeserializeObject<meinItem>(line);
                        if (item.uin == null || item.nick == null)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var olditem = JsonConvert.DeserializeObject<KeyValuePair<string, Dictionary<string, object>>>(line);
                            item = new meinItem()
                            {
                                uin = olditem.Key,
                                mein = Convert.ToInt64(olditem.Value["mein"]),
                                score = Convert.ToInt64(olditem.Value["score"]),
                                time = new DateTime(Convert.ToInt64(olditem.Value["time"])),
                                nick = olditem.Value["nick"] as string,
                                mark = olditem.Value["mark"] as string,
                                lasttime = new DateTime(Convert.ToInt64(olditem.Value["time"])),
                                lastsay = "签到",
                            };
                            SetSaveFlag();
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    if (_meinAll.ContainsKey(item.uin))
                    {
                        var newitem = item;
                        newitem.score += _meinAll[item.uin].score;
                        _meinAll[item.uin] = newitem;
                    }
                    else
                    {
                        _meinAll.Add(item.uin, item);
                    }
                }
                if (_meinAll.Count != lines.Length)
                {
                    SetSaveFlag();
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
                    timespan = new TimeSpan(4, 0, 0)
                };
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
                    var allArray = _meinAll.ToArray();
                    var lines = new string[allArray.Length];
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    for (int index = 0; index < allArray.Length; index++)
                    {
                        lines[index] = JsonConvert.SerializeObject(allArray[index].Value,timeConverter);
                    }
                    File.WriteAllLines(_meinfilePath, lines);
                    _saveFlag = false;
                }
            }
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
            string uin = p1 + '|' + p2;
            TimeSpan leave;
            DateTime now = DateTime.Now;
            #region 签到
            if (message == "签到")
            {
                meinItem item;
                if (!_meinAll.ContainsKey(uin))
                {
                    //return string.Format(_unregStr, nick, _personStr);
                    item = new meinItem()
                    {
                        uin = uin,
                        mein = 0,
                        score = 0,
                        time = DateTime.MinValue,
                        nick = nick,
                        mark = mark,
                    };
                    _meinAll.Add(uin, item);
                }
                else
                {
                    item = _meinAll[uin];
                    var ntime = item.time + _iniItem.timespan;
                    if (ntime > now)
                    {
                        leave = ntime - now;
                        return string.Format(
    @"{0}，签到失败
上次签到时间为{1:yyyy-MM-dd HH:mm:ss}
距下次可签到剩余{2}:{3:D2}:{4:D2}
共成功签到{5}次,获得{6}{7}
{8}",
                            name,//0
                            item.time,//1
                            leave.TotalHours,//2
                            leave.Minutes,//3
                            leave.Seconds,//4
                            item.mein,//5
                            item.score,//6
                            _iniItem.item,//7
                            _iniItem.pdata//8
                            );
                    }
                }
                Int32 i = r.Next(_iniItem.mintomax) + _iniItem.min;
                item = new meinItem()
                {
                    uin = uin,
                    mein = item.mein + 1,
                    score = item.score + i,
                    time = now,
                    nick = nick,
                    mark = mark,
                    lasttime = now,
                    lastsay = message,
                };
                _meinAll[uin] = item;
                SetSaveFlag();
                leave = _iniItem.timespan;
                return string.Format(
@"{0}，签到成功
{1:yyyy-MM-dd HH:mm:ss}获取{2}{3}
距下次可签到剩余{4}:{5:D2}:{6:D2}
共成功签到{7}次,获得{8}{3}
{9}",
                    name,//0
                    now,//1
                    i,//2
                    _iniItem.item,//3
                    leave.TotalHours,//4
                    leave.Minutes,//5
                    leave.Seconds,//6
                    item.mein,
                    item.score,
                    _iniItem.pdata
                    );
            }
            #endregion
            else if (_autoMein)
            {
                meinItem theitem;
                if (!_meinAll.ContainsKey(uin))
                {
                    //return string.Format(_unregStr, nick, _personStr);
                    theitem = new meinItem()
                    {
                        uin = uin,
                        mein = 0,
                        score = 0,
                        time = DateTime.MinValue,
                        nick = nick,
                        mark = mark,
                    };
                    _meinAll.Add(uin, theitem);
                }
                else
                {
                    theitem = _meinAll[uin];
                }
                theitem.lasttime = now;
                theitem.lastsay = message;
                _meinAll[uin] = theitem;
                SetSaveFlag();
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
            else if (menuName == "start")
            {
                _autoMein = true;
                MessageBox.Show("启用成功。", "自动状态");
            }
            else if (menuName == "stop")
            {
                _autoMein = false;
                MessageBox.Show("停用成功。", "自动状态");
            }
            else if (menuName == "status")
            {
                MessageBox.Show(string.Format("当前状态为{0}。", _autoMein ? "启用" : "停用"), "自动状态");
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
