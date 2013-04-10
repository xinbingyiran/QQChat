using MessageDeal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MeIn
{
    internal struct meinItem
    {
        public Int64 score;
        public Int64 time;
        public string nick;
        public string mark;
    }

    public class MyApi : IMessageDeal
    {
        private Random r = new Random();
        private string _filePath;
        private Dictionary<string, meinItem> _meinAll;
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
            {"设置","setting"},
            {"关于","about"}
        };

        public Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"注册","注册用户，只有注册用户可以签到。"},
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
            _filePath = assemblay.Location;
            _filePath = _filePath.Substring(0, _filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filePath = _filePath + this.GetType().FullName + ".db";
            LoadFromFile();
        }

        public void LoadFromFile()
        {
            try
            {
                _meinAll.Clear();
                string[] lines = File.ReadAllLines(_filePath);
                Int64 loadtime = DateTime.Now.Ticks;
                foreach (string line in lines)
                {
                    string[] items = line.Split(new char[] { ' ' }, 5, StringSplitOptions.None);
                    if (items.Length == 5)
                    {
                        Int64 score;
                        Int64 time;
                        if (!Int64.TryParse(items[1], out score))
                        {
                            score = 0;
                        }
                        if (!Int64.TryParse(items[2], out time))
                        {
                            time = loadtime;
                        }
                        string nick = items[3].Replace("<space/>", " ").Substring(1);
                        string mark = items[4].Replace("<space/>", " ").Substring(1);
                        if (_meinAll.ContainsKey(items[0]))
                        {
                            score += _meinAll[items[0]].score;
                            _meinAll[items[0]] = new meinItem()
                            {
                                score = score,
                                time = time,
                                nick = nick,
                                mark = mark,
                            };
                        }
                        else
                        {
                            _meinAll.Add(items[0], new meinItem()
                            {
                                score = score,
                                time = time,
                                nick = nick,
                                mark = mark,
                            });
                        }
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
        }

        public void SaveToFile()
        {
            var lines = _meinAll.Select(ele =>
                ele.Key + " "
                + ele.Value.score + " "
                + ele.Value.time + " -"
                + ele.Value.nick.Replace(" ", "<space/>") + " -"
                + ele.Value.mark.Replace(" ", "<space/>") + " "
                );
            File.WriteAllLines(_filePath, lines);
        }

        public string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            string p1 = "000000";
            string p2 = info[TranslateMessageUser.UserNum.Key].ToString();
            string nick = info[TranslateMessageUser.UserNick.Key] as string ?? "";
            string mark = info[TranslateMessageUser.UserMarkName.Key] as string ?? "";
            nick = string.Format("{0}[{1}]", nick, p2);
            return DealMessage(message, p1, p2, nick, mark, false);
        }

        public string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            string p1 = info[TranslateMessageGroup.GroupNum.Key].ToString();
            string p2 = info[TranslateMessageGroup.MemberNum.Key].ToString();
            string nick = info[TranslateMessageGroup.MemberNick.Key] as string;
            string card = info[TranslateMessageGroup.MemberCard.Key] as string;
            nick = string.Format("{0}[{1}]", nick, p2);
            return DealMessage(message, p1, p2, nick, card, true);
        }

        private string _personStr = "";
        private string _successStr =
@"{0}，签到成功：
本次签到获取{1}积分，
当前拥有{2}积分
感谢使用签到服务。
{3}";
        private string _timeoutStr =
@"{0}，签到失败：
上次签到时间为{1}
当前拥有{2}积分
感谢使用签到服务。
{3}";
        //        private string _unregStr =
        //@"{0}，签到失败：
        //由于当前用户尚未注册，
        //暂时无法提供此服务
        //请发送""注册""进行用户注册。
        //{1}";
        private string _hasregedStr =
@"{0}，注册失败：
当前用户已进行过注册了，
请不要重复注册。
{1}";
        private string _regokStr =
@"{0}，注册成功：
注册当前用户成功，
现在可以使用签到服务了。
{1}";
        private Int64 timespan = new TimeSpan(2, 0, 0).Ticks;

        private string DealMessage(string message, string p1, string p2, string nick, string mark, bool isGroup)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            string name = isGroup ? (string.IsNullOrEmpty(mark) ? nick : mark) : nick;
            if (message == "签到")
            {
                string uin = p1 + '|' + p2;
                if (!_meinAll.ContainsKey(uin))
                {
                    //return string.Format(_unregStr, nick, _personStr);
                    _meinAll.Add(uin, new meinItem() { score = 0, time = DateTime.MinValue.Ticks, nick = nick, mark = mark });
                }
                else if (DateTime.Now.Ticks - _meinAll[uin].time < timespan)
                {
                    return string.Format(_timeoutStr, name, new DateTime(_meinAll[uin].time).ToString("yyyy-MM-dd HH:mm:ss"), _meinAll[uin].score, _personStr);
                }
                Int32 i = r.Next(14) + 1;
                _meinAll[uin] = new meinItem() { score = _meinAll[uin].score + i, time = DateTime.Now.Ticks, nick = nick, mark = mark };
                SaveToFile();
                return string.Format(_successStr, name, i, _meinAll[uin].score, _personStr);
            }
            else if (message == "注册")
            {
                string uin = p1 + '|' + p2;
                if (_meinAll.ContainsKey(uin))
                {
                    return string.Format(_hasregedStr, name, _personStr);
                }
                else
                {
                    _meinAll.Add(uin, new meinItem() { score = 0, time = DateTime.Now.Ticks - timespan, nick = nick, mark = mark });
                    SaveToFile();
                    return string.Format(_regokStr, name, _personStr);
                }
            }
            else
            {

            }
            return null;
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "setting")
            {
                setting s = new setting();
                s.Message = _personStr;
                s.ShowDialog();
                _personStr = s.Message;
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
