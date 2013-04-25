using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MessageDeal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Timer = System.Timers.Timer;
using System.Text;

namespace MeIn
{
    internal struct meinItem
    {
        public string uin;
        public string nick;
        public string mark;
        public Int64 mein;
        public Int64 score;
        public DateTime time;
        public DateTime lasttime;
        public string lastsay;
    }

    internal class MeinTimesComparer : IComparer<meinItem>
    {
        private MeinTimesComparer() { }

        private static readonly MeinTimesComparer _default = new MeinTimesComparer();

        public static MeinTimesComparer Default
        {
            get { return _default; }
        }

        public int Compare(meinItem x, meinItem y)
        {
            return y.mein.CompareTo(x.mein);
        }
    }

    internal class MeinScoreComparer : IComparer<meinItem>
    {
        private MeinScoreComparer() { }

        private static readonly MeinScoreComparer _default = new MeinScoreComparer();

        public static MeinScoreComparer Default
        {
            get { return _default; }
        }
        public int Compare(meinItem x, meinItem y)
        {
            return y.score.CompareTo(x.score);
        }
    }

    internal struct iniItem
    {
        public bool autoIn;
        public bool isEnable;
        public string item;
        public Int32 min;
        public Int32 mintomax;
        public Int32 top;
        public string pdata;
        public TimeSpan timespan;
    }

    public class MyApi : TMessage
    {
        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
            {
                {"设置", "setting"},
                {"关于", "about"}
            };

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
            {
                {"签到", "个人签到，我的世界，你曾经来过。"},
                {"签到排名", "个人签到，我的世界，你曾经来过。"},
                {"成绩排名", "个人签到，我的世界，你曾经来过。"}
            };

        private readonly Dictionary<string, meinItem> _meinAll;
        private readonly string _meinfilePath;
        private readonly object _saveLock;
        private readonly Random r = new Random();
        private iniItem _iniItem;
        private bool _saveFlag;
        private Timer _timer;

        public MyApi()
        {
            _saveLock = new object();
            _saveFlag = false;
            _meinAll = new Dictionary<string, meinItem>();
            Assembly assemblay = GetType().Assembly;
            string filedir = assemblay.Location;
            filedir = filedir.Substring(0, filedir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _meinfilePath = filedir + GetType().FullName + ".db";
            _iniItem = new iniItem
                {
                    isEnable = false,
                    autoIn = false,
                    item = "积分",
                    pdata = "",
                    min = 1,
                    mintomax = 14,
                    top = 5,
                    timespan = new TimeSpan(4, 0, 0)
                };
            new Task(() =>
                {
                    LoadParas();
                    _timer = new Timer();
                    _timer.AutoReset = true;
                    _timer.Interval = 300000;
                    _timer.Elapsed += _timer_Elapsed;
                    System.Threading.Thread.Sleep(new Random().Next(300000));
                    _timer.Start();
                }).Start();
        }

        public override string Setting
        {
            get { return JsonConvert.SerializeObject(_iniItem); }
            set
            {
                try
                {
                    _iniItem = JsonConvert.DeserializeObject<iniItem>(value);
                }
                catch (Exception)
                {
                }
            }
        }

        public override string PluginName
        {
            get { return "签到插件"; }
        }

        public override bool Enabled
        {
            get { return _iniItem.isEnable; }
            set { _iniItem.isEnable = value; }
        }

        public override Dictionary<string, string> Menus
        {
            get { return _menus; }
        }


        public override Dictionary<string, string> Filters
        {
            get { return _filters; }
        }

        public override string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            string p1 = "000000";
            string p2 = info[TranslateMessageUser.UserNum.Key].ToString();
            string nick = info[TranslateMessageUser.UserNick.Key] as string ?? "";
            string mark = info[TranslateMessageUser.UserMarkName.Key] as string ?? "";
            return DealMessage(message, p1, p2, nick, mark, false);
        }

        public override string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            string p1 = info[TranslateMessageGroup.GroupNum.Key].ToString();
            string p2 = info[TranslateMessageGroup.MemberNum.Key].ToString();
            var nick = info[TranslateMessageGroup.MemberNick.Key] as string;
            var card = info[TranslateMessageGroup.MemberCard.Key] as string;
            return DealMessage(message, p1, p2, nick, card, true);
        }

        public override void MenuClicked(string menuName)
        {
            if (menuName == "setting")
            {
                var s = new setting();
                s.SaveItem = _iniItem;
                if (s.ShowDialog() == DialogResult.OK)
                {
                    _iniItem = s.SaveItem;
                }
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这是一个签到插件\r\n当你输入签到，代表你曾经到过。", "软件说明");
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveToFile();
        }

        public override void OnExited()
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
                            var olditem =
                                JsonConvert.DeserializeObject<KeyValuePair<string, Dictionary<string, object>>>(line);
                            item = new meinItem
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
                        meinItem newitem = item;
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
                    KeyValuePair<string, meinItem>[] allArray = _meinAll.ToArray();
                    var lines = new string[allArray.Length];
                    var timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    for (int index = 0; index < allArray.Length; index++)
                    {
                        lines[index] = JsonConvert.SerializeObject(allArray[index].Value, timeConverter);
                    }
                    File.WriteAllLines(_meinfilePath, lines);
                    _saveFlag = false;
                }
            }
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
                    item = new meinItem
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
                    DateTime ntime = item.time + _iniItem.timespan;
                    if (ntime > now)
                    {
                        leave = ntime - now;
                        return string.Format(
                            @"{0}，签到失败
上次签到时间为{1:yyyy-MM-dd HH:mm:ss}
距下次可签到剩余{2}:{3:D2}:{4:D2}
共成功签到{5}次,获得{6}{7}
{8}",
                            name, //0
                            item.time, //1
                            leave.Ticks / TimeSpan.TicksPerHour, //2
                            leave.Minutes, //3
                            leave.Seconds, //4
                            item.mein, //5
                            item.score, //6
                            _iniItem.item, //7
                            _iniItem.pdata //8
                            );
                    }
                }
                Int32 i = r.Next(_iniItem.mintomax) + _iniItem.min;
                item = new meinItem
                    {
                        uin = uin,
                        mein = item.mein + 1,
                        score = item.score + i,
                        time = now,
                        nick = nick,
                        mark = mark,
                        lasttime = item.lasttime,
                        lastsay = item.lastsay,
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
                    name, //0
                    now, //1
                    i, //2
                    _iniItem.item, //3
                    leave.Ticks / TimeSpan.TicksPerHour, //4
                    leave.Minutes, //5
                    leave.Seconds, //6
                    item.mein,
                    item.score,
                    _iniItem.pdata
                    );
            }
            #endregion

            else if (message == "签到排名")
            {
                var gstr = p1 + "|";
                var gstrlen = gstr.Length;
                var items = _meinAll.Values.Where(ele => ele.uin.StartsWith(gstr)).ToArray();
                if (items.Length == 0)
                {
                    return "无排名";
                }
                Array.Sort(items, MeinTimesComparer.Default);
                int length = items.Length > _iniItem.top ? _iniItem.top : items.Length;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("排名结果：");
                for (int i = 0; i < length; i++)
                {
                    sb.AppendFormat("第{0}名：{1} 次 {2}[{3}]", i + 1, items[i].mein, items[i].mark == null ? items[i].nick : items[i].mark, items[i].uin.Substring(gstrlen));
                    sb.AppendLine();
                }
                return sb.ToString();
            }
            else if (message == "成绩排名")
            {
                var gstr = p1 + "|";
                var gstrlen = gstr.Length;
                var items = _meinAll.Values.Where(ele => ele.uin.StartsWith(p1 + "|")).ToArray();
                if (items.Length == 0)
                {
                    return "无排名";
                }
                Array.Sort(items, MeinScoreComparer.Default);
                int length = items.Length > _iniItem.top ? _iniItem.top : items.Length;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("排名结果：");
                for (int i = 0; i < length; i++)
                {
                    sb.AppendFormat("第{0}名：{1} {2} {3}[{4}]", i + 1, items[i].score, _iniItem.item, items[i].mark == null ? items[i].nick : items[i].mark, items[i].uin.Substring(gstrlen));
                    sb.AppendLine();
                }
                return sb.ToString();
            }
            else if (_iniItem.autoIn)
            {
                meinItem theitem;
                if (!_meinAll.ContainsKey(uin))
                {
                    //return string.Format(_unregStr, nick, _personStr);
                    theitem = new meinItem
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
    }
}