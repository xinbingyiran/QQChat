using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebQQ2.Extends;

namespace WebQQ2.WebQQ2
{
    public class QQ_Smart : QQ_Base
    {
        #region smart

        public event EventHandler<FriendEventArgs> MessageFriendReceived;
        public event EventHandler<GroupEventArgs> MessageGroupReceived;
        public event EventHandler<ErrorEventArgs> GetMessageError;

        protected long qq_smart_msg_id_start = 73500000;
        protected long qq_smart_msg_id = 0;

        private static readonly string qq_smart_weburl = "http://d1.web2.qq.com/";
        private static readonly string qq_smart_referurl = qq_smart_weburl + "proxy.html?v=20151105001&callback=1&id=2";

        private static readonly string qq_smart_ptlogin2uiurl = "https://ui.ptlogin2.qq.com/";
        private static readonly string qq_smart_prelogin = qq_smart_ptlogin2uiurl + "cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t={0}";

        private static readonly string qq_smart_ptlogin2sslurl = "https://ssl.ptlogin2.qq.com/";
        private static readonly string qq_smart_qrshow = qq_smart_ptlogin2sslurl + "ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t={0}";
        private static readonly string qq_smart_qrlogin = qq_smart_ptlogin2sslurl + "ptqrlogin?webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=0-0-{0}&mibao_css=m_webqq&t=undefined&g=1&js_type=0&js_ver=10153&login_sig=&pt_randsalt=0";

        private static readonly string qq_smart_apiurl = "http://s.web2.qq.com/api/";
        private static readonly string qq_smart_getvfwebqq = qq_smart_apiurl + "getvfwebqq?ptwebqq={0}&clientid={1}&psessionid=&t={2}";
        private static readonly string qq_smart_get_group_info_ext2 = qq_smart_apiurl + "get_group_info_ext2?gcode={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_smart_get_user_friends2 = qq_smart_apiurl + "get_user_friends2";
        private static readonly string qq_smart_get_group_name_list_mask2 = qq_smart_apiurl + "get_group_name_list_mask2";
        private static readonly string qq_smart_get_self_info2 = qq_smart_apiurl + "get_self_info2?t={0}";

        private static readonly string qq_smart_webchannelurl = "http://d1.web2.qq.com/channel/";
        private static readonly string qq_smart_login2 = qq_smart_webchannelurl + "login2";
        private static readonly string qq_smart_poll2 = qq_smart_webchannelurl + "poll2";
        private static readonly string qq_smart_send_buddy_msg2 = qq_smart_webchannelurl + "send_buddy_msg2";
        private static readonly string qq_smart_send_qun_msg2 = qq_smart_webchannelurl + "send_qun_msg2";
        private static readonly string qq_smart_get_online_buddies2 = qq_smart_webchannelurl + "get_online_buddies2?vfwebqq={0}&clientid={1}&psessionid={2}&t={3}";
        private static readonly string qq_smart_get_recent_list2 = qq_smart_webchannelurl + "get_recent_list2";

        protected class qq_smart_login2_post
        {
            public string ptwebqq { get; set; }
            public long clientid { get; set; }
            public string psessionid { get; set; }
            public string status { get; set; }
        };

        protected class qq_smart_poll2_post
        {
            public string ptwebqq { get; set; }
            public long clientid { get; set; }
            public string psessionid { get; set; }
            public string key { get; set; }
        };
        public interface IContent
        {
            object Result { get; }
        }

        public class StringContent : IContent
        {
            public string Content { get; set; }
            public object Result
            {
                get
                {
                    return Content;
                }
            }
        }
        public class FaceContent : IContent
        {
            public int FaceNumber { get; set; }
            public object Result
            {
                get
                {
                    return new object[] { "face", FaceNumber };
                }
            }
        }
        protected class FontContent : IContent
        {
            public string name { get; set; }
            public int size { get; set; }
            public int[] style { get; set; }
            public string color { get; set; }

            public static FontContent Default = new FontContent();
            public FontContent()
            {
                this.name = "宋体";
                this.size = 10;
                this.style = new int[] { 0, 0, 0 };
                this.color = "000000";
            }
            public object Result
            {
                get
                {
                    return new object[] { "font",
                        new Dictionary<string, object> {
                            { "name", name },
                            { "size", size },
                            { "style", style },
                            { "color", color },
                        } };
                }
            }
        }
        protected long GetMsgID()
        {
            if (qq_smart_msg_id <= qq_smart_msg_id_start)
            {
                qq_smart_msg_id = qq_smart_msg_id_start;
            }
            qq_smart_msg_id++;
            return qq_smart_msg_id;
        }

        protected class qq_smart_get_info_post
        {
            public string vfwebqq { get; set; }
            public string hash { get; set; }
        }
        protected class qq_smart_send_buddy_msg2_post
        {
            public long to { get; set; }
            public string content { get; set; }
            public long face { get; set; }
            public long clientid { get; set; }
            public long msg_id { get; set; }
            public string psessionid { get; set; }

            public qq_smart_send_buddy_msg2_post()
            {
                face = 594;
            }
        };
        protected class qq_smart_send_qun_msg2_post
        {
            public long group_uin { get; set; }
            public string content { get; set; }
            public long face { get; set; }
            public long clientid { get; set; }
            public long msg_id { get; set; }
            public string psessionid { get; set; }

            public qq_smart_send_qun_msg2_post()
            {
                face = 594;
            }
        };
        protected override void OnInit()
        {
            base.OnInit();
            var curMs = DateTime.UtcNow.Second;
            var pgv_info = "s" + ((Math.Round(_random.NextDouble() * 2147483647) * curMs) % 10000000000);
            var pgv_pvid = (Math.Round(_random.NextDouble() * 2147483647) * curMs) % 10000000000;
            _cookiecontainer.Add(new Cookie("pgv_info", "ssid=" + pgv_info, "/", ".qq.com"));
            _cookiecontainer.Add(new Cookie("pgv_pvid", pgv_pvid.ToString(), "/", ".qq.com"));
        }
        public bool SmartPreLogin()
        {
            string result = _helper.GetUrlText(string.Format(qq_smart_prelogin, QQHelper.GetTime()), qq_smart_referurl);
            int cookieSet = 0;
            foreach (Cookie v in _cookiecontainer.GetCookies(new Uri("http://ptlogin2.qq.com")))
            {
                if (string.Compare(v.Name, "pt_login_sig") == 0)
                {
                    _user.PtLoginSig = v.Value;
                    cookieSet |= 0x01;
                }
                else if (string.Compare(v.Name, "pt_clientip") == 0)
                {
                    _user.PtClientIp = v.Value;
                    cookieSet |= 0x02;
                }
                else if (string.Compare(v.Name, "pt_serverip") == 0)
                {
                    _user.PtServerIp = v.Value;
                    cookieSet |= 0x04;
                }
            }
            foreach (Cookie v in _cookiecontainer.GetCookies(new Uri("http://ui.ptlogin2.qq.com")))
            {
                if (string.Compare(v.Name, "ptui_identifier") == 0)
                {
                    _user.PtUiIdentifier = v.Value;
                    cookieSet |= 0x08;
                }
            }
            return cookieSet == 0x0f;
        }
        public Image GetQrImage()
        {
            return new Bitmap(
                _helper.GetUrlStream(
                    string.Format(qq_smart_qrshow, _random.NextDouble())
                    , qq_smart_referurl)
                );
        }

        public IEnumerable<string> DoSmartLogin(bool logClient = false)
        {
            int rnumber = 100;
            while (true)
            {
                Thread.Sleep(1000);
                rnumber += _random.Next(2000 - 10, 2000 + 10);
                string result = _helper.GetUrlText(
                    string.Format(qq_smart_qrlogin, rnumber), qq_smart_referurl
                    );
                if (string.IsNullOrWhiteSpace(result))
                {
                    yield return "检查出错";
                    yield break;
                }
                var ptuiCBStart = "ptuiCB('";
                var ptuiCBEnd = "');";
                var resultitems = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                result = null;
                foreach (var item in resultitems)
                {
                    var item0 = item.Replace(" ", "");
                    if (!item0.StartsWith(ptuiCBStart) || !item0.EndsWith(ptuiCBEnd))
                    {
                        continue;
                    }
                    result = item0;
                    break;
                }
                if (result == null)
                {
                    yield return "检查出错";
                    yield break;
                }
                result = result.Replace(" ", "");
                //"ptuiCB('66','0','','0','二维码未失效。(428870048)', '');"
                result = result.Substring(ptuiCBStart.Length);
                result = result.Substring(0, result.Length - ptuiCBEnd.Length);
                var items = result.Split(new string[] { "','" }, StringSplitOptions.None);
                if (items.Length != 6)
                {
                    yield return "检查出错";
                    yield break;
                }
                if (items[0] != "0")
                {
                    yield return items[4];
                }
                else
                {
                    yield return "获取cookie";
                    result = _helper.GetUrlText(items[2], qq_smart_referurl);

                    var cookies = _cookiecontainer.GetCookies(new Uri("http://ui.ptlogin2.qq.com"));
                    foreach (Cookie v in cookies)
                    {
                        if (string.Compare(v.Name, "ptwebqq") == 0)
                        {
                            _user.PtWebQQ = v.Value;
                        }
                        else if (string.Compare(v.Name, "skey") == 0)
                        {
                            _user.skey = v.Value;
                            _user.GTK = QQHelper.getGTK(_user.skey);
                        }
                        else if (string.Compare(v.Name, "uin") == 0)
                        {
                            _user.QQNum = v.Value.TrimStart('o').TrimStart('0');
                        }
                    }
                    foreach (Cookie v in cookies)
                    {
                        if (v.Name == "ptnick_" + _user.QQNum)
                        {
                            var utf8name = v.Value;
                            if (!string.IsNullOrWhiteSpace(utf8name))
                            {
                                if (utf8name.Length % 2 == 0)
                                {
                                    var bytes = new byte[utf8name.Length / 2];
                                    for (int i = 0; i < utf8name.Length; i += 2)
                                    {
                                        bytes[i / 2] = byte.Parse(utf8name.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                                    }
                                    _user.QQName = Encoding.UTF8.GetString(bytes);
                                }
                            }
                        }
                    }
                    if (!logClient)
                    {
                        yield break;
                    }
                    yield return "获取cookie2";

                    result = _helper.GetUrlText(string.Format(qq_smart_getvfwebqq, _user.PtWebQQ, _user.ClientID, QQHelper.GetTime()), qq_smart_referurl);

                    var obj = QQHelper.FromJson<Dictionary<string, object>>(result);
                    if (!obj.ContainsKey("retcode") || !obj.ContainsKey("result"))
                    {
                        yield return "获取vfwebqq失败" + result;
                        yield break;
                    }
                    if (obj["retcode"].ToString() != "0")
                    {
                        yield return "获取vfwebqq失败" + result;
                        yield break;
                    }

                    var obj2 = obj["result"] as Dictionary<string, object>;
                    if (obj2 == null)
                    {
                        yield return "获取vfwebqq失败" + result;
                        yield break;
                    }

                    _user.VfWebQQ = obj2["vfwebqq"].ToString();

                    yield return "获取vfwebqq";
                    string url = qq_smart_login2;
                    var postItem = new qq_smart_login2_post
                    {
                        status = "online",
                        ptwebqq = _user.PtWebQQ,
                        clientid = _user.ClientID,
                    };
                    string para = QQHelper.ToPostData(postItem);
                    string retstr = _helper.PostUrlText(url, Encoding.UTF8.GetBytes(para), qq_smart_referurl);
                    //_user.VfWebQQ = null;
                    _user.PsessionID = null;
                    _user.Status = "offline";

                    if (retstr != null && retstr.Length > 0)
                    {
                        Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                        if (root["retcode"] as int? == 0)
                        {
                            Dictionary<string, object> rootresult = root["result"] as Dictionary<string, object>;
                            //_user.VfWebQQ = rootresult["vfwebqq"] as string;
                            _user.PsessionID = rootresult["psessionid"] as string;
                            _user.Status = rootresult["status"] as string;
                            _user.Uin = rootresult["uin"].ToString();
                            if (_user.Status != "offline")
                            {
                                _user.LoginTime = DateTime.Now;
                            }
                            yield break;
                        }
                        else
                        {
                            yield return QQHelper.ToJson(root);
                        }
                    }
                    else
                    {
                        yield return "登录失败";
                    }
                    yield break;
                }
            }
        }


        public IEnumerable<object> DoMessageLoop()
        {
            string url = qq_smart_poll2;
            var postItem = new qq_smart_poll2_post
            {
                ptwebqq = _user.PtWebQQ,
                clientid = _user.ClientID,
                psessionid = _user.PsessionID,
            };
            string para = QQHelper.ToPostData(postItem);
            while (true)
            {
                string str = _helper.PostUrlText(url, Encoding.UTF8.GetBytes(para), qq_smart_referurl, int.MaxValue);
                //str = null;未获取信息
                if (str == null)
                {
                    yield return "网络中断";
                    yield break;
                }
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(str);
                var retcode = (root["retcode"] as int?).GetValueOrDefault(-1);
                if (retcode == 102)
                {
                    //没信息
                }
                else if (retcode == 103 || retcode == 121)
                {
                    yield return "掉线";
                    yield break;
                }
                else if (retcode == 0)
                {
                    System.Collections.ArrayList list = root["result"] as System.Collections.ArrayList;
                    for (int i = list.Count - 1; i >= 0; i--)//倒序
                    {
                        Dictionary<string, object> ele = list[i] as Dictionary<string, object>;
                        if (ele == null) continue;
                        StringBuilder sb = new StringBuilder();
                        string poll_type = ele["poll_type"] as string;
                        Dictionary<string, object> messagevalue = ele["value"] as Dictionary<string, object>;
                        //sb.AppendFormat("poll_type:{0}\r\n", poll_type);
                        switch (poll_type)
                        {
                            case "buddies_status_change":
                                {

                                    string status = messagevalue["status"].ToString();
                                    QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["uin"]), true);
                                    if (friend != null)
                                    {
                                        if (friend.status != status)
                                        {
                                            friend.status = status;
                                            if (MessageFriendReceived != null)
                                            {
                                                MessageFriendReceived(this, new FriendEventArgs(friend, 0, DateTime.Now, MessageEventType.MESSAGE_STATUS, status));
                                            }
                                        }
                                    }
                                }
                                break;

                            case "message":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]), true);
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        MessageFriendReceived(this, new FriendEventArgs(friend, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_COMMON, msgs));
                                    }
                                }
                                break;
                            case "sess_message":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        QQFriend friend = _user.GetUserSess(Convert.ToInt64(messagevalue["from_uin"]));
                                        friend.num = Convert.ToInt64(messagevalue["ruin"]);
                                        friend.id = Convert.ToInt64(messagevalue["id"]);
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        MessageFriendReceived(this, new FriendEventArgs(friend, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_SESS, msgs));
                                    }
                                }
                                break;

                            case "shake_message":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]), true);
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        MessageFriendReceived(this, new FriendEventArgs(friend, msgid, DateTime.Now, MessageEventType.MESSAGE_SHAKE));
                                    }
                                }
                                break;
                            case "group_message":
                                {
                                    if (MessageGroupReceived != null)
                                    {
                                        QQGroup group = _user.GetUserGroup(Convert.ToInt64(messagevalue["from_uin"]));
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        QQGroupMember member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        if (member == null)
                                        {
                                            new Task(() =>
                                            {
                                                RefreshGroupInfo(group);
                                                member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                                MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_COMMON, msgs));
                                            }).Start();
                                        }
                                        else
                                        {
                                            MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_COMMON, msgs));
                                        }
                                    }
                                }
                                break;
                            case "group_web_message":
                                {
                                    if (MessageGroupReceived != null)
                                    {
                                        QQGroup group = _user.GetUserGroup(Convert.ToInt64(messagevalue["from_uin"]));
                                        string xml = messagevalue["xml"] as string;
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "xml", xml } };
                                        QQGroupMember member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        if (member == null)
                                        {
                                            new Task(() =>
                                            {
                                                RefreshGroupInfo(group); member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                                member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                                MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, DateTime.Now, MessageEventType.MESSAGE_COMMON, msgs));
                                            }).Start();
                                        }
                                        else
                                        {
                                            MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, DateTime.Now, MessageEventType.MESSAGE_COMMON, msgs));
                                        }
                                    }
                                }
                                break;
                            case "file_message":
                                if (MessageFriendReceived != null)
                                {
                                    QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]), true);
                                    Dictionary<string, object> msgs = new Dictionary<string, object>
                                        {
                                            {"poll_type", poll_type}
                                        };
                                    foreach (string key in messagevalue.Keys)
                                    {
                                        msgs.Add(key, messagevalue[key]);
                                    }
                                    long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                    MessageFriendReceived(this, new FriendEventArgs(friend, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_FILE, msgs));
                                }
                                break;
                            case "push_offfile":
                                if (MessageFriendReceived != null)
                                {
                                    QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]), true);
                                    Dictionary<string, object> msgs = new Dictionary<string, object>
                                        {
                                            {"poll_type", poll_type}
                                        };
                                    foreach (string key in messagevalue.Keys)
                                    {
                                        msgs.Add(key, messagevalue[key]);
                                    }
                                    long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                    MessageFriendReceived(this, new FriendEventArgs(friend, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_OFFLINE, msgs));
                                }
                                break;
                            case "kick_message":
                                {
                                    _user.Status = "offline";
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>
                                            {
                                                {"poll_type", poll_type}
                                            };
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(null, 0, DateTime.Now, MessageEventType.MESSAGE_KICK, msgs));
                                    }
                                }
                                break;
                            case "input_notify":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]), true);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>
                                            {
                                                {"poll_type", poll_type}
                                            };
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(friend, msgid, DateTime.Now, MessageEventType.MESSAGE_INPUT, msgs));
                                    }
                                }
                                break;
                            case "tips":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>
                                            {
                                                {"poll_type", poll_type}
                                            };
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(null, 0, DateTime.Now, MessageEventType.MESSAGE_SYSTEM, msgs));
                                    }
                                }
                                break;
                            case "sys_g_message":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>
                                            {
                                                {"poll_type", poll_type}
                                            };
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(null, 0, DateTime.Now, MessageEventType.MESSAGE_SYSTEM, msgs));
                                    }
                                }
                                break;
                            case "system_message":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>
                                            {
                                                {"poll_type", poll_type}
                                            };
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(null, 0, DateTime.Now, MessageEventType.MESSAGE_SYSTEM, msgs));
                                    }
                                }
                                break;
                            default:
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>
                                            {
                                                {"poll_type", poll_type}
                                            };
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(null, 0, DateTime.Now, MessageEventType.MESSAGE_UNKNOW, msgs));
                                    }
                                }
                                break;
                        }
                        yield return poll_type;
                    }
                }
                else
                {
                    yield return str;
                    yield break;
                }
            }
        }

        public void RefreshGroupInfo(QQGroup group, int timeout = 60000)
        {
            GetGroupMembers(group, timeout);
        }

        public void GetGroupMembers(QQGroup group, int timeout = 60000)
        {
            string url = string.Format(qq_smart_get_group_info_ext2, group.code, _user.VfWebQQ, QQHelper.GetTime());
            string resultStr = _helper.GetUrlText(url,qq_smart_referurl,timeout);
            Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
            if (root != null && root["retcode"] as int? == 0)
            {
                Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                Dictionary<long, QQGroupMember> oldmembers = new Dictionary<long, QQGroupMember>(group.allMembers);
                group.Clear();

                #region ginfo
                if (result.Keys.Contains("ginfo"))
                {
                    var ginfo = result["ginfo"] as Dictionary<string, object>;
                    group.face = Convert.ToInt64(ginfo["face"]);
                    group.memo = ginfo["memo"].ToString();
                    group._class = Convert.ToInt64(ginfo["class"]);
                    group.fingermemo = ginfo["fingermemo"].ToString();
                    group.code = Convert.ToInt64(ginfo["code"]);
                    group.createtime = Convert.ToInt64(ginfo["createtime"]);
                    group.flag = Convert.ToInt64(ginfo["flag"]);
                    group.level = Convert.ToInt64(ginfo["level"]);
                    group.name = ginfo["name"].ToString();
                    group.gid = Convert.ToInt64(ginfo["gid"]);
                    group.option = Convert.ToInt64(ginfo["option"]);

                    long ownerid = Convert.ToInt64(ginfo["owner"]);

                    #region members
                    if (ginfo.Keys.Contains("members"))
                    {
                        var members = ginfo["members"] as ArrayList;
                        var minfo = result["minfo"] as ArrayList;
                        var vipinfo = result["vipinfo"] as ArrayList;
                        //var stats = result["stats"] as ArrayList;
                        //var cards = result["cards"] as ArrayList;
                        for (int i = 0; i < members.Count; i++)
                        {
                            Dictionary<string, object> member = members[i] as Dictionary<string, object>;
                            Dictionary<string, object> m = minfo[i] as Dictionary<string, object>;
                            Dictionary<string, object> vip = vipinfo[i] as Dictionary<string, object>;
                            long uin = Convert.ToInt64(member["muin"]);
                            QQGroupMember newitem = null;
                            if (oldmembers.ContainsKey(uin))
                            {
                                newitem = oldmembers[uin];
                            }
                            newitem = new QQGroupMember()
                            {
                                uin = uin
                            };
                            newitem.mflag = Convert.ToInt64(member["mflag"]);
                            //minfo
                            newitem.province = m["province"].ToString();
                            newitem.country = m["country"].ToString();
                            newitem.city = m["city"].ToString();
                            newitem.nick = m["nick"].ToString();
                            newitem.gender = m["gender"].ToString();
                            //vipinfo
                            newitem.vip_level = Convert.ToInt64(vip["vip_level"]);
                            newitem.is_vip = Convert.ToInt64(vip["is_vip"]);
                            //stats
                            //for (int j = 0; j < stats.Count; j++)
                            //{
                            //    var stat = stats[j] as Dictionary<string, object>;
                            //    long statuin = Convert.ToInt64(stat["uin"]);
                            //    if (statuin == uin)
                            //    {
                            //        if (member.ContainsKey("client_type")) newitem.client_type = Convert.ToInt64(stat["client_type"]);
                            //        if (member.ContainsKey("stat")) newitem.stat = Convert.ToInt64(stat["stat"]);
                            //        stats.RemoveAt(j);
                            //        break;
                            //    }
                            //}
                            ////cards
                            //for (int j = 0; j < cards.Count; j++)
                            //{
                            //    var card = cards[j] as Dictionary<string, object>;
                            //    long carduin = Convert.ToInt64(card["muin"]);
                            //    if (carduin == uin)
                            //    {
                            //        newitem.card = card["card"].ToString();
                            //        cards.RemoveAt(j);
                            //        break;
                            //    }
                            //}
                            //
                            group.allMembers.Add(newitem.uin, newitem);
                            if ((newitem.mflag & 1) != 0)
                            {
                                group.leaders.Add(newitem.uin, newitem);
                            }
                            else if (newitem.uin == ownerid)
                            {
                                group.owner = newitem;
                            }
                            else
                            {
                                group.members.Add(newitem.uin, newitem);
                            }
                        }
                    }
                }
                #endregion

                //#region minfo

                //if (result.Keys.Contains("minfo"))
                //{
                //    var minfo = result["minfo"] as ArrayList;
                //    foreach (Dictionary<string, object> m in minfo)
                //    {
                //        var newitem = group.GetGroupMember(Convert.ToInt64(m["uin"]));
                //        if (newitem == null)
                //        {
                //            newitem = new QQGroupMember() { uin = Convert.ToInt64(m["uin"]) };
                //            group.allMembers.Add(newitem.uin, newitem);
                //            group.members.Add(newitem.uin, newitem);
                //        }
                //        newitem.province = m["province"].ToString();
                //        newitem.country = m["country"].ToString();
                //        newitem.city = m["city"].ToString();
                //        newitem.nick = m["nick"].ToString();
                //        newitem.gender = m["gender"].ToString();
                //    }
                //}
                //#endregion

                //#region vipinfo

                //if (result.Keys.Contains("vipinfo"))
                //{
                //    var vipinfo = result["vipinfo"] as ArrayList;
                //    foreach (Dictionary<string, object> vip in vipinfo)
                //    {
                //        var newitem = group.GetGroupMember(Convert.ToInt64(vip["u"]));
                //        if (newitem == null)
                //        {
                //            newitem = new QQGroupMember() { uin = Convert.ToInt64(vip["u"]) };
                //            group.allMembers.Add(newitem.uin, newitem);
                //            group.members.Add(newitem.uin, newitem);
                //        }
                //        newitem.vip_level = Convert.ToInt64(vip["vip_level"]);
                //        newitem.is_vip = Convert.ToInt64(vip["is_vip"]);
                //    }
                //}
                //#endregion

                #region stats

                if (result.Keys.Contains("stats"))
                {
                    var stats = result["stats"] as ArrayList;
                    foreach (Dictionary<string, object> stat in stats)
                    {
                        var newitem = group.GetGroupMember(Convert.ToInt64(stat["uin"]));
                        if (newitem == null)
                        {
                            newitem = new QQGroupMember() { uin = Convert.ToInt64(stat["uin"]) };
                            group.allMembers.Add(newitem.uin, newitem);
                            group.members.Add(newitem.uin, newitem);
                        }
                        if (stat.ContainsKey("client_type")) newitem.client_type = Convert.ToInt64(stat["client_type"]);
                        if (stat.ContainsKey("stat")) newitem.stat = Convert.ToInt64(stat["stat"]);
                    }
                }
                #endregion
                #region cards

                if (result.Keys.Contains("cards"))
                {
                    var cards = result["cards"] as ArrayList;
                    foreach (Dictionary<string, object> card in cards)
                    {
                        var newitem = group.GetGroupMember(Convert.ToInt64(card["muin"]));
                        if (newitem == null)
                        {
                            newitem = new QQGroupMember() { uin = Convert.ToInt64(card["muin"]) };
                            group.allMembers.Add(newitem.uin, newitem);
                            group.members.Add(newitem.uin, newitem);
                        }
                        newitem.card = card["card"].ToString();
                    }
                }
                #endregion

                #endregion
            }
        }

        public void GetUserList()
        {
            string url = qq_smart_get_user_friends2;
            var postItem = new qq_smart_get_info_post
            {
                vfwebqq = _user.VfWebQQ,
                hash = QQHelper.GetToken(_user),
            };
            string para = QQHelper.ToPostData(postItem);
            string resultStr = _helper.PostUrlText(url, Encoding.UTF8.GetBytes(para), qq_smart_referurl, int.MaxValue);
            Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
            if (root != null && root["retcode"] as int? == 0)
            {
                //Dictionary<long, QQFriendDir> oldgroups = new Dictionary<long, QQFriendDir>(_user.QQFriends.GroupList);
                Dictionary<long, QQFriend> oldfriends = new Dictionary<long, QQFriend>(_user.QQFriends.FriendList);
                Dictionary<long, QQFriend> oldsesses = new Dictionary<long, QQFriend>(_user.QQFriends.SessList);
                Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                _user.QQFriends.Clear();
                foreach (Dictionary<string, object> item in result["categories"] as ArrayList)
                {
                    _user.QQFriends.Add(new QQFriendDir()
                    {
                        index = Convert.ToInt64(item["index"]),
                        sort = Convert.ToInt64(item["sort"]),
                        name = item["name"].ToString(),
                    });
                }
                QQFriendDir nofriendgroup = new QQFriendDir()
                {
                    index = -1,
                    sort = -1,
                    name = "[已删除]",
                };
                _user.QQFriends.Add(nofriendgroup);
                var friends = result["friends"] as ArrayList;
                var info = result["info"] as ArrayList;
                var vipinfo = result["vipinfo"] as ArrayList;
                for (int i = 0; i < friends.Count; i++)
                {
                    Dictionary<string, object> friend = friends[i] as Dictionary<string, object>;
                    Dictionary<string, object> infoi = info[i] as Dictionary<string, object>;
                    Dictionary<string, object> vipinfoi = vipinfo[i] as Dictionary<string, object>;
                    QQFriend newfriend = null;
                    long uin = Convert.ToInt64(friend["uin"]);
                    if (oldfriends.ContainsKey(uin))
                    {
                        newfriend = oldfriends[uin];
                    }
                    else if (oldsesses.ContainsKey(uin))
                    {
                        newfriend = oldsesses[uin];
                    }
                    else
                    {
                        newfriend = new QQFriend() { uin = uin };
                    }
                    newfriend.flag_friends = Convert.ToInt64(friend["flag"]);
                    newfriend.categories = Convert.ToInt64(friend["categories"]);
                    //info
                    newfriend.face = Convert.ToInt64(infoi["face"]);
                    newfriend.flag_info = Convert.ToInt64(infoi["flag"]);
                    newfriend.nick = Convert.ToString(infoi["nick"]);
                    //vipinfo
                    newfriend.is_vip = Convert.ToInt64(vipinfoi["is_vip"]);
                    newfriend.vip_level = Convert.ToInt64(vipinfoi["vip_level"]);
                    //
                    _user.QQFriends.Add(newfriend);
                }
                //foreach (Dictionary<string, object> infoi in result["info"] as ArrayList)
                //{
                //    long uin = Convert.ToInt64(infoi["uin"]);
                //    QQFriend user = _user.GetUserFriend(uin, false);
                //    if (user != null)
                //    {
                //        user.face = Convert.ToInt64(infoi["face"]);
                //        user.flag_info = Convert.ToInt64(infoi["flag"]);
                //        user.nick = Convert.ToString(infoi["nick"]);
                //    }
                //}
                //foreach (Dictionary<string, object> vipinfoi in result["vipinfo"] as ArrayList)
                //{
                //    long uin = Convert.ToInt64(vipinfoi["u"]);
                //    QQFriend user = _user.GetUserFriend(uin, false);
                //    if (user != null)
                //    {
                //        user.is_vip = Convert.ToInt64(vipinfoi["is_vip"]);
                //        user.vip_level = Convert.ToInt64(vipinfoi["vip_level"]);
                //    }
                //}
                foreach (Dictionary<string, object> item in result["marknames"] as ArrayList)
                {
                    long uin = Convert.ToInt64(item["uin"]);
                    QQFriend user = _user.GetUserFriend(uin, false);
                    if (user != null)
                    {
                        user.markname = item["markname"].ToString();
                    }
                    else
                    {
                        QQFriend friend = null;
                        if (oldfriends.ContainsKey(uin))
                        {
                            friend = oldfriends[uin];
                        }
                        else if (oldsesses.ContainsKey(uin))
                        {
                            friend = oldsesses[uin];
                        }
                        else
                        {
                            friend = new QQFriend() { uin = uin };
                        }
                        friend.markname = item["markname"].ToString();
                        friend.categories = nofriendgroup.index;
                        _user.QQFriends.Add(friend);
                    }
                }
            }
        }

        public void GetUserOnlineList()
        {
            string url = string.Format(qq_smart_get_online_buddies2,_user.VfWebQQ,_user.ClientID,_user.PsessionID,QQHelper.GetTime());
            string resultStr = _helper.GetUrlText(url, qq_smart_referurl, int.MaxValue);
            Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
            if (root != null && root["retcode"] as int? == 0)
            {
                foreach (Dictionary<string, object> item in root["result"] as ArrayList)
                {
                    QQFriend user = _user.GetUserFriend(Convert.ToInt64(item["uin"]), false);
                    if (user != null)
                    {
                        user.status = item["status"].ToString();
                    }
                }
            }
        }

        public void GetGroupList()
        {
            string url = qq_smart_get_group_name_list_mask2;
            var postItem = new qq_smart_get_info_post
            {
                vfwebqq = _user.VfWebQQ,
                hash = QQHelper.GetToken(_user),
            };
            string para = QQHelper.ToPostData(postItem);
            string resultStr = _helper.PostUrlText(url, Encoding.UTF8.GetBytes(para), qq_smart_referurl, int.MaxValue);
            Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
            if (root != null && root["retcode"] as int? == 0)
            {
                Dictionary<long, QQGroup> oldgroup = new Dictionary<long, QQGroup>(_user.QQGroups.GroupList);
                _user.QQGroups.Clear();
                Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                foreach (Dictionary<string, object> item in result["gnamelist"] as ArrayList)
                {
                    long gid = Convert.ToInt64(item["gid"]);
                    QQGroup group = null;
                    if (oldgroup.ContainsKey(gid))
                    {
                        group = oldgroup[gid];
                    }
                    else
                    {
                        group = new QQGroup()
                        {
                            gid = Convert.ToInt64(item["gid"]),
                        };
                    }
                    group.flag = Convert.ToInt64(item["flag"]);
                    group.code = Convert.ToInt64(item["code"]);
                    group.name = item["name"].ToString();
                    _user.QQGroups.Add(group);
                }
            }
        }

        public bool SendBuddyMessage(long uin, params IContent[] messages)
        {
            if (messages.Length == 0)
            {
                return false;
            }
            var messagesList = messages.ToList();
            messagesList.Add(FontContent.Default);
            var content = QQHelper.ToJson(messagesList.Select(e => e.Result));
            string url = qq_smart_send_buddy_msg2;
            var postItem = new qq_smart_send_buddy_msg2_post
            {
                clientid = _user.ClientID,
                psessionid = _user.PsessionID,
                msg_id = GetMsgID(),
                to = uin,
                content = content,
            };
            string para = QQHelper.ToPostData(postItem);
            string result = _helper.PostUrlText(url, Encoding.UTF8.GetBytes(para), qq_smart_referurl, int.MaxValue);
            //str = null;未获取信息
            if (result == null)
            {
                return false;
            }
            var dict = QQHelper.FromJson<Dictionary<string,object>>(result);
            if(dict.ContainsKey("errCode") && dict["errCode"].ToString() == "0")
            {
                return true;
            }
            return false;
        }
        public bool SendQunMessage(long uin, params IContent[] messages)
        {
            if (messages.Length == 0)
            {
                return false;
            }
            var messagesList = messages.ToList();
            messagesList.Add(FontContent.Default);
            var content = QQHelper.ToJson(messagesList.Select(e => e.Result));
            string url = qq_smart_send_qun_msg2;
            var postItem = new qq_smart_send_qun_msg2_post
            {
                clientid = _user.ClientID,
                psessionid = _user.PsessionID,
                msg_id = GetMsgID(),
                group_uin = uin,
                content = content,
            };
            string para = QQHelper.ToPostData(postItem);
            string result = _helper.PostUrlText(url, Encoding.UTF8.GetBytes(para), qq_smart_referurl, int.MaxValue);
            //str = null;未获取信息
            if (result == null)
            {
                return false;
            }
            var dict = QQHelper.FromJson<Dictionary<string, object>>(result);
            if (dict.ContainsKey("errCode") && dict["errCode"].ToString() == "0")
            {
                return true;
            }
            return false;
        }

        #endregion

    }
}