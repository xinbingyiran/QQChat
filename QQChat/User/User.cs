using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Web;
using QQChat.Extends;
using System.Threading.Tasks;
using System.Threading;

namespace QQChat.User
{
    #region classes

    public class QQUser_FriendGroup
    {
        public long index { get; set; }
        public long sort { get; set; }
        public string name { get; set; }
    }

    public class QQUser_Friend
    {
        public object tag { get; set; }
        public long num { get; set; }
        public long uin { get; set; }
        public long flag_friends { get; set; }
        public long is_vip { get; set; }
        public long vip_level { get; set; }
        public long face { get; set; }
        public long flag_info { get; set; }
        public string markname { get; set; }
        public string nick { get; set; }
        public long categories { get; set; }
        public string status { get; set; }

        public bool ShowMarkName { get; set; }

        public string LongName
        {
            get
            {
                if (this.nick == null && this.num == 0)
                {
                    return this.uin.ToString();
                }
                if (this.markname != null && ShowMarkName)
                {
                    return string.Format("{0}[{1}]({2})", this.nick, this.markname, this.num);
                }
                return string.Format("{0}({1})", this.nick, this.num);
            }
        }

        public string ShortName
        {
            get
            {
                if (this.nick == null && this.num == 0)
                {
                    return this.uin.ToString();
                }
                if (this.markname != null && ShowMarkName)
                {
                    return string.Format("{0}[{1}]({2})", this.nick, this.markname);
                }
                return string.Format("{0}", this.nick);
            }
        }

        public string LongNameWithStatus
        {
            get
            {
                return string.Format("{0} - {1}", LongName, status);
            }
        }

        public string Name
        {
            get
            {
                if (this.markname != null && this.markname.Length > 0)
                {
                    return string.Format("{0}", this.markname);
                }
                if (this.nick != null && this.nick.Length > 0)
                {
                    return string.Format("{0}", this.nick);
                }
                if (this.num != 0)
                {
                    return string.Format("QQ:{0}", this.num);
                }
                return string.Format("UIN:{0}", this.uin);
            }
        }

        public QQUser_Friend()
        {
            ShowMarkName = true;
            status = "offline";
        }

    }

    public class QQUserFriends
    {
        private Dictionary<string, QQUser_FriendGroup> _groups;
        private Dictionary<string, QQUser_Friend> _friends;

        public Dictionary<string, QQUser_FriendGroup> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public Dictionary<string, QQUser_Friend> Friends
        {
            get { return _friends; }
            set { _friends = value; }
        }

        public QQUserFriends()
        {
            _groups = new Dictionary<string, QQUser_FriendGroup>();
            _friends = new Dictionary<string, QQUser_Friend>();
        }

        public void Clear()
        {
            _friends.Clear();
            _groups.Clear();
        }

        public void Add(QQUser_FriendGroup item)
        {
            _groups.Add(item.index.ToString(), item);
        }

        public void Add(QQUser_Friend item)
        {
            _friends.Add(item.uin.ToString(), item);
        }
    }

    public class QQUser_Group
    {
        public long flag { get; set; }
        public string name { get; set; }
        public long gid { get; set; }
        public long code { get; set; }
        public string ShortName
        {
            get
            {
                return string.Format("{0}", name);
            }
        }

        public string LongName
        {
            get
            {
                return string.Format("{0}[{1}]", name, code);
            }
        }
    }

    public class QQUserGroups
    {
        private Dictionary<string, QQUser_Group> _groups;

        public Dictionary<string, QQUser_Group> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public QQUserGroups()
        {
            _groups = new Dictionary<string, QQUser_Group>();
        }

        public void Add(QQUser_Group item)
        {
            _groups.Add(item.gid.ToString(), item);
        }

        public void Clear()
        {
            _groups.Clear();
        }
    }


    #endregion

    public class QQUser
    {

        #region urldefine

        static readonly string qq_referurl = "http://d.web2.qq.com/proxy.html";
        static readonly string qq_check = "http://check.ptlogin2.qq.com/check?uin={0}&appid=1003903&r={1:f16}";
        static readonly string qq_getimage = "http://captcha.qq.com/getimage?aid=1003903&r={0:f16}&uin={1}";
        static readonly string qq_login = "http://ptlogin2.qq.com/login?u={0}&p={1}&verifycode={2}&webqq_type=10&remember_uin=1&login2qq=1&aid=1003903&u1=http%3A%2F%2Fw.qq.com%2Floginproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=3-6-9305&mibao_css=m_webqq&t=1&g=1";
        static readonly string qq_login2 = "http://d.web2.qq.com/channel/login2";
        static readonly string qq_get_user_friends2 = "http://s.web2.qq.com/api/get_user_friends2";
        static readonly string qq_get_friend_info2 = "http://s.web2.qq.com/api/get_friend_info2?tuin={0}&verifysession=&code=&vfwebqq={1}&t={2}";
        static readonly string qq_getface_user = "http://face3.qun.qq.com/cgi/svr/face/getface?cache=1&type=1&fid=0&uin={0}&vfwebqq={1}&t={2}";
        static readonly string qq_getface_qun = "http://face1.qun.qq.com/cgi/svr/face/getface?cache=0&type=4&fid=0&uin={0}&vfwebqq={1}";//uin = code
        static readonly string qq_get_group_name_list_mask2 = "http://s.web2.qq.com/api/get_group_name_list_mask2";
        static readonly string qq_get_group_info = "http://s.web2.qq.com/api/get_group_info?gcode=%5B{0}%5D&retainKey=memo&vfwebqq={1}&t={2}";
        static readonly string qq_get_group_info_ext2 = "http://s.web2.qq.com/api/get_group_info_ext2?gcode={0}&vfwebqq={1}&t={2}";
        static readonly string qq_send_qun_msg2 = "http://d.web2.qq.com/channel/send_qun_msg2";
        static readonly string qq_get_online_buddies2 = "http://d.web2.qq.com/channel/get_online_buddies2?clientid={0}&psessionid={1}&t={2}";
        static readonly string qq_get_single_long_nick2 = "http://s.web2.qq.com/api/get_single_long_nick2?tuin={0}&vfwebqq={1}&t={2}";
        static readonly string qq_set_long_nick2 = "http://s.web2.qq.com/api/set_long_nick2";
        static readonly string qq_get_msg_tip = "http://web.qq.com/web2/get_msg_tip?uin=&tp=1&id=0&retype=1&rc=18&lv=3&t={0}";
        static readonly string qq_get_discu_list_new2 = "http://d.web2.qq.com/channel/get_discu_list_new2?clientid={0}&psessionid={1}&vfwebqq={2}&t={3}";
        static readonly string qq_get_recent_list2 = "http://d.web2.qq.com/channel/get_recent_list2";
        static readonly string qq_poll2 = "http://d.web2.qq.com/channel/poll2";
        static readonly string qq_send_buddy_msg2 = "http://d.web2.qq.com/channel/send_buddy_msg2";
        static readonly string qq_get_qq_level2 = "http://s.web2.qq.com/api/get_qq_level2?tuin={0}&vfwebqq={1}&t={2}";
        static readonly string qq_get_friend_uin2 = "http://s.web2.qq.com/api/get_friend_uin2?tuin={0}&verifysession=&type=1&code=&vfwebqq={1}&t={2}";
        static readonly string qq_change_status2 = "http://d.web2.qq.com/channel/change_status2?newstatus={0}&clientid={1}&psessionid={2}&t={3}";
        static readonly string qq_logout2 = "http://d.web2.qq.com/channel/logout2?ids=&clientid={0}&psessionid={1}&t={2}";
        static readonly string qq_search_qq_by_uin2 = "http://s.web2.qq.com/api/search_qq_by_uin2?tuin={0}&verifysession={1}&code={2}&vfwebqq={3}&t={4}";
        static readonly string qq_get_c2cmsg_sig2 = "http://d.web2.qq.com/channel/get_c2cmsg_sig2?id=3195852032&to_uin={0}&service_type=0&clientid={1}&psessionid={2}&t={3}";
        static readonly string qq_get_stranger_info2 = "http://s.web2.qq.com/api/get_stranger_info2?tuin={0}&verifysession=&gid=0&code=&vfwebqq={1}&t={2}";

        static readonly string qq_refuse_file2 = "http://d.web2.qq.com/channel/refuse_file2?to={0}&lcid={1}&clientid={2}&psessionid={3}&t={4}";
        static readonly string qq_get_file2 = "http://d.web2.qq.com/channel/get_file2?lcid={0}&guid={1}&to={2}&psessionid={3}&count=1&time={4}&clientid={5}";
        static readonly string qq_notify_offfile2 = "http://d.web2.qq.com/channel/notify_offfile2?to={0}&file_name={1}&file_size={2}&action=2&psessionid={3}&clientid={4}&t={5}";
        static readonly string qq_get_offfile2 = "http://{0}:{1}/{2}?ver=2173&rkey={3}&range=0";
        static readonly string qq_get_offpic2 = "http://d.web2.qq.com/channel/get_offpic2?file_path={0}&f_uin={1}&clientid={2}&psessionid={3}";

        static readonly string qq_face = "http://0.web.qstatic.com/webqqpic/style/face/{0}.gif";

        static readonly string qq_cface = "http://d.web2.qq.com/channel/get_cface2?lcid={0}&guid={1}&to={2}&count=5&time=1&clientid={3}&psessionid={4}";

        #endregion

        #region postparadefine

        Dictionary<string, object> qq_login2_post = new Dictionary<string, object>()
        {
           {"status",""},
           {"ptwebqq",""},
           {"passwd_sig",""},
           {"clientid",""},
        };

        Dictionary<string, object> qq_get_user_friends2_post = new Dictionary<string, object>()
        {
            {"h",""},
            {"vfwebqq",""},
        };
        Dictionary<string, object> qq_get_group_name_list_mask2_post = new Dictionary<string, object>()
        {
            {"vfwebqq",""},
        };


        private int qq_send_buddy_msg2_post_msg_id = 0;
        Dictionary<string, object> qq_send_buddy_msg2_post = new Dictionary<string, object>()
        {
            {"to",0},
            {"face",0},
            {"content",""},
            {"msg_id",56410001},
            {"clientid",""},
            {"psessionid",""}
        };

        ArrayList qq_send_buddy_msg2_post_content = new ArrayList(){
		        "",
                "",
                new ArrayList(){        
                    "font",
                    new Dictionary<string, object>()
                    {
                        {"name","宋体"},
                        {"size","10"},
                        {"style",new ArrayList(){0,0,0}},
                        {"color","000000"},
                    },
                },
            };
        Dictionary<string, object> qq_send_qun_msg2_post = new Dictionary<string, object>(){
            {"group_uin",0},
            {"content",""},
            {"msg_id",49800001}, 
            {"clientid",""},
            {"psessionid",""}
         };

        ArrayList qq_send_qun_msg2_post_content = new ArrayList(){
		        "",
                "",
                new ArrayList(){        
                    "font",
                    new Dictionary<string, object>()
                    {
                        {"name","宋体"},
                        {"size","10"},
                        {"style",new ArrayList(){0,0,0}},
                        {"color","000000"},
                    },
                },
            };

        Dictionary<string, object> qq_poll2_post = new Dictionary<string, object>()
        {
            {"clientid",""},
            {"psessionid",""},
            {"key",""},
            {"ids",new ArrayList()},
        };
        Dictionary<string, object> qq_get_recent_list2_post = new Dictionary<string, object>()
        {
            {"vfwebqq",""},
            {"clientid",""},
            {"psessionid",""},
        };
        Dictionary<string, object> qq_set_long_nick2_post = new Dictionary<string, object>()
        {
            {"nlk",""},
            {"vfwebqq",""}
        };

        #endregion

        #region event
        public event EventHandler<FriendEventArgs> MessageFriendReceived;
        public event EventHandler<GroupEventArgs> MessageGroupReceived;

        #endregion

        #region paradefine

        private Random _random;
        private CookieContainer _cookiecontainer;
        private string _qqnum;
        private string _qqname;
        private string _uin;
        private string _clientid;
        private string _ptwebqq;
        private string _vfwebqq;
        private string _psessionid;
        private string _status;
        private QQUserFriends _friends;
        private QQUserGroups _groups;
        private Task _messageTask;
        private CancellationTokenSource _messageTaskCts;
        private DateTime? _loginTime = null;

        public DateTime? LoginTime
        {
            get { return _loginTime; }
        }

        public QQUserGroups Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public QQUserFriends Friends
        {
            get { return _friends; }
            set { _friends = value; }
        }

        public string QQNum
        {
            get { return _qqnum; }
            set { _qqnum = value; }
        }

        public string QQName
        {
            get { return _qqname; }
        }

        public CookieContainer Cookies
        {
            get { return _cookiecontainer; }
        }

        public string Status
        {
            get { return _status; }
        }

        public bool IsPreLoged
        {
            get
            {
                return (_ptwebqq != null && _ptwebqq.Length > 0);
            }
        }

        #endregion

        #region handle

        #endregion

        #region constructor

        private void OnCreated()
        {
            _status = "offline";
            _random = new Random();
            _cookiecontainer = new CookieContainer();
            _clientid = GenerateClientID();
            _friends = new QQUserFriends();
            _groups = new QQUserGroups();
            qq_send_buddy_msg2_post_msg_id = (_random.Next(9000) + 1000) * 10000 + 1;
        }

        public QQUser()
        {
            _qqnum = "0";
            OnCreated();

        }

        public QQUser(string qqnum)
        {
            _qqnum = qqnum;
            OnCreated();
        }

        #endregion

        #region functiondeclare

        private readonly static string ptui_checkVCStart = "ptui_checkVC(";
        private readonly static string ptui_checkVCEnd = ");";
        public string GetVerifyCode()
        {
            string url = String.Format(qq_check, _qqnum, _random.NextDouble());
            string result = GetUrlText(url);
            string vc = "";
            _uin = "";
            try
            {
                if (result.StartsWith(ptui_checkVCStart) && result.EndsWith(ptui_checkVCEnd))
                {
                    result = result.Substring(ptui_checkVCStart.Length);
                    result = result.Substring(0, result.Length - 2);
                    string[] subresult = result.Split(',');
                    if (subresult.Length == 3)
                    {
                        vc = subresult[1].Trim().Trim('\'');
                        _uin = subresult[2].Trim().Trim('\'');
                        AnylizeUin();
                    }
                }
            }
            catch (Exception)
            {
            }
            return vc;
        }

        private void AnylizeUin()
        {
            string[] newstr = _uin.Split(new string[] { "\\x" }, StringSplitOptions.RemoveEmptyEntries);
            _uin = "";
            foreach (string str in newstr)
            {
                char c = (char)Convert.ToByte(str, 16);
                _uin += c;
            }
        }

        public Image GetVerifyImage()
        {

            try
            {
                string url = String.Format(qq_getimage, _random.NextDouble(), _qqnum);
                Stream result = GetUrlStream(url);
                return new Bitmap(result);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string LogQQ(string password, string vercode)
        {
            string mpass = getPassword(_uin, password, vercode);
            string url = string.Format(qq_login, _qqnum, mpass, vercode);
            string result = GetUrlText(url);
            _ptwebqq = null;
            foreach (Cookie v in _cookiecontainer.GetCookies(new Uri(url)))
            {
                if (string.Compare(v.Name, "ptwebqq") == 0)
                {
                    _ptwebqq = v.Value;
                    break;
                }
            }
            if (result.StartsWith("ptuiCB"))
            {
                int start = result.IndexOf('(');
                string sub = result.Substring(start + 1, result.LastIndexOf(')') - start - 1);
                string[] sublist = sub.Split(',');
                _qqname = sublist[5].Trim().Trim('\'');
                result = _qqname + ":" + sublist[4].Trim().Trim('\'');
            }
            return result;
        }

        public string LogQQ2(string logStatus)
        {
            if (_ptwebqq == null || _ptwebqq.Length == 0)
            {
                return "尚未全局登录成功";
            }
            string url = qq_login2;
            qq_login2_post["status"] = logStatus;
            qq_login2_post["ptwebqq"] = _ptwebqq;
            qq_login2_post["clientid"] = _clientid;
            string para = ToPostData(qq_login2_post);
            string retstr = PostUrlText(url, Encoding.Default.GetBytes(para));
            _vfwebqq = null;
            _psessionid = null;
            _status = "offline";
            if (retstr != null && retstr.Length > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Dictionary<string, object> root = (Dictionary<string, object>)jss.Deserialize(retstr, typeof(object));
                if (root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    _vfwebqq = result["vfwebqq"] as string;
                    _psessionid = result["psessionid"] as string;
                    _status = result["status"] as string;
                    if (_status != "offline")
                    {
                        _loginTime = DateTime.Now;
                    }
                    //StartGetMessage();
                }
                else
                {
                    return ToJson(root);
                }
            }
            return null;
        }

        public string LogOutQQ2()
        {
            if (_ptwebqq == null || _ptwebqq.Length == 0)
            {
                return "尚未全局登录成功";
            }
            string url = string.Format(qq_logout2, _clientid, _psessionid, GetTime()); ;
            string retstr = GetUrlText(url);
            if (retstr != null && retstr.Length > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Dictionary<string, object> root = (Dictionary<string, object>)jss.Deserialize(retstr, typeof(object));
                if (root["retcode"] as int? == 0)
                {
                    _status = "offline";
                }
                else
                {
                    return ToJson(root);
                }
            }
            return null;
        }

        public string ChangeStatus(string newStatus)
        {
            if (_status == "offline" && newStatus != "offline")
            {
                this.LogQQ2(newStatus);
            }
            if (_ptwebqq == null || _ptwebqq.Length == 0)
            {
                return "尚未全局登录成功";
            }
            string url = string.Format(qq_change_status2, newStatus, _clientid, _psessionid, GetTime()); ;
            string retstr = GetUrlText(url);
            if (retstr != null && retstr.Length > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Dictionary<string, object> root = (Dictionary<string, object>)jss.Deserialize(retstr, typeof(object));
                if (root["retcode"] as int? == 0)
                {
                    _status = newStatus;
                }
                else
                {
                    _status = "offline";
                    return ToJson(root);
                }
            }
            return null;
        }

        public Image GetUserHead()
        {
            try
            {
                string url = string.Format(qq_getface_user, _qqnum, _vfwebqq, GetTime());
                Stream result = GetUrlStream(url);
                return new Bitmap(result);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetFriendQQNum(QQUser_Friend friend)
        {
            try
            {
                string url = string.Format(qq_get_friend_uin2, friend.uin, _vfwebqq, GetTime());
                string retstr = GetUrlText(url);
                if (retstr != null && retstr.Length > 0)
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    Dictionary<string, object> root = (Dictionary<string, object>)jss.Deserialize(retstr, typeof(object));
                    if (root["retcode"] as int? == 0)
                    {
                        Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                        try
                        {
                            long qqacc = Convert.ToInt64(result["account"]);
                            friend.num = qqacc;
                            if (MessageFriendReceived != null)
                            {
                                MessageFriendReceived(this, new FriendEventArgs(friend, 0, DateTime.Now, MessageEventType.MESSAGE_USER));
                            }
                            return qqacc.ToString();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        return ToJson(root);
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void StartGetMessage()
        {
            if (_messageTask != null)
            {
                try
                {
                    _messageTaskCts.Cancel(false);
                }
                catch (Exception)
                {
                }
                _messageTask = null;
            }
            if(_messageTaskCts == null)
            {
                _messageTaskCts = new CancellationTokenSource();
            }
            _messageTask = new Task(GetMessage, _messageTaskCts.Token);
            _messageTask.Start();
        }

        public void GetMessage()
        {
            while (_status != "offline")
            {
                GetMessageSub();
            }
        }

        public void GetMessageSub()
        {
            string url = qq_poll2;
            qq_poll2_post["clientid"] = _clientid;
            qq_poll2_post["psessionid"] = _psessionid;
            string para = ToPostData(qq_poll2_post) + string.Format("&clientid={0}&psessionid={1}", _clientid, _psessionid);
            string str = PostUrlText(url, Encoding.Default.GetBytes(para));
            try
            {
                Dictionary<string, object> root = FromJson<Dictionary<string, object>>(str) as Dictionary<string, object>;
                if (root["retcode"] as int? == 0)
                {
                    System.Collections.ArrayList list = root["result"] as System.Collections.ArrayList;
                    foreach (Dictionary<string, object> ele in list)
                    {
                        StringBuilder sb = new StringBuilder();
                        string poll_type = ele["poll_type"] as string;
                        Dictionary<string, object> messagevalue = ele["value"] as Dictionary<string, object>;
                        //sb.AppendFormat("poll_type:{0}\r\n", poll_type);
                        switch (poll_type)
                        {
                            case "buddies_status_change":
                                {
                                    //{"retcode":0,"result":[{"poll_type":"buddies_status_change","value":{"uin":15130679,"status":"online","client_type":1}}]}

                                    string status = messagevalue["status"].ToString();
                                    QQUser_Friend friend = GetUserFriend(messagevalue["uin"].ToString());
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
                                    //{"retcode":0,"result":[{"poll_type":"message","value":
                                    //{"msg_id":17964,"from_uin":1550833875,"to_uin":841473232,"msg_id2":802927,"msg_type":9,"reply_ip":178847978,"time":1336723235,"content":[["font",
                                    //{"size":12,"color":"000080","style":[0,0,0],"name":"\u534E\u6587\u5B8B\u4F53"}],"\u6B66\u5A01  "]}}]}
                                    if (MessageFriendReceived != null)
                                    {
                                        QQUser_Friend friend = GetUserFriend(messagevalue["from_uin"].ToString());
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        MessageFriendReceived(this, new FriendEventArgs(friend, msgid, ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_COMMON, msgs));
                                    }
                                }
                                break;

                            case "shake_message":
                                {
                                    //{"retcode":0,"result":[{"poll_type":"shake_message","value":
                                    //{"msg_id":26413,"from_uin":2867991890,"to_uin":374491485,"msg_id2":980317,
                                    //"msg_type":9,"reply_ip":178849323}}]}
                                    if (MessageFriendReceived != null)
                                    {
                                        QQUser_Friend friend = GetUserFriend(messagevalue["from_uin"].ToString());
                                        MessageFriendReceived(this, new FriendEventArgs(friend, 0, DateTime.Now, MessageEventType.MESSAGE_SHAKE));
                                    }
                                }
                                break;
                            case "group_message":
                                {
                                    //{"retcode":0,"result":[{"poll_type":"group_message","value":
                                    //{"msg_id":13487,"from_uin":511623037,"to_uin":841473232,"msg_id2":839472,"msg_type":43,"reply_ip":176881873,"group_code":3407613792,
                                    //"send_uin":455227435,"seq":104046,"time":1336781087,"info_seq":69054338,
                                    //"content":[["font",{"size":17,"color":"ff0000","style":[1,1,0],"name":"仿宋_GB2312"}],["face",5]," "]}}]}
                                    if (MessageGroupReceived != null)
                                    {
                                        QQUser_Group group = GetUserGroup(messagevalue["from_uin"].ToString());
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        QQUser_Friend friend = GetUserFriend(messagevalue["send_uin"].ToString());
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        MessageGroupReceived(this, new GroupEventArgs(group, friend, msgid, ToTime(Convert.ToInt64(messagevalue["time"])), msgs));
                                    }
                                }
                                break;
                            case "file_message":
                                if (MessageFriendReceived != null)
                                {
                                    QQUser_Friend friend = GetUserFriend(messagevalue["from_uin"].ToString());
                                    Dictionary<string, object> msgs = new Dictionary<string, object>();
                                    msgs.Add("poll_type", poll_type);
                                    foreach (string key in messagevalue.Keys)
                                    {
                                        msgs.Add(key, messagevalue[key]);
                                    }
                                    MessageFriendReceived(this, new FriendEventArgs(friend, 0, ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_FILE, msgs));
                                }
                                break;
                            case "push_offfile":
                                if (MessageFriendReceived != null)
                                {
                                    QQUser_Friend friend = GetUserFriend(messagevalue["from_uin"].ToString());
                                    Dictionary<string, object> msgs = new Dictionary<string, object>();
                                    msgs.Add("poll_type", poll_type);
                                    foreach (string key in messagevalue.Keys)
                                    {
                                        msgs.Add(key, messagevalue[key]);
                                    }
                                    MessageFriendReceived(this, new FriendEventArgs(friend, 0, ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_OFFLINE, msgs));
                                }
                                break;
                            case "kick_message":
                                {
                                    _status = "offline";
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>();
                                        msgs.Add("poll_type", poll_type);
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
                                        QQUser_Friend friend = GetUserFriend(messagevalue["from_uin"].ToString());
                                        Dictionary<string, object> msgs = new Dictionary<string, object>();
                                        msgs.Add("poll_type", poll_type);
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(friend, 0, DateTime.Now, MessageEventType.MESSAGE_INPUT, msgs));
                                    }
                                }
                                break;
                            case "tips":
                            case "system_message":
                                {
                                    if (MessageFriendReceived != null)
                                    {
                                        Dictionary<string, object> msgs = new Dictionary<string, object>();
                                        msgs.Add("poll_type", poll_type);
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
                                        Dictionary<string, object> msgs = new Dictionary<string, object>();
                                        msgs.Add("poll_type", poll_type);
                                        foreach (string key in messagevalue.Keys)
                                        {
                                            msgs.Add(key, messagevalue[key]);
                                        }
                                        MessageFriendReceived(this, new FriendEventArgs(null, 0, DateTime.Now, MessageEventType.MESSAGE_UNKNOW, msgs));
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public string RefuseFileURL(string uin, string ssid)
        {
            return string.Format(qq_refuse_file2, uin, ssid, _clientid, _psessionid, GetTime());
        }
        public string GetFileURL(string ssid, string name, string uin)
        {
            name = HttpUtility.UrlEncode(name, Encoding.GetEncoding("GBK"));
            return string.Format(qq_get_file2, ssid, name, uin, _psessionid, GetTime(), _clientid);
        }

        public string RefuleOfffileURL(string uin, string name, string size)
        {
            name = HttpUtility.UrlEncode(name, Encoding.GetEncoding("GBK"));
            return string.Format(qq_notify_offfile2, uin, name, size, _psessionid, _clientid, GetTime());
        }

        public string GetOfffileURL(string ip, string port, string name, string rkey)
        {
            name = HttpUtility.UrlEncode(name, Encoding.GetEncoding("GBK"));
            return string.Format(qq_get_offfile2, ip, port, name, rkey);
        }

        private System.Threading.ManualResetEvent _event = new System.Threading.ManualResetEvent(true);

        public bool SendFriendMessage(QQUser_Friend friend, string msg)
        {
            if (msg == null || msg.Length == 0)
            {
                return false;
            }
            if (_event.WaitOne(5000) == false)
            {
                return false;
            }
            _event.Reset();
            string url = qq_send_buddy_msg2;
            qq_send_buddy_msg2_post["to"] = friend.uin;
            qq_send_buddy_msg2_post["clientid"] = _clientid;
            qq_send_buddy_msg2_post["psessionid"] = _psessionid;
            qq_send_buddy_msg2_post["msg_id"] = qq_send_buddy_msg2_post_msg_id;
            qq_send_buddy_msg2_post_msg_id++;
            qq_send_buddy_msg2_post_content[0] = msg;
            qq_send_buddy_msg2_post["content"] = ToJson(qq_send_buddy_msg2_post_content);
            string para = ToPostData(qq_send_buddy_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _clientid, _psessionid);
            string retstr = PostUrlText(url, Encoding.Default.GetBytes(para));
            bool ret = false;
            if (retstr != null && retstr.Length > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Dictionary<string, object> root = (Dictionary<string, object>)jss.Deserialize(retstr, typeof(object));
                if (root["retcode"] as int? == 0)
                {
                    ret = true;
                }
            }
            _event.Set();
            return ret;
        }

        public bool SendGroupMessage(QQUser_Group group, string msg)
        {
            if (msg == null || msg.Length == 0)
            {
                return false;
            }
            if (_event.WaitOne(5000) == false)
            {
                return false;
            }
            _event.Reset();
            string url = qq_send_qun_msg2;
            qq_send_qun_msg2_post["group_uin"] = group.gid;
            qq_send_qun_msg2_post["clientid"] = _clientid;
            qq_send_qun_msg2_post["psessionid"] = _psessionid;
            qq_send_buddy_msg2_post["msg_id"] = qq_send_buddy_msg2_post_msg_id;
            qq_send_buddy_msg2_post_msg_id++;
            qq_send_qun_msg2_post_content[0] = msg;
            qq_send_qun_msg2_post["content"] = ToJson(qq_send_qun_msg2_post_content);
            string para = ToPostData(qq_send_qun_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _clientid, _psessionid);
            string retstr = PostUrlText(url, Encoding.Default.GetBytes(para));
            bool ret = false;
            if (retstr != null && retstr.Length > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Dictionary<string, object> root = (Dictionary<string, object>)jss.Deserialize(retstr, typeof(object));
                if (root["retcode"] as int? == 0)
                {
                    ret = true;
                }
            }
            _event.Set();
            return ret;
        }

        public QQUserFriends RefreshFriendList()
        {
            try
            {
                string url = qq_get_user_friends2;
                qq_get_user_friends2_post["h"] = "hello";
                qq_get_user_friends2_post["vfwebqq"] = _vfwebqq;
                string para = ToPostData(qq_get_user_friends2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                Dictionary<string, object> root = FromJson<Dictionary<string, object>>(resultStr) as Dictionary<string, object>;
                if (root != null && root["retcode"] as int? == 0)
                {
                    _friends.Clear();
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    foreach (Dictionary<string, object> item in result["categories"] as ArrayList)
                    {
                        _friends.Add(new QQUser_FriendGroup()
                        {
                            index = Convert.ToInt64(item["index"]),
                            sort = Convert.ToInt64(item["sort"]),
                            name = item["name"].ToString(),
                        });
                    }
                    QQUser_FriendGroup nofriendgroup = new QQUser_FriendGroup()
                    {
                        index = -1,
                        sort = -1,
                        name = "[已删除]",
                    };
                    _friends.Add(nofriendgroup);
                    foreach (Dictionary<string, object> item in result["friends"] as ArrayList)
                    {
                        _friends.Add(new QQUser_Friend()
                        {
                            uin = Convert.ToInt64(item["uin"]),
                            flag_friends = Convert.ToInt64(item["flag"]),
                            categories = Convert.ToInt64(item["categories"]),
                        });
                    }
                    foreach (Dictionary<string, object> item in result["info"] as ArrayList)
                    {
                        QQUser_Friend user = _friends.Friends[item["uin"].ToString()];
                        if (user != null)
                        {
                            user.face = Convert.ToInt64(item["face"]);
                            user.flag_info = Convert.ToInt64(item["flag"]);
                            user.nick = item["nick"].ToString();
                        }
                    }
                    foreach (Dictionary<string, object> item in result["vipinfo"] as ArrayList)
                    {
                        QQUser_Friend user = _friends.Friends[item["u"].ToString()];
                        if (user != null)
                        {
                            user.is_vip = Convert.ToInt64(item["is_vip"]);
                            user.vip_level = Convert.ToInt64(item["vip_level"]);
                        }
                    }
                    foreach (Dictionary<string, object> item in result["marknames"] as ArrayList)
                    {
                        QQUser_Friend user = _friends.Friends.SingleOrDefault(ele => ele.Key == item["uin"].ToString()).Value;
                        if (user != null)
                        {
                            user.markname = item["markname"].ToString();
                        }
                        else
                        {
                            _friends.Friends.Add(item["uin"].ToString(), new QQUser_Friend()
                            {
                                uin = Convert.ToInt64(item["uin"]),
                                markname = item["markname"].ToString(),
                                categories = nofriendgroup.index
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return _friends;
        }

        public QQUserFriends GetOnlineUsers()
        {
            try
            {
                string url = string.Format(qq_get_online_buddies2, _clientid, _psessionid, GetTime());
                string resultStr = GetUrlText(url);
                Dictionary<string, object> root = FromJson<Dictionary<string, object>>(resultStr) as Dictionary<string, object>;
                if (root != null && root["retcode"] as int? == 0)
                {
                    foreach (Dictionary<string, object> item in root["result"] as ArrayList)
                    {
                        QQUser_Friend user = _friends.Friends.SingleOrDefault(ele => ele.Key == item["uin"].ToString()).Value;
                        if (user != null)
                        {
                            user.status = item["status"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return _friends;
        }

        public QQUserGroups RefreshGroupList()
        {
            try
            {
                string url = qq_get_group_name_list_mask2;
                qq_get_group_name_list_mask2_post["vfwebqq"] = _vfwebqq;
                string para = ToPostData(qq_get_user_friends2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                Dictionary<string, object> root = FromJson<Dictionary<string, object>>(resultStr) as Dictionary<string, object>;
                if (root != null && root["retcode"] as int? == 0)
                {
                    _groups.Clear();
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    foreach (Dictionary<string, object> item in result["gnamelist"] as ArrayList)
                    {
                        _groups.Add(new QQUser_Group()
                        {
                            flag = Convert.ToInt64(item["flag"]),
                            gid = Convert.ToInt64(item["gid"]),
                            code = Convert.ToInt64(item["code"]),
                            name = item["name"].ToString(),
                        });
                    }
                }
            }
            catch (Exception)
            {
            }
            return _groups;
        }

        #endregion

        #region geturl

        public static Dictionary<string, int> T_TRANSFER_TABLE = QQUser.FromJson<Dictionary<string, int>>("{14:0,1:1,2:2,3:3,4:4,5:5,6:6,7:7,8:8,9:9,10:10,11:11,12:12,13:13,0:14,50:15,51:16,96:17,53:18,54:19,73:20,74:21,75:22,76:23,77:24,78:25,55:26,56:27,57:28,58:29,79:30,80:31,81:32,82:33,83:34,84:35,85:36,86:37,87:38,88:39,97:40,98:41,99:42,100:43,101:44,102:45,103:46,104:47,105:48,106:49,107:50,108:51,109:52,110:53,111:54,112:55,32:56,113:57,114:58,115:59,63:60,64:61,59:62,33:63,34:64,116:65,36:66,37:67,38:68,91:69,92:70,93:71,29:72,117:73,72:74,45:75,42:76,39:77,62:78,46:79,47:80,71:81,95:82,118:83,119:84,120:85,121:86,122:87,123:88,124:89,27:90,21:91,23:92,25:93,26:94,125:95,126:96,127:97,128:98,129:99,130:100,131:101,132:102,133:103,134:104,52:105,24:106,22:107,20:108,60:109,61:110,89:111,90:112,31:113,94:114,65:115,35:116,66:117,67:118,68:119,69:120,70:121,15:122,16:123,17:124,18:125,19:126,28:127,30:128,40:129,41:130,43:131,44:132,48:133,49:134}");

        public string GetFaceUrl(object face)
        {
            int id = 0;
            try
            {
                id = T_TRANSFER_TABLE[face.ToString()];
            }
            catch
            {
                return face.ToString();
            }
            return GetTrueFaceUrl(id);
        }

        public string GetTrueFaceUrl(int id)
        {
            return string.Format(qq_face, id);
        }

        public string GetCFaceUrl(int msgid, int guid, int to)
        {
            string url = string.Format(qq_cface, msgid, guid, to, _clientid, _psessionid);
            return GetFileTrueUrl(url);
        }

        public Stream GetOffPic(string filepath, string fuin)
        {
            //"http://d.web2.qq.com/channel/get_offpic2?file_path={0}&f_uin={1}&clientid={2}&psessionid={3}";
            string fileurl = string.Format(qq_get_offpic2, filepath, fuin, _clientid, _psessionid);
            return GetUrlStream(fileurl);
        }

        public string GetFileTrueUrl(string url)
        {

            try
            {
                HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                myRequest.Method = "GET";
                myRequest.Referer = qq_referurl;
                myRequest.CookieContainer = _cookiecontainer;
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                myRequest.AllowAutoRedirect = false;
                myRequest.KeepAlive = true;
                myRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                myRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                myRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                myRequest.Headers.Add("Accept-Charset", "GBK,utf-8;q=0.7,*;q=0.3");
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();//1
                WebHeaderCollection headers = myResponse.Headers;
                String newUrl = url;
                if ((myResponse.StatusCode == System.Net.HttpStatusCode.Found) ||
                  (myResponse.StatusCode == System.Net.HttpStatusCode.Redirect) ||
                  (myResponse.StatusCode == System.Net.HttpStatusCode.Moved) ||
                  (myResponse.StatusCode == System.Net.HttpStatusCode.MovedPermanently))
                {
                    newUrl = headers["Location"];
                    newUrl = newUrl.Trim();
                }
                myResponse.Close();
                return newUrl;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetUrlText(string url)
        {
            HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            myRequest.Method = "GET";
            myRequest.Referer = qq_referurl;
            myRequest.CookieContainer = _cookiecontainer;
            myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";

            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                Stream newStream = myResponse.GetResponseStream();
                if (newStream != null)
                {
                    StreamReader reader = new StreamReader(newStream, Encoding.GetEncoding(myResponse.CharacterSet));
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public Stream GetUrlStream(string url)
        {
            HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            myRequest.Method = "GET";
            myRequest.Referer = qq_referurl;
            myRequest.CookieContainer = _cookiecontainer;
            myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
            myRequest.AllowAutoRedirect = true;
            myRequest.KeepAlive = true;

            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                Stream newStream = myResponse.GetResponseStream();
                return newStream;
            }
            catch (Exception)
            {
                return null;
            }
        }



        public string PostUrlText(string url, byte[] postData)
        {
            HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            myRequest.Method = "POST";
            myRequest.Referer = qq_referurl;
            myRequest.CookieContainer = _cookiecontainer;
            myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
            myRequest.AllowAutoRedirect = true;
            myRequest.KeepAlive = true;

            myRequest.ContentLength = postData.Length;
            try
            {
                using (var sw = myRequest.GetRequestStream())
                {
                    sw.Write(postData, 0, postData.Length);
                }

                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                Stream newStream = myResponse.GetResponseStream();
                if (newStream != null)
                {
                    StreamReader reader = new StreamReader(newStream, Encoding.GetEncoding(myResponse.CharacterSet));
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public Stream PostUrlStream(string url, byte[] postData)
        {
            HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            myRequest.Method = "POST";
            myRequest.Referer = qq_referurl;
            myRequest.CookieContainer = _cookiecontainer;
            myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
            myRequest.ContentLength = postData.Length;

            try
            {
                using (var sw = myRequest.GetRequestStream())
                {
                    sw.Write(postData, 0, postData.Length);
                }
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                Stream newStream = myResponse.GetResponseStream();
                return newStream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region other method
        public QQUser_Friend GetUserFriend(string uin)
        {
            var i = _friends.Friends.SingleOrDefault(ele => ele.Key == uin);
            if (i.Value == null)
            {
                try
                {
                    return new QQUser_Friend() { uin = Convert.ToInt64(uin) };
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return i.Value;
        }

        public QQUser_Group GetUserGroup(string gid)
        {
            return _groups.Groups.SingleOrDefault(ele => ele.Key == gid).Value;
        }

        #endregion

        #region static method

        private static string GenerateClientID()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(0, 99) + "" + GetTime() / 1000000;
        }

        private static long GetTime()
        {
            DateTime startDate = new DateTime(1970, 1, 1);
            DateTime endDate = DateTime.Now.ToUniversalTime();
            TimeSpan span = endDate - startDate;
            return (long)(span.TotalMilliseconds + 0.5);
        }

        private static DateTime ToTime(long time)
        {
            DateTime dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            dtZone = dtZone.AddSeconds(time);
            return dtZone.ToLocalTime();
        }

        private static string getPassword(string uin, string password, string verficode)
        {
            string rpass = HEXMD5.hexchar2bin(HEXMD5.md5(password));
            rpass = HEXMD5.md5(rpass + uin);
            rpass = HEXMD5.md5(rpass + verficode.ToUpper());
            return rpass;
        }

        private static string getPassword(string password, string verifycode)
        {
            return md5(md5_3(password) + verifycode.ToUpper());
        }

        private static string md5_3(string input)
        {
            MD5 md = MD5.Create();
            byte[] buffer = md.ComputeHash(Encoding.UTF8.GetBytes(input));
            buffer = md.ComputeHash(buffer);
            buffer = md.ComputeHash(buffer);
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        private static string md5(string input)
        {
            byte[] buffer = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static TEntry FromJson<TEntry>(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return (TEntry)jss.Deserialize(input, typeof(TEntry));
        }

        public static object FromJson(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize(input, typeof(object));
        }

        public static string ToJson(object input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(input);
        }

        private static string ToPostData(object input)
        {
            string str = ToJson(input);
            return "r=" + HttpUtility.UrlEncode(str);
        }

        #endregion

    }
}
