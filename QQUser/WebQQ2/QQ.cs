using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebQQ2.Extends;

namespace WebQQ2.WebQQ2
{
    public class QQ
    {

        #region urldefine

        private static readonly string qq_referurl = "http://d.web2.qq.com/proxy.html";
        private static readonly string qq_check = "http://check.ptlogin2.qq.com/check?uin={0}&appid=1003903&r={1:f16}";
        private static readonly string qq_getimage = "http://captcha.qq.com/getimage?aid=1003903&r={0:f16}&uin={1}";
        private static readonly string qq_login = "http://ptlogin2.qq.com/login?u={0}&p={1}&verifycode={2}&webqq_type=10&remember_uin=1&login2qq=1&aid=1003903&u1=http%3A%2F%2Fw.qq.com%2Floginproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=3-6-9305&mibao_css=m_webqq&t=1&g=1";
        private static readonly string qq_login2 = "http://d.web2.qq.com/channel/login2";
        private static readonly string qq_get_user_friends2 = "http://s.web2.qq.com/api/get_user_friends2";
        private static readonly string qq_get_friend_info2 = "http://s.web2.qq.com/api/get_friend_info2?tuin={0}&verifysession=&code=&vfwebqq={1}&t={2}";
        private static readonly string qq_getface_user = "http://face3.qun.qq.com/cgi/svr/face/getface?cache=1&type=1&fid=0&uin={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_getface_qun = "http://face1.qun.qq.com/cgi/svr/face/getface?cache=0&type=4&fid=0&uin={0}&vfwebqq={1}";//uin = code
        private static readonly string qq_get_group_name_list_mask2 = "http://s.web2.qq.com/api/get_group_name_list_mask2";
        private static readonly string qq_get_group_info = "http://s.web2.qq.com/api/get_group_info?gcode=%5B{0}%5D&retainKey=memo&vfwebqq={1}&t={2}";
        private static readonly string qq_get_group_info_ext2 = "http://s.web2.qq.com/api/get_group_info_ext2?gcode={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_send_qun_msg2 = "http://d.web2.qq.com/channel/send_qun_msg2";
        private static readonly string qq_get_online_buddies2 = "http://d.web2.qq.com/channel/get_online_buddies2?clientid={0}&psessionid={1}&t={2}";
        private static readonly string qq_get_single_long_nick2 = "http://s.web2.qq.com/api/get_single_long_nick2?tuin={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_set_long_nick2 = "http://s.web2.qq.com/api/set_long_nick2";
        private static readonly string qq_get_msg_tip = "http://web.qq.com/web2/get_msg_tip?uin=&tp=1&id=0&retype=1&rc=18&lv=3&t={0}";
        private static readonly string qq_get_discu_list_new2 = "http://d.web2.qq.com/channel/get_discu_list_new2?clientid={0}&psessionid={1}&vfwebqq={2}&t={3}";
        private static readonly string qq_get_recent_list2 = "http://d.web2.qq.com/channel/get_recent_list2";
        private static readonly string qq_poll2 = "http://d.web2.qq.com/channel/poll2";
        private static readonly string qq_send_buddy_msg2 = "http://d.web2.qq.com/channel/send_buddy_msg2";
        private static readonly string qq_get_qq_level2 = "http://s.web2.qq.com/api/get_qq_level2?tuin={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_get_friend_uin2 = "http://s.web2.qq.com/api/get_friend_uin2?tuin={0}&verifysession=&type=1&code=&vfwebqq={1}&t={2}";
        private static readonly string qq_change_status2 = "http://d.web2.qq.com/channel/change_status2?newstatus={0}&clientid={1}&psessionid={2}&t={3}";
        private static readonly string qq_logout2 = "http://d.web2.qq.com/channel/logout2?ids=&clientid={0}&psessionid={1}&t={2}";
        private static readonly string qq_search_qq_by_uin2 = "http://s.web2.qq.com/api/search_qq_by_uin2?tuin={0}&verifysession={1}&code={2}&vfwebqq={3}&t={4}";
        private static readonly string qq_get_c2cmsg_sig2 = "http://d.web2.qq.com/channel/get_c2cmsg_sig2?id=3195852032&to_uin={0}&service_type=0&clientid={1}&psessionid={2}&t={3}";
        private static readonly string qq_get_stranger_info2 = "http://s.web2.qq.com/api/get_stranger_info2?tuin={0}&verifysession=&gid=0&code=&vfwebqq={1}&t={2}";

        private static readonly string qq_refuse_file2 = "http://d.web2.qq.com/channel/refuse_file2?to={0}&lcid={1}&clientid={2}&psessionid={3}&t={4}";
        private static readonly string qq_get_file2 = "http://d.web2.qq.com/channel/get_file2?lcid={0}&guid={1}&to={2}&psessionid={3}&count=1&time={4}&clientid={5}";
        private static readonly string qq_notify_offfile2 = "http://d.web2.qq.com/channel/notify_offfile2?to={0}&file_name={1}&file_size={2}&action=2&psessionid={3}&clientid={4}&t={5}";
        private static readonly string qq_get_offfile2 = "http://{0}:{1}/{2}?ver=2173&rkey={3}&range=0";
        private static readonly string qq_get_offpic2 = "http://d.web2.qq.com/channel/get_offpic2?file_path={0}&f_uin={1}&clientid={2}&psessionid={3}";

        private static readonly string qq_face = "http://0.web.qstatic.com/webqqpic/style/face/{0}.gif";

        private static readonly string qq_cface = "http://d.web2.qq.com/channel/get_cface2?lcid={0}&guid={1}&to={2}&count=5&time=1&clientid={3}&psessionid={4}";

        private static readonly string qq_deny_added_request2 = "http://s.web2.qq.com/api/deny_added_request2";
        private static readonly string qq_allow_added_request2 = "http://s.web2.qq.com/api/allow_added_request2";
        private static readonly string qq_allow_and_add2 = "http://s.web2.qq.com/api/allow_and_add2";


        #endregion

        #region postparadefine

        private Dictionary<string, object> qq_login2_post = new Dictionary<string, object>()
        {
           {"status",""},
           {"ptwebqq",""},
           {"passwd_sig",""},
           {"clientid",""},
        };

        private Dictionary<string, object> qq_get_user_friends2_post = new Dictionary<string, object>()
        {
            {"h",""},
            {"vfwebqq",""},
        };
        private Dictionary<string, object> qq_get_group_name_list_mask2_post = new Dictionary<string, object>()
        {
            {"vfwebqq",""},
        };


        private Dictionary<string, object> qq_send_buddy_msg2_post = new Dictionary<string, object>()
        {
            {"to",0},
            {"face",0},
            {"content",""},
            {"msg_id",56410001},
            {"clientid",""},
            {"psessionid",""}
        };

        private ArrayList qq_send_buddy_msg2_post_content = new ArrayList(){
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
        private Dictionary<string, object> qq_send_qun_msg2_post = new Dictionary<string, object>(){
            {"group_uin",0},
            {"content",""},
            {"msg_id",49800001}, 
            {"clientid",""},
            {"psessionid",""}
         };

        private ArrayList qq_send_qun_msg2_post_content = new ArrayList(){
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

        private Dictionary<string, object> qq_poll2_post = new Dictionary<string, object>()
        {
            {"clientid",""},
            {"psessionid",""},
            {"key",""},
            {"ids",new ArrayList()},
        };
        private Dictionary<string, object> qq_get_recent_list2_post = new Dictionary<string, object>()
        {
            {"vfwebqq",""},
            {"clientid",""},
            {"psessionid",""},
        };
        private Dictionary<string, object> qq_set_long_nick2_post = new Dictionary<string, object>()
        {
            {"nlk",""},
            {"vfwebqq",""}
        };
        private Dictionary<string, object> qq_deny_added_request2_post = new Dictionary<string, object>()
        {
            {"account",0},
            {"msg",""},
            {"vfwebqq",""}
        };

        private Dictionary<string, object> qq_allow_added_request2_post = new Dictionary<string, object>()
        {
            {"account",0},
            {"vfwebqq",""}
        };

        private Dictionary<string, object> qq_allow_and_add2_post = new Dictionary<string, object>()
        {
            {"account",0},
            {"gid",0},
            {"mname",""},
            {"vfwebqq",""}
        };

        #endregion

        #region event

        public event EventHandler<FriendEventArgs> MessageFriendReceived;
        public event EventHandler<GroupEventArgs> MessageGroupReceived;

        #endregion

        #region paradefine

        private Task _messageTask;
        private CancellationTokenSource _messageTaskCts;
        private int _qq_send_buddy_msg2_post_msg_id = 0;
        private Random _random;
        private CookieContainer _cookiecontainer;
        private QQUser _user;

        public QQUser User
        {
            get { return _user; }
        }

        #endregion

        #region constructor

        private void OnCreated()
        {
            _random = new Random();
            _cookiecontainer = new CookieContainer();
            _qq_send_buddy_msg2_post_msg_id = (_random.Next(3000) + 4000) * 10000 + 1;
        }

        public QQ()
        {
            _user = new QQUser();
            OnCreated();

        }

        public QQ(string qqnum)
        {
            _user = new QQUser(qqnum);
            OnCreated();
        }

        #endregion


        #region geturl

        public static Dictionary<string, int> T_TRANSFER_TABLE = QQHelper.FromJson<Dictionary<string, int>>("{14:0,1:1,2:2,3:3,4:4,5:5,6:6,7:7,8:8,9:9,10:10,11:11,12:12,13:13,0:14,50:15,51:16,96:17,53:18,54:19,73:20,74:21,75:22,76:23,77:24,78:25,55:26,56:27,57:28,58:29,79:30,80:31,81:32,82:33,83:34,84:35,85:36,86:37,87:38,88:39,97:40,98:41,99:42,100:43,101:44,102:45,103:46,104:47,105:48,106:49,107:50,108:51,109:52,110:53,111:54,112:55,32:56,113:57,114:58,115:59,63:60,64:61,59:62,33:63,34:64,116:65,36:66,37:67,38:68,91:69,92:70,93:71,29:72,117:73,72:74,45:75,42:76,39:77,62:78,46:79,47:80,71:81,95:82,118:83,119:84,120:85,121:86,122:87,123:88,124:89,27:90,21:91,23:92,25:93,26:94,125:95,126:96,127:97,128:98,129:99,130:100,131:101,132:102,133:103,134:104,52:105,24:106,22:107,20:108,60:109,61:110,89:111,90:112,31:113,94:114,65:115,35:116,66:117,67:118,68:119,69:120,70:121,15:122,16:123,17:124,18:125,19:126,28:127,30:128,40:129,41:130,43:131,44:132,48:133,49:134}");

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
            string url = string.Format(qq_cface, msgid, guid, to, _user.ClientID, _user.PsessionID); ;
            return GetFileTrueUrl(url);
        }

        public Stream GetOffPic(string filepath, string fuin)
        {
            string fileurl = string.Format(qq_get_offpic2, filepath, fuin, _user.ClientID, _user.PsessionID);
            return GetUrlStream(fileurl);
        }

        #endregion


        #region QQOperation

        public string GetVerifyCode()
        {
            string result = GetUrlText(
                string.Format(qq_check, _user.QQNum, _random.NextDouble())
                );
            string ptui_checkVCStart = "ptui_checkVC(";
            string ptui_checkVCEnd = ");";
            _user.Uin = "";
            try
            {
                if (result.StartsWith(ptui_checkVCStart) && result.EndsWith(ptui_checkVCEnd))
                {
                    result = result.Substring(ptui_checkVCStart.Length);
                    result = result.Substring(0, result.Length - 2);
                    string[] subresult = result.Split(',');
                    if (subresult.Length == 3)
                    {
                        string uin = subresult[2].Trim().Trim('\'');
                        string[] newstr = uin.Split(new string[] { "\\x" }, StringSplitOptions.RemoveEmptyEntries);
                        uin = "";
                        foreach (string str in newstr)
                        {
                            char c = (char)Convert.ToByte(str, 16);
                            uin += c;
                        }
                        _user.Uin = uin;
                        return subresult[1].Trim().Trim('\'');
                    }
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public Image GetVerifyImage()
        {
            return new Bitmap(
                GetUrlStream(
                    string.Format(qq_getimage, _random.NextDouble(), _user.QQNum)
                    )
                );
        }

        public string LoginQQ(string password, string vercode)
        {
            string mpass = QQHelper.GetPassword(_user.Uin, password, vercode);
            string url = string.Format(qq_login, _user.QQNum, mpass, vercode);
            string result = GetUrlText(url);
            _user.PtWebQQ = null;
            foreach (Cookie v in _cookiecontainer.GetCookies(new Uri(url)))
            {
                if (string.Compare(v.Name, "ptwebqq") == 0)
                {
                    _user.PtWebQQ = v.Value;
                    break;
                }
            }
            if (result.StartsWith("ptuiCB"))
            {
                int start = result.IndexOf('(');
                string sub = result.Substring(start + 1, result.LastIndexOf(')') - start - 1);
                string[] sublist = sub.Split(',');
                _user.QQName = sublist[5].Trim().Trim('\'');
                result = _user.QQName + ":" + sublist[4].Trim().Trim('\'');
            }
            return result;
        }

        public string LoginQQ2(string logStatus)
        {
            if (_user.PtWebQQ == null || _user.PtWebQQ.Length == 0)
            {
                return "尚未全局登录成功";
            }
            string url = qq_login2;
            qq_login2_post["status"] = logStatus;
            qq_login2_post["ptwebqq"] = _user.PtWebQQ;
            qq_login2_post["clientid"] = _user.ClientID;
            string para = QQHelper.ToPostData(qq_login2_post);
            string retstr = PostUrlText(url, Encoding.Default.GetBytes(para));
            _user.VfWebQQ = null;
            _user.PsessionID = null;
            _user.Status = "offline";
            if (retstr != null && retstr.Length > 0)
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                if (root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    _user.VfWebQQ = result["vfwebqq"] as string;
                    _user.PsessionID = result["psessionid"] as string;
                    _user.Status = result["status"] as string;
                    if (_user.Status != "offline")
                    {
                        _user.LoginTime = DateTime.Now;
                    }
                }
                else
                {
                    return QQHelper.ToJson(root);
                }
            }
            return null;
        }


        public string LogOutQQ2()
        {
            if (_user.PtWebQQ == null || _user.PtWebQQ.Length == 0)
            {
                return "尚未全局登录成功";
            }
            string url = string.Format(qq_logout2, _user.ClientID, _user.PsessionID, QQHelper.GetTime()); ;
            string retstr = GetUrlText(url);
            if (retstr != null && retstr.Length > 0)
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                if (root["retcode"] as int? == 0)
                {
                    _user.Status = "offline";
                }
                else
                {
                    return QQHelper.ToJson(root);
                }
            }
            return null;
        }

        public string ChangeStatus(string newStatus)
        {
            if (_user.Status == "offline" && newStatus != "offline")
            {
                this.LoginQQ2(newStatus);
            }
            if (_user.PtWebQQ == null || _user.PtWebQQ.Length == 0)
            {
                return "尚未全局登录成功";
            }
            string url = string.Format(qq_change_status2, newStatus, _user.ClientID, _user.PsessionID, QQHelper.GetTime()); ;
            string retstr = GetUrlText(url);
            if (retstr != null && retstr.Length > 0)
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                if (root["retcode"] as int? == 0)
                {
                    _user.Status = newStatus;
                }
                else
                {
                    _user.Status = "offline";
                    return QQHelper.ToJson(root);
                }
            }
            return null;
        }

        public Image GetUserHead()
        {
            try
            {
                string url = string.Format(qq_getface_user, _user.QQNum, _user.VfWebQQ, QQHelper.GetTime());
                Stream result = GetUrlStream(url);
                return new Bitmap(result);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetFriendQQNum(QQFriend friend)
        {
            try
            {
                string url = string.Format(qq_get_friend_uin2, friend.uin, _user.VfWebQQ, QQHelper.GetTime());
                string retstr = GetUrlText(url);
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
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
                        return QQHelper.ToJson(root);
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
            if (_messageTaskCts == null)
            {
                _messageTaskCts = new CancellationTokenSource();
            }
            _messageTask = new Task(() =>
                {
                    _messageTaskCts.Token.ThrowIfCancellationRequested();
                    GetMessage();
                }, _messageTaskCts.Token);
            _messageTask.Start();
        }

        public void GetMessage()
        {
            while (_user.Status != "offline")
            {
                GetMessageSub();
            }
        }

        public void GetMessageSub()
        {
            string url = qq_poll2;
            qq_poll2_post["clientid"] = _user.ClientID;
            qq_poll2_post["psessionid"] = _user.PsessionID;
            string para = QQHelper.ToPostData(qq_poll2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
            string str = PostUrlText(url, Encoding.Default.GetBytes(para), int.MaxValue);
            try
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(str);
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
                                    QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["uin"]));
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
                                        QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]));
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        MessageFriendReceived(this, new FriendEventArgs(friend, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_COMMON, msgs));
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
                                        QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]));
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
                                        QQGroup group = _user.GetUserGroup(Convert.ToInt64(messagevalue["from_uin"]));
                                        System.Collections.ArrayList array = new ArrayList(messagevalue["content"] as System.Collections.ArrayList);
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "content", array } };
                                        QQGroupMember member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                        if (member == null || member.IsValid)
                                        {
                                            new Task(() =>
                                            {
                                                RefreshGroupInfo(group);
                                            }).Start();
                                        }
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), msgs));
                                    }
                                }
                                break;
                            case "group_web_message":
                                {
                                    //{"retcode":0,"result":[{""poll_type":"group_web_message","value":
                                    //{"msg_id":11746,"from_uin":2607650969,"to_uin":344420262,"msg_id2":556231,"msg_type":45,"reply_ip":176752015,"group_code":1876212762,
                                    //"group_type":1,"ver":1,"send_uin":1007665318,
                                    //"xml":"\u003c?xml version=\"1.0\" encoding=\"utf-8\"?\u003e\u003cd\u003e\u003cn t=\"h\" u=\"979270988\" i=\"6\" s=\"1.url.cn/qun/feeds/img/server/g16.png\"/\u003e\u003cn t=\"t\" s=\"\u5728\u7FA4\u52A8\u6001\u4E2D\"/\u003e\u003cn t=\"b\"/\u003e\u003cn t=\"t\" s=\"\u5206\u4EAB1\u5F20\u56FE\u7247\"/\u003e\u003c/d\u003e"}}}]}

                                    if (MessageGroupReceived != null)
                                    {
                                        QQGroup group = _user.GetUserGroup(Convert.ToInt64(messagevalue["from_uin"]));
                                        string xml = messagevalue["xml"] as string;
                                        Dictionary<string, object> msgs = new Dictionary<string, object>() { { "xml", xml } };
                                        QQGroupMember member = group.GetGroupMember(Convert.ToInt64(messagevalue["send_uin"]));
                                        if (member == null || member.IsValid)
                                        {
                                            new Task(() =>
                                            {
                                                RefreshGroupInfo(group);
                                            }).Start();
                                        }
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), msgs));
                                    }
                                }
                                break;
                            case "file_message":
                                if (MessageFriendReceived != null)
                                {
                                    QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]));
                                    Dictionary<string, object> msgs = new Dictionary<string, object>();
                                    msgs.Add("poll_type", poll_type);
                                    foreach (string key in messagevalue.Keys)
                                    {
                                        msgs.Add(key, messagevalue[key]);
                                    }
                                    MessageFriendReceived(this, new FriendEventArgs(friend, 0, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_FILE, msgs));
                                }
                                break;
                            case "push_offfile":
                                if (MessageFriendReceived != null)
                                {
                                    QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]));
                                    Dictionary<string, object> msgs = new Dictionary<string, object>();
                                    msgs.Add("poll_type", poll_type);
                                    foreach (string key in messagevalue.Keys)
                                    {
                                        msgs.Add(key, messagevalue[key]);
                                    }
                                    MessageFriendReceived(this, new FriendEventArgs(friend, 0, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_OFFLINE, msgs));
                                }
                                break;
                            case "kick_message":
                                {
                                    _user.Status = "offline";
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
                                        QQFriend friend = _user.GetUserFriend(Convert.ToInt64(messagevalue["from_uin"]));
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
                            case "sys_g_message":
                                {
                                    //{"retcode":0,"result":[{"poll_type":"sys_g_msg","value":
                                    //{"msg_id":53435,"from_uin":4257273782,"to_uin":2221933016,
                                    //"msg_id2":268651,"msg_type":33,"reply_ip":176498397,
                                    //"type":"group_join","gcode":519756087,"t_gcode":70125956,
                                    //"op_type":3,"new_member":2221933016,"t_new_member":"",
                                    //"admin_uin":1793858042,"admin_nick":"\u521B\u5EFA\u8005"}}]}
                                    //{"retcode":0,"result":[{"poll_type":"sys_g_msg","value":
                                    //{"msg_id":803,"from_uin":4257273782,"to_uin":2221933016,
                                    //"msg_id2":794855,"msg_type":34,"reply_ip":176722703,
                                    //"type":"group_leave","gcode":519756087,"t_gcode":70125956,
                                    //"op_type":3,"old_member":2221933016,"t_old_member":"",
                                    //"admin_uin":1793858042,"t_admin_uin":"","admin_nick":"\u521B\u5EFA\u8005"}}]}
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
                            case "system_message":
                                {
                                    //{"retcode":0,"result":[{"poll_type":"system_message","value":
                                    //{"seq":63433,"type":"verify_required","uiuin":"",
                                    //"from_uin":4011979716,"account":841473232,
                                    //"msg":"aaa","allow":1,"stat":10,"client_type":1}}]}
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
            return string.Format(qq_refuse_file2, uin, ssid, _user.ClientID, _user.PsessionID, QQHelper.GetTime());
        }
        public string GetFileURL(string ssid, string name, string uin)
        {
            name = HttpUtility.UrlEncode(name, Encoding.GetEncoding("GBK"));
            return string.Format(qq_get_file2, ssid, name, uin, _user.PsessionID, QQHelper.GetTime(), _user.ClientID);
        }

        public string RefuleOfffileURL(string uin, string name, string size)
        {
            name = HttpUtility.UrlEncode(name, Encoding.GetEncoding("GBK"));
            return string.Format(qq_notify_offfile2, uin, name, size, _user.PsessionID, _user.ClientID, QQHelper.GetTime());
        }

        public string GetOfffileURL(string ip, string port, string name, string rkey)
        {
            name = HttpUtility.UrlEncode(name, Encoding.GetEncoding("GBK"));
            return string.Format(qq_get_offfile2, ip, port, name, rkey);
        }

        private System.Threading.ManualResetEvent _event = new System.Threading.ManualResetEvent(true);

        public bool SendFriendMessage(QQFriend friend, string msg)
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
            qq_send_buddy_msg2_post["clientid"] = _user.ClientID;
            qq_send_buddy_msg2_post["psessionid"] = _user.PsessionID;
            qq_send_buddy_msg2_post["msg_id"] = _qq_send_buddy_msg2_post_msg_id;
            _qq_send_buddy_msg2_post_msg_id++;
            qq_send_buddy_msg2_post_content[0] = msg;
            qq_send_buddy_msg2_post["content"] = QQHelper.ToJson(qq_send_buddy_msg2_post_content);
            string para = QQHelper.ToPostData(qq_send_buddy_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
            string retstr = PostUrlText(url, Encoding.Default.GetBytes(para));
            bool ret = false;
            if (retstr != null && retstr.Length > 0)
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                if (root["retcode"] as int? == 0)
                {
                    ret = true;
                }
            }
            _event.Set();
            return ret;
        }

        public bool SendGroupMessage(QQGroup group, string msg)
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
            qq_send_qun_msg2_post["clientid"] = _user.ClientID;
            qq_send_qun_msg2_post["psessionid"] = _user.PsessionID;
            qq_send_buddy_msg2_post["msg_id"] = _qq_send_buddy_msg2_post_msg_id;
            _qq_send_buddy_msg2_post_msg_id++;
            qq_send_qun_msg2_post_content[0] = msg;
            qq_send_qun_msg2_post["content"] = QQHelper.ToJson(qq_send_qun_msg2_post_content);
            string para = QQHelper.ToPostData(qq_send_qun_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
            string retstr = PostUrlText(url, Encoding.Default.GetBytes(para));
            bool ret = false;
            if (retstr != null && retstr.Length > 0)
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                if (root["retcode"] as int? == 0)
                {
                    ret = true;
                }
            }
            _event.Set();
            return ret;
        }

        public QQFriends RefreshFriendList()
        {
            try
            {
                string url = qq_get_user_friends2;
                qq_get_user_friends2_post["h"] = "hello";
                qq_get_user_friends2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_get_user_friends2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    _user.QQFriends.Clear();
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
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
                    foreach (Dictionary<string, object> item in result["friends"] as ArrayList)
                    {
                        _user.QQFriends.Add(new QQFriend()
                        {
                            uin = Convert.ToInt64(item["uin"]),
                            flag_friends = Convert.ToInt64(item["flag"]),
                            categories = Convert.ToInt64(item["categories"]),
                        });
                    }
                    foreach (Dictionary<string, object> item in result["info"] as ArrayList)
                    {
                        QQFriend user = _user.GetUserFriend(Convert.ToInt64(item["uin"]));
                        if (user != null)
                        {
                            user.face = Convert.ToInt64(item["face"]);
                            user.flag_info = Convert.ToInt64(item["flag"]);
                            user.nick = item["nick"].ToString();
                        }
                    }
                    foreach (Dictionary<string, object> item in result["vipinfo"] as ArrayList)
                    {
                        QQFriend user = _user.GetUserFriend(Convert.ToInt64(item["u"]));
                        if (user != null)
                        {
                            user.is_vip = Convert.ToInt64(item["is_vip"]);
                            user.vip_level = Convert.ToInt64(item["vip_level"]);
                        }
                    }
                    foreach (Dictionary<string, object> item in result["marknames"] as ArrayList)
                    {
                        QQFriend user = _user.GetUserFriend(Convert.ToInt64(item["uin"]));
                        if (user != null)
                        {
                            user.markname = item["markname"].ToString();
                        }
                        else
                        {
                            _user.QQFriends.Add(new QQFriend()
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
            return _user.QQFriends;
        }

        public QQFriends GetOnlineUsers()
        {
            try
            {
                string url = string.Format(qq_get_online_buddies2, _user.ClientID, _user.PsessionID, QQHelper.GetTime());
                string resultStr = GetUrlText(url);
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    foreach (Dictionary<string, object> item in root["result"] as ArrayList)
                    {
                        QQFriend user = _user.GetUserFriend(Convert.ToInt64(item["uin"]));
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
            return _user.QQFriends;
        }

        public QQGroups RefreshGroupList()
        {
            try
            {
                string url = qq_get_group_name_list_mask2;
                qq_get_group_name_list_mask2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_get_user_friends2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    _user.QQGroups.Clear();
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    foreach (Dictionary<string, object> item in result["gnamelist"] as ArrayList)
                    {
                        _user.QQGroups.Add(new QQGroup()
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
            new Task(() =>
                {
                    foreach (var g in _user.QQGroups.GroupList)
                    {
                        RefreshGroupInfo(g.Value);
                    }
                }).Start();
            return _user.QQGroups;
        }

        public void RefreshGroupInfo(QQGroup group, int timeout = 60000)
        {
            GetGroupMembers(group, timeout);
        }

        public void GetGroupInfo(QQGroup group, int timeout)
        {
            //private static readonly string qq_get_group_info = "http://s.web2.qq.com/api/get_group_info?gcode=%5B{0}%5D&retainKey=memo&vfwebqq={1}&t={2}";

            try
            {
                string url = string.Format(qq_get_group_info, group.code, _user.VfWebQQ, QQHelper.GetTime());
                string resultStr = GetUrlText(url, timeout);
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    ArrayList result = root["result"] as ArrayList;
                    foreach (Dictionary<string, object> item in result)
                    {
                        group.memo = item["memo"] as string;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public bool DenyFriendAdd(long account,string reason)
        {
            try
            {
                string url = qq_deny_added_request2;
                qq_deny_added_request2_post["account"] = account;
                qq_deny_added_request2_post["msg"] = reason??"";
                qq_deny_added_request2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_deny_added_request2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    if(result["result"] as int? == 0)
                        return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public long AllowFriendAdd(long account)
        {
            try
            {
                string url = qq_allow_added_request2;
                qq_allow_added_request2_post["account"] = account;
                qq_allow_added_request2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_allow_added_request2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                //{"retcode":0,"result":{"result":0,"client_type":1,"account":841473232,"tuin":4011979716,"stat":10}}
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    if (result["result"] as int? == 0)
                        if (result.Keys.Contains("tuin"))
                        {
                            return Convert.ToInt64(result["tuin"]);
                        }
                }
            }
            catch (Exception)
            {
            }
            return -1;
        }

        public long AllowFriendAddAndAddFriend(long account, long group, string markname)
        {
            try
            {
                string url = qq_allow_and_add2;
                qq_allow_and_add2_post["account"] = account;
                qq_allow_and_add2_post["gid"] = group;
                qq_allow_and_add2_post["mname"] = markname;
                qq_allow_and_add2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_allow_and_add2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                //{"retcode":0,"result":{"result1":0,"account":841473232,"tuin":4011979716,"stat":10}}
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    if (result["result1"] as int? == 0)
                        if (result.Keys.Contains("tuin"))
                        {
                            return Convert.ToInt64(result["tuin"]);
                        }
                }
            }
            catch (Exception)
            {
            }
            return -1;
        }

        public void GetGroupMembers(QQGroup group, int timeout)
        {
            //private static readonly string qq_get_group_info_ext2 = "http://s.web2.qq.com/api/get_group_info_ext2?gcode={0}&vfwebqq={1}&t={2}";
            //{"retcode":0,"result":
            //{"stats":[{"client_type":41,"uin":344420262,"stat":10}],
            //"minfo":[{"nick":"心冰依然","province":"河北","gender":"male","uin":344420262,"country":"中国","city":"保定"}],
            //"ginfo":{"face":3,"memo":".net开发交流\r（建设中...）","class":10048,"fingermemo":"","code":2546332710,"createtime":1190296107,"flag":17825793,"level":3,"name":".net编程","gid":1535437634,"owner":344420262,
            //"members":[{"muin":344420262,"mflag":200}],"option":2},
            //"cards":[{"muin":1740774180,"card":"[东莞]陈崖"}],
            //"vipinfo":[{"vip_level":0,"u":344420262,"is_vip":0}]}}
            try
            {
                string url = string.Format(qq_get_group_info_ext2, group.code, _user.VfWebQQ, QQHelper.GetTime());
                string resultStr = GetUrlText(url, timeout);
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    //stats,minfo,ginfo,vipinfo,cards[不一定有]
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
                            foreach (Dictionary<string, object> member in members)
                            {
                                QQGroupMember newitem = new QQGroupMember()
                                {
                                    uin = Convert.ToInt64(member["muin"]),
                                    mflag = Convert.ToInt64(member["mflag"]),
                                };
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

                    #region minfo

                    if (result.Keys.Contains("minfo"))
                    {
                        var minfo = result["minfo"] as ArrayList;
                        foreach (Dictionary<string, object> member in minfo)
                        {
                            var newitem = group.GetGroupMember(Convert.ToInt64(member["uin"]));
                            if (newitem == null)
                            {
                                newitem = new QQGroupMember() { uin = Convert.ToInt64(member["uin"]) };
                                group.allMembers.Add(newitem.uin, newitem);
                                group.members.Add(newitem.uin, newitem);
                            }
                            newitem.province = member["province"].ToString();
                            newitem.country = member["country"].ToString();
                            newitem.city = member["city"].ToString();
                            newitem.nick = member["nick"].ToString();
                            newitem.gender = member["gender"].ToString();
                        }
                    }
                    #endregion

                    #region vipinfo

                    if (result.Keys.Contains("vipinfo"))
                    {
                        var minfo = result["vipinfo"] as ArrayList;
                        foreach (Dictionary<string, object> member in minfo)
                        {
                            var newitem = group.GetGroupMember(Convert.ToInt64(member["u"]));
                            if (newitem == null)
                            {
                                newitem = new QQGroupMember() { uin = Convert.ToInt64(member["u"]) };
                                group.allMembers.Add(newitem.uin, newitem);
                                group.members.Add(newitem.uin, newitem);
                            }
                            newitem.vip_level = Convert.ToInt64(member["vip_level"]);
                            newitem.is_vip = Convert.ToInt64(member["is_vip"]);
                        }
                    }
                    #endregion

                    #region stats

                    if (result.Keys.Contains("stats"))
                    {
                        //{"stats":[{"client_type":41,"uin":344420262,"stat":10}],
                        var minfo = result["stats"] as ArrayList;
                        foreach (Dictionary<string, object> member in minfo)
                        {
                            var newitem = group.GetGroupMember(Convert.ToInt64(member["uin"]));
                            if (newitem == null)
                            {
                                newitem = new QQGroupMember() { uin = Convert.ToInt64(member["uin"]) };
                                group.allMembers.Add(newitem.uin, newitem);
                                group.members.Add(newitem.uin, newitem);
                            }
                            newitem.client_type = Convert.ToInt64(member["client_type"]);
                            newitem.stat = Convert.ToInt64(member["stat"]);
                        }
                    }
                    #endregion
                    #region cards

                    if (result.Keys.Contains("cards"))
                    {
                        var minfo = result["cards"] as ArrayList;
                        foreach (Dictionary<string, object> member in minfo)
                        {
                            var newitem = group.GetGroupMember(Convert.ToInt64(member["muin"]));
                            if (newitem == null)
                            {
                                newitem = new QQGroupMember() { uin = Convert.ToInt64(member["muin"]) };
                                group.allMembers.Add(newitem.uin, newitem);
                                group.members.Add(newitem.uin, newitem);
                            }
                            newitem.card = member["card"].ToString();
                        }
                    }
                    #endregion

                    #endregion
                }
            }
            catch (Exception)
            {
            }

        }

        #endregion

        #region GetPost

        public string GetUrlText(string url, int timeout = 60000)
        {
            try
            {
                HttpWebResponse myResponse = GetUrlResponse(url, timeout);
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

        public string PostUrlText(string url, byte[] postData, int timeout = 60000)
        {
            try
            {
                HttpWebResponse myResponse = GetPostResponse(url, postData, timeout);
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

        public Stream GetUrlStream(string url, int timeout = 60000)
        {
            try
            {
                HttpWebResponse myResponse = GetUrlResponse(url, timeout);
                Stream newStream = myResponse.GetResponseStream();
                return newStream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Stream GetPostStream(string url, byte[] postData, int timeout = 60000)
        {
            try
            {
                HttpWebResponse myResponse = GetPostResponse(url, postData, timeout);
                Stream newStream = myResponse.GetResponseStream();
                return newStream;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private HttpWebResponse GetUrlResponse(string url, int timeout = 60000)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            HttpWebResponse response = null;
            Task task = new Task(() =>
                {
                    token.ThrowIfCancellationRequested();
                    HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                    myRequest.Method = "GET";
                    myRequest.Referer = qq_referurl;
                    myRequest.CookieContainer = _cookiecontainer;
                    myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                    myRequest.AllowAutoRedirect = true;
                    myRequest.KeepAlive = true;
                    response = (HttpWebResponse)myRequest.GetResponse();
                }, token);
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return response;
            throw new TimeoutException();
        }

        private HttpWebResponse GetPostResponse(string url, byte[] postData, int timeout = 60000)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                token.ThrowIfCancellationRequested();
                HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                myRequest.Method = "POST";
                myRequest.Referer = qq_referurl;
                myRequest.CookieContainer = _cookiecontainer;
                myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                myRequest.ContentLength = postData.Length;
                using (var sw = myRequest.GetRequestStream())
                {
                    sw.Write(postData, 0, postData.Length);
                }
                response = (HttpWebResponse)myRequest.GetResponse();
            }, token);
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return response;
            throw new TimeoutException();
        }


        public string GetFileTrueUrl(string url, int timeout = 60000)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            String newUrl = null;
            Task task = new Task(() =>
            {

                try
                {
                    token.ThrowIfCancellationRequested();
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
                    if ((myResponse.StatusCode == System.Net.HttpStatusCode.Found) ||
                      (myResponse.StatusCode == System.Net.HttpStatusCode.Redirect) ||
                      (myResponse.StatusCode == System.Net.HttpStatusCode.Moved) ||
                      (myResponse.StatusCode == System.Net.HttpStatusCode.MovedPermanently))
                    {
                        newUrl = headers["Location"];
                        newUrl = newUrl.Trim();
                    }
                    myResponse.Close();
                }
                catch (Exception)
                {
                    newUrl = null;
                }
            }, token);
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return newUrl;
            throw new TimeoutException();
        }


        #endregion

    }
}
