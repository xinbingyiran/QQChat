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
        private static readonly string qq_check = "https://ssl.ptlogin2.qq.com/check?uin={0}&appid=1003903&r={1:f16}";
        private static readonly string qq_getimage = "https://ssl.captcha.qq.com/getimage?aid=1003903&r={0:f16}&uin={1}";
        //private static readonly string qq_login = "https://ssl.ptlogin2.qq.com/login?u={0}&p={1}&verifycode={2}&webqq_type=10&remember_uin=1&login2qq=1&aid=1003903&u1=http%3A%2F%2Fw.qq.com%2Floginproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=3-6-9305&mibao_css=m_webqq&t=1&g=1";
        private static readonly string qq_loginnew = "https://ssl.ptlogin2.qq.com/login?u={0}&p={1}&verifycode={2}&webqq_type=10&remember_uin=1&login2qq=0&aid=1003903&u1=http%3A%2F%2Fweb2.qq.com%2Floginproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=1-15-19876&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10042&login_sig=ies07xzizpY30I2qFMaEp0PXOM3SxgBi40YEjvys0DDcV9vnSQ5Yg4xWNO9H7btA";
        private static readonly string qq_login2 = "http://d.web2.qq.com/channel/login2";
        private static readonly string qq_get_user_friends2 = "http://s.web2.qq.com/api/get_user_friends2";
        //private static readonly string qq_get_friend_info2 = "http://s.web2.qq.com/api/get_friend_info2?tuin={0}&verifysession=&code=&vfwebqq={1}&t={2}";
        private static readonly string qq_getface_user = "http://face3.qun.qq.com/cgi/svr/face/getface?cache=1&type=1&fid=0&uin={0}&vfwebqq={1}&t={2}";
        //private static readonly string qq_getface_qun = "http://face1.qun.qq.com/cgi/svr/face/getface?cache=0&type=4&fid=0&uin={0}&vfwebqq={1}";//uin = code
        private static readonly string qq_get_group_name_list_mask2 = "http://s.web2.qq.com/api/get_group_name_list_mask2";
        private static readonly string qq_get_group_info = "http://s.web2.qq.com/api/get_group_info?gcode=%5B{0}%5D&retainKey=memo&vfwebqq={1}&t={2}";
        private static readonly string qq_get_group_info_ext2 = "http://s.web2.qq.com/api/get_group_info_ext2?gcode={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_send_qun_msg2 = "http://d.web2.qq.com/channel/send_qun_msg2";
        private static readonly string qq_get_online_buddies2 = "http://d.web2.qq.com/channel/get_online_buddies2?clientid={0}&psessionid={1}&t={2}";
        //private static readonly string qq_get_single_long_nick2 = "http://s.web2.qq.com/api/get_single_long_nick2?tuin={0}&vfwebqq={1}&t={2}";
        //private static readonly string qq_set_long_nick2 = "http://s.web2.qq.com/api/set_long_nick2";
        //private static readonly string qq_get_msg_tip = "http://web.qq.com/web2/get_msg_tip?uin=&tp=1&id=0&retype=1&rc=18&lv=3&t={0}";
        //private static readonly string qq_get_discu_list_new2 = "http://d.web2.qq.com/channel/get_discu_list_new2?clientid={0}&psessionid={1}&vfwebqq={2}&t={3}";
        //private static readonly string qq_get_recent_list2 = "http://d.web2.qq.com/channel/get_recent_list2";
        private static readonly string qq_poll2 = "http://d.web2.qq.com/channel/poll2";
        private static readonly string qq_send_buddy_msg2 = "http://d.web2.qq.com/channel/send_buddy_msg2";
        //private static readonly string qq_get_qq_level2 = "http://s.web2.qq.com/api/get_qq_level2?tuin={0}&vfwebqq={1}&t={2}";
        private static readonly string qq_get_friend_uin2_user = "http://s.web2.qq.com/api/get_friend_uin2?tuin={0}&verifysession=&type=1&code=&vfwebqq={1}&t={2}";
        //private static readonly string qq_get_friend_uin2_group = "http://s.web2.qq.com/api/get_friend_uin2?tuin={0}&verifysession=&type=4&code=&vfwebqq={1}&t={2}";
        private static readonly string qq_change_status2 = "http://d.web2.qq.com/channel/change_status2?newstatus={0}&clientid={1}&psessionid={2}&t={3}";
        private static readonly string qq_logout2 = "http://d.web2.qq.com/channel/logout2?ids=&clientid={0}&psessionid={1}&t={2}";
        //private static readonly string qq_search_qq_by_uin2 = "http://s.web2.qq.com/api/search_qq_by_uin2?tuin={0}&verifysession={1}&code={2}&vfwebqq={3}&t={4}";
        private static readonly string qq_get_c2cmsg_sig2 = "http://d.web2.qq.com/channel/get_c2cmsg_sig2?id={0}&to_uin={1}&service_type=0&clientid={2}&psessionid={3}&t={4}";
        private static readonly string qq_get_stranger_info2 = "http://s.web2.qq.com/api/get_stranger_info2?tuin={0}&verifysession=&gid=0&code=&vfwebqq={1}&t={2}";
        private static readonly string qq_send_sess_msg2 = "http://d.web2.qq.com/channel/send_sess_msg2";

        private static readonly string qq_refuse_file2 = "http://d.web2.qq.com/channel/refuse_file2?to={0}&lcid={1}&clientid={2}&psessionid={3}&t={4}";
        private static readonly string qq_get_file2 = "http://d.web2.qq.com/channel/get_file2?lcid={0}&guid={1}&to={2}&psessionid={3}&count=1&time={4}&clientid={5}";
        private static readonly string qq_notify_offfile2 = "http://d.web2.qq.com/channel/notify_offfile2?to={0}&file_name={1}&file_size={2}&action=2&psessionid={3}&clientid={4}&t={5}";
        private static readonly string qq_get_offfile2 = "http://{0}:{1}/{2}?ver=2173&rkey={3}&range=0";
        private static readonly string qq_get_offpic2 = "http://d.web2.qq.com/channel/get_offpic2?file_path={0}&f_uin={1}&clientid={2}&psessionid={3}";

        private static readonly string qq_face = "http://0.web.qstatic.com/webqqpic/style/face/{0}.gif";

        private static readonly string qq_cface = "http://qun.qq.com/cgi/svr/chatimg/get?af=1&pic={0}&gid={1}&time={2}";

        private static readonly string qq_cface2 = "http://d.web2.qq.com/channel/get_cface2?lcid={0}&guid={1}&to={2}&count=5&time=1&clientid={3}&psessionid={4}";

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
            {"hash",""},
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


        private Dictionary<string, object> qq_send_sess_msg2_post = new Dictionary<string, object>()
        {
            {"to",0},
            {"group_sig",""},
            {"face",0},
            {"content",""},
            {"msg_id",56410001},
            {"clientid",""},
            {"psessionid",""}
        };

        private ArrayList qq_send_buddy_msg2_post_content_append = new ArrayList(){
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

        private ArrayList qq_send_qun_msg2_post_content_append = new ArrayList(){
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
        public event EventHandler<ErrorEventArgs> GetMessageError;

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

        public static string GetFaceUrl(object face)
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

        public static string GetTrueFaceUrl(int id)
        {
            return string.Format(qq_face, id);
        }

        public string GetCFaceUrl(string pic, string gid)
        {
            string url = string.Format(qq_cface, pic, gid, QQHelper.GetTime());
            return GetFileTrueUrl(url);
        }

        public string GetCFace2Url(string msg_id, string filename, string guin)
        {
            //"http://d.web2.qq.com/channel/get_cface2?lcid={0}&guid={1}&to={2}&count=5&time=1&clientid={3}&psessionid={4}";
            return string.Format(qq_cface2, msg_id, filename, guin, _user.ClientID, _user.PsessionID);
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
            string url = string.Format(qq_loginnew, _user.QQNum, mpass, vercode);
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
                string sub0 = sublist[0].Trim().Trim('\'');
                string sub2 = sublist[2].Trim().Trim('\'');
                if (sub0 == "0")
                {
                    try
                    {
                        string sub2r = GetUrlText(sub2);
                        _user.QQName = sublist[5].Trim().Trim('\'');
                        result = _user.QQName + ":" + sublist[4].Trim().Trim('\'');
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    result = sub2;
                }
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
            try
            {
                if (_messageTaskCts != null && !_messageTaskCts.IsCancellationRequested)
                {
                    _messageTaskCts.Cancel(false);
                    _messageTaskCts = null;
                }
                if (_user.PtWebQQ == null || _user.PtWebQQ.Length == 0)
                {
                    return "尚未全局登录成功";
                }
                string url = string.Format(qq_logout2, _user.ClientID, _user.PsessionID, QQHelper.GetTime());
                string retstr = GetUrlText(url, 3000);
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
            }
            catch (Exception)
            {
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
            string url = string.Format(qq_change_status2, newStatus, _user.ClientID, _user.PsessionID, QQHelper.GetTime());
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
        public bool GetFriendStrangeInfo(QQFriend friend)
        {
            try
            {
                //tuin={0}&verifysession=&gid=0&code=&vfwebqq={1}&t={2}";
                string url = string.Format(qq_get_stranger_info2, friend.uin, _user.VfWebQQ, QQHelper.GetTime());
                string retstr = GetUrlText(url);
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                    if (root["retcode"] as int? == 0)
                    {
                        Dictionary<string, object> result = root["result"] as Dictionary<string, object>;

                        if (result.ContainsKey("face")) friend.face = Convert.ToInt64(result["face"]);
                        if (result.ContainsKey("birthday")) friend.birthday = QQHelper.ToJson(result["birthday"]);
                        if (result.ContainsKey("phone")) friend.phone = result["phone"] as string;
                        if (result.ContainsKey("occupation")) friend.occupation = result["occupation"] as string;
                        if (result.ContainsKey("allow")) friend.allow = Convert.ToInt64(result["allow"]);
                        if (result.ContainsKey("college")) friend.college = result["college"] as string;
                        if (result.ContainsKey("uin")) friend.uin = Convert.ToInt64(result["uin"]);
                        if (result.ContainsKey("blood")) friend.blood = Convert.ToInt64(result["blood"]);
                        if (result.ContainsKey("constel")) friend.constel = Convert.ToInt64(result["constel"]);
                        if (result.ContainsKey("homepage")) friend.homepage = result["homepage"] as string;
                        if (result.ContainsKey("stat")) friend.stat = Convert.ToInt64(result["stat"]);
                        if (result.ContainsKey("country")) friend.country = result["country"] as string;
                        if (result.ContainsKey("city")) friend.city = result["city"] as string;
                        if (result.ContainsKey("personal")) friend.personal = result["personal"] as string;
                        if (result.ContainsKey("nick")) friend.nick = result["nick"] as string;
                        if (result.ContainsKey("shengxiao")) friend.shengxiao = Convert.ToInt64(result["shengxiao"]);
                        if (result.ContainsKey("email")) friend.email = result["email"] as string;
                        if (result.ContainsKey("token")) friend.token = result["token"] as string;
                        if (result.ContainsKey("client_type")) friend.client_type = Convert.ToInt64(result["client_type"]);
                        if (result.ContainsKey("province")) friend.province = result["province"] as string;
                        if (result.ContainsKey("gender")) friend.gender = result["gender"] as string;
                        if (result.ContainsKey("mobile")) friend.mobile = result["mobile"] as string;
                        if (MessageFriendReceived != null)
                        {
                            MessageFriendReceived(this, new FriendEventArgs(friend, 0, DateTime.Now, MessageEventType.MESSAGE_USER));
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool GetGroupMemberStrangeInfo(QQGroupMember member)
        {
            try
            {
                //tuin={0}&verifysession=&gid=0&code=&vfwebqq={1}&t={2}";
                string url = string.Format(qq_get_stranger_info2, member.uin, _user.VfWebQQ, QQHelper.GetTime());
                string retstr = GetUrlText(url);
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                    if (root["retcode"] as int? == 0)
                    {
                        Dictionary<string, object> result = root["result"] as Dictionary<string, object>;

                        if (result.ContainsKey("face")) member.face = Convert.ToInt64(result["face"]);
                        if (result.ContainsKey("birthday")) member.birthday = QQHelper.ToJson(result["birthday"]);
                        if (result.ContainsKey("phone")) member.phone = result["phone"] as string;
                        if (result.ContainsKey("occupation")) member.occupation = result["occupation"] as string;
                        if (result.ContainsKey("allow")) member.allow = Convert.ToInt64(result["allow"]);
                        if (result.ContainsKey("college")) member.college = result["college"] as string;
                        if (result.ContainsKey("uin")) member.uin = Convert.ToInt64(result["uin"]);
                        if (result.ContainsKey("blood")) member.blood = Convert.ToInt64(result["blood"]);
                        if (result.ContainsKey("constel")) member.constel = Convert.ToInt64(result["constel"]);
                        if (result.ContainsKey("homepage")) member.homepage = result["homepage"] as string;
                        if (result.ContainsKey("stat")) member.stat = Convert.ToInt64(result["stat"]);
                        if (result.ContainsKey("country")) member.country = result["country"] as string;
                        if (result.ContainsKey("city")) member.city = result["city"] as string;
                        if (result.ContainsKey("personal")) member.personal = result["personal"] as string;
                        if (result.ContainsKey("nick")) member.nick = result["nick"] as string;
                        if (result.ContainsKey("shengxiao")) member.shengxiao = Convert.ToInt64(result["shengxiao"]);
                        if (result.ContainsKey("email")) member.email = result["email"] as string;
                        if (result.ContainsKey("token")) member.token = result["token"] as string;
                        if (result.ContainsKey("client_type")) member.client_type = Convert.ToInt64(result["client_type"]);
                        if (result.ContainsKey("province")) member.province = result["province"] as string;
                        if (result.ContainsKey("gender")) member.gender = result["gender"] as string;
                        if (result.ContainsKey("mobile")) member.mobile = result["mobile"] as string;
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool GetFriendInfos(QQFriend friend)
        {
            return GetFriendQQNum(friend) && GetFriendStrangeInfo(friend);
        }

        public bool GetGroupMemberInfos(QQGroup group, QQGroupMember member)
        {
            return GetGroupMemberQQNum(group, member) && GetGroupMemberStrangeInfo(member);
        }

        public bool GetFriendQQNum(QQFriend friend)
        {
            try
            {
                string url = string.Format(qq_get_friend_uin2_user, friend.uin, _user.VfWebQQ, QQHelper.GetTime());
                string retstr = GetUrlText(url);
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                    if (root["retcode"] as int? == 0)
                    {
                        Dictionary<string, object> result = root["result"] as Dictionary<string, object>;

                        long qqacc = Convert.ToInt64(result["account"]);
                        friend.num = qqacc;
                        if (MessageFriendReceived != null)
                        {
                            MessageFriendReceived(this, new FriendEventArgs(friend, 0, DateTime.Now, MessageEventType.MESSAGE_USER));
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public string GetGroupQQNum(QQGroup group)
        {
            try
            {
                string url = string.Format(qq_get_friend_uin2_user, group.code, _user.VfWebQQ, QQHelper.GetTime());
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
                            group.num = qqacc;
                            if (MessageGroupReceived != null)
                            {
                                MessageGroupReceived(this, new GroupEventArgs(group, null, 0, DateTime.Now, MessageEventType.MESSAGE_USER, string.Empty));
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

        public bool GetGroupMemberQQNum(QQGroup group, QQGroupMember member)
        {
            try
            {
                string url = string.Format(qq_get_friend_uin2_user, member.uin, _user.VfWebQQ, QQHelper.GetTime());
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
                            member.num = qqacc;
                            if (MessageGroupReceived != null)
                            {
                                MessageGroupReceived(this, new GroupEventArgs(group, member, 0, DateTime.Now, MessageEventType.MESSAGE_USER, string.Empty));
                            }
                            return true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void StartGetMessage()
        {
            _messageTask = new Task(() =>
            {
                if (_messageTaskCts == null)
                {
                    _messageTaskCts = new CancellationTokenSource();
                }
                GetMessage();
            });
            _messageTask.Start();
        }

        public void GetMessage()
        {
            while (_user.Status != QQStatus.StatusOffline.StatusInternal)
            {
                _messageTaskCts.Token.ThrowIfCancellationRequested();
                GetMessageSub();
            }
        }

        public void ThrowError(string error)
        {
            User.Status = QQStatus.StatusOffline.StatusInternal;
            if (GetMessageError != null)
            {
                GetMessageError(this, new ErrorEventArgs(new Exception(error)));
            }
        }

        public void GetMessageSub()
        {
            string url = qq_poll2;
            qq_poll2_post["clientid"] = _user.ClientID;
            qq_poll2_post["psessionid"] = _user.PsessionID;
            string para = QQHelper.ToPostData(qq_poll2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
            string str = PostUrlText(url, Encoding.Default.GetBytes(para), int.MaxValue);
            //str = null;未获取信息
            if (str == null)
            {
                ThrowError("网络中断");
                return;
            }
            try
            {
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(str);
                var retcode = (root["retcode"] as int?).GetValueOrDefault(-1);
                if (retcode == 102)
                {
                    //没信息
                }
                else if (retcode == 103 || retcode == 121)
                {
                    ThrowError("掉线");
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
                                    //{"retcode":0,"result":[{"poll_type":"buddies_status_change","value":{"uin":15130679,"status":"online","client_type":1}}]}

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
                                    //{"retcode":0,"result":[{"poll_type":"message","value":
                                    //{"msg_id":17964,"from_uin":1550833875,"to_uin":841473232,"msg_id2":802927,"msg_type":9,"reply_ip":178847978,"time":1336723235,"content":[["font",
                                    //{"size":12,"color":"000080","style":[0,0,0],"name":"\u534E\u6587\u5B8B\u4F53"}],"\u6B66\u5A01  "]}}]}
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
                                    //{"poll_type":"sess_message",
                                    //"msg_id":26835,"from_uin":3365751686,"to_uin":2221933016,"msg_id2":367496,"msg_type":140,"reply_ip":178849369,"time":1366004047,
                                    //"id":1429244372,"ruin":841473232,"service_type":0,"flags":{"text":1,"pic":1,"file":1,"audio":1,"video":1},
                                    //"content":[["font",{"size":12,"color":"391175","style":[0,0,0],"name":"宋体"}],
                                    //["face",59]," "]}
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
                                    //{"retcode":0,"result":[{"poll_type":"shake_message","value":
                                    //{"msg_id":26413,"from_uin":2867991890,"to_uin":374491485,"msg_id2":980317,
                                    //"msg_type":9,"reply_ip":178849323}}]}
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
                                        if (member == null)
                                        {
                                            new Task(() =>
                                            {
                                                RefreshGroupInfo(group);
                                            }).Start();
                                        }
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, QQHelper.ToTime(Convert.ToInt64(messagevalue["time"])), MessageEventType.MESSAGE_COMMON, msgs));
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
                                        if (member == null)
                                        {
                                            new Task(() => RefreshGroupInfo(group)).Start();
                                        }
                                        long msgid = Convert.ToInt64(messagevalue["msg_id"]);
                                        MessageGroupReceived(this, new GroupEventArgs(group, member, msgid, DateTime.Now, MessageEventType.MESSAGE_COMMON, msgs));
                                    }
                                }
                                break;
                            case "file_message":
                                //{"retcode":0,"result":[{"poll_type":"file_message","value":
                                //{"msg_id":6299,"mode":"recv","from_uin":2786090795,"to_uin":344420262,
                                //"msg_id2":559004,"msg_type":9,"reply_ip":176621916,"type":101,
                                //"name":"test.txt","time":1364865210,"session_id":19823,"inet_ip":2051695055}}]}

                                //{"retcode":0,"result":[{"poll_type":"file_message","value":
                                //{"msg_id":6301,"mode":"refuse","from_uin":2786090795,"to_uin":344420262,
                                //"msg_id2":615845,"reply_ip":176621916,"type":101,
                                //"session_id":19823,"cancel_type":1,"time":1364865264}}]}
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
                                //{"retcode":0,"result":[{"poll_type":"push_offfile","value":
                                //{"msg_id":10438,"rkey":"2c09344b4c531f458e9f0b89162e6979624ff73400585f72e2322d8da678f5f06bb2f61eee2682a6647428095d475757478a4fc78662e36d58d34f1f3146bbdb",
                                //"ip":"101.226.77.168","port":80,"from_uin":2786090795,"size":235,"name":"test2.py","expire_time":1365470120,"time":1364865321}}]}
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
                                    //{"retcode":0,"result":[{"poll_type":"input_notify","value":
                                    //{"msg_id":52149,"from_uin":2786090795,"to_uin":344420262,"msg_id2":2553447584,"msg_type":121,"reply_ip":4294967295}}]}

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
                                    //{"retcode":0,"result":[{"poll_type":"system_message","value":
                                    //{"seq":63433,"type":"verify_required","uiuin":"",
                                    //"from_uin":4011979716,"account":841473232,
                                    //"msg":"aaa","allow":1,"stat":10,"client_type":1}}]}
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

        public bool SendFriendMessage(QQFriend friend, ArrayList msg)
        {
            if (msg == null || msg.Count == 0)
            {
                return false;
            }
            if (_event.WaitOne(5000) == false)
            {
                return false;
            }
            _event.Reset();
            try
            {
                string url = qq_send_buddy_msg2;
                qq_send_buddy_msg2_post["to"] = friend.uin;
                qq_send_buddy_msg2_post["clientid"] = _user.ClientID;
                qq_send_buddy_msg2_post["psessionid"] = _user.PsessionID;
                qq_send_buddy_msg2_post["msg_id"] = _qq_send_buddy_msg2_post_msg_id;
                _qq_send_buddy_msg2_post_msg_id++;
                msg.AddRange(qq_send_buddy_msg2_post_content_append);
                qq_send_buddy_msg2_post["content"] = QQHelper.ToJson(msg);
                string para = QQHelper.ToPostData(qq_send_buddy_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
                string retstr = PostUrlText(url, Encoding.Default.GetBytes(para), 5000);
                bool ret = false;
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                    if (root["retcode"] as int? == 0)
                    {
                        ret = true;
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _event.Set();
            }
        }


        public bool CheckGroupSig(QQFriend friend)
        {
            if (friend.group_sig != null)
                return true;
            return GetGroupSig(friend, friend.id);
        }

        public bool GetGroupSig(QQFriend friend, long groupid)
        {
            if (groupid == 0)
                return false;
            if (_event.WaitOne(5000) == false)
            {
                return false;
            }
            _event.Reset();
            try
            {
                var ret = false;
                //"id={0}&to_uin={1}&service_type=0&clientid={2}&psessionid={3}&t={4}";
                var url = string.Format(qq_get_c2cmsg_sig2, groupid, friend.uin, _user.ClientID, _user.PsessionID, QQHelper.GetTime());
                string resultStr = GetUrlText(url);
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> paras = root["result"] as Dictionary<string, object>;
                    friend.group_sig = paras["value"] as string;
                    ret = true;
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _event.Set();
            }
        }

        public bool SendSessMessage(QQFriend friend, ArrayList msg)
        {
            if (msg == null || msg.Count == 0)
            {
                return false;
            }
            if (!CheckGroupSig(friend))
            {
                return false;
            }
            if (_event.WaitOne(5000) == false)
            {
                return false;
            }
            _event.Reset();
            try
            {
                string url = qq_send_sess_msg2;
                qq_send_sess_msg2_post["to"] = friend.uin;
                qq_send_sess_msg2_post["group_sig"] = friend.group_sig;
                qq_send_sess_msg2_post["clientid"] = _user.ClientID;
                qq_send_sess_msg2_post["psessionid"] = _user.PsessionID;
                qq_send_sess_msg2_post["msg_id"] = _qq_send_buddy_msg2_post_msg_id;
                _qq_send_buddy_msg2_post_msg_id++;
                msg.AddRange(qq_send_buddy_msg2_post_content_append);
                qq_send_sess_msg2_post["content"] = QQHelper.ToJson(msg);
                string para = QQHelper.ToPostData(qq_send_sess_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
                string retstr = PostUrlText(url, Encoding.Default.GetBytes(para), 5000);
                bool ret = false;
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                    if (root["retcode"] as int? == 0)
                    {
                        ret = true;
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _event.Set();
            }
        }

        public bool SendGroupMessage(QQGroup group, ArrayList msg)
        {
            if (msg == null || msg.Count == 0)
            {
                return false;
            }
            if (_event.WaitOne(5000) == false)
            {
                return false;
            }
            _event.Reset();
            try
            {
                string url = qq_send_qun_msg2;
                qq_send_qun_msg2_post["group_uin"] = group.gid;
                qq_send_qun_msg2_post["clientid"] = _user.ClientID;
                qq_send_qun_msg2_post["psessionid"] = _user.PsessionID;
                qq_send_buddy_msg2_post["msg_id"] = _qq_send_buddy_msg2_post_msg_id;
                _qq_send_buddy_msg2_post_msg_id++;
                msg.AddRange(qq_send_qun_msg2_post_content_append);
                qq_send_qun_msg2_post["content"] = QQHelper.ToJson(msg);
                string para = QQHelper.ToPostData(qq_send_qun_msg2_post) + string.Format("&clientid={0}&psessionid={1}", _user.ClientID, _user.PsessionID);
                string retstr = PostUrlText(url, Encoding.Default.GetBytes(para), 5000);
                bool ret = false;
                if (retstr != null && retstr.Length > 0)
                {
                    Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(retstr);
                    if (root["retcode"] as int? == 0)
                    {
                        ret = true;
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _event.Set();
            }
        }

        public QQFriends RefreshFriendList()
        {
            try
            {
                string url = qq_get_user_friends2;
                qq_get_user_friends2_post["h"] = "hello";
                qq_get_user_friends2_post["hash"] = QQHelper.GetToken(_user);
                qq_get_user_friends2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_get_user_friends2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
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
                        QQFriend user = _user.GetUserFriend(Convert.ToInt64(item["uin"]), false);
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

        public bool DenyFriendAdd(long account, string reason)
        {
            try
            {
                string url = qq_deny_added_request2;
                qq_deny_added_request2_post["account"] = account;
                qq_deny_added_request2_post["msg"] = reason ?? "";
                qq_deny_added_request2_post["vfwebqq"] = _user.VfWebQQ;
                string para = QQHelper.ToPostData(qq_deny_added_request2_post);
                string resultStr = PostUrlText(url, Encoding.Default.GetBytes(para));
                Dictionary<string, object> root = QQHelper.FromJson<Dictionary<string, object>>(resultStr);
                if (root != null && root["retcode"] as int? == 0)
                {
                    Dictionary<string, object> result = root["result"] as Dictionary<string, object>;
                    if (result["result"] as int? == 0)
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
                        //{"stats":[{"client_type":41,"uin":344420262,"stat":10}],
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
            if (_messageTaskCts == null)
            {
                _messageTaskCts = new CancellationTokenSource();
            }
            _messageTaskCts.Token.ThrowIfCancellationRequested();
            HttpWebResponse response = null;
            Task task = new Task(() =>
                {
                    HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                    myRequest.Method = "GET";
                    myRequest.Referer = qq_referurl;
                    myRequest.CookieContainer = _cookiecontainer;
                    myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                    myRequest.AllowAutoRedirect = true;
                    myRequest.KeepAlive = true;
                    _messageTaskCts.Token.ThrowIfCancellationRequested();
                    response = (HttpWebResponse)myRequest.GetResponse();
                });
            task.Start();
            bool wait = task.Wait(timeout, _messageTaskCts.Token);
            if (wait)
                return response;
            throw new TimeoutException();
        }

        private HttpWebResponse GetPostResponse(string url, byte[] postData, int timeout = 60000)
        {
            if (_messageTaskCts == null)
            {
                _messageTaskCts = new CancellationTokenSource();
            }
            _messageTaskCts.Token.ThrowIfCancellationRequested();
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                myRequest.Method = "POST";
                myRequest.Referer = qq_referurl;
                myRequest.CookieContainer = _cookiecontainer;
                myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                myRequest.ContentLength = postData.Length;
                _messageTaskCts.Token.ThrowIfCancellationRequested();
                using (var sw = myRequest.GetRequestStream())
                {
                    sw.Write(postData, 0, postData.Length);
                }
                response = (HttpWebResponse)myRequest.GetResponse();
            });
            task.Start();
            bool wait = task.Wait(timeout, _messageTaskCts.Token);
            if (wait)
                return response;
            throw new TimeoutException();
        }


        public string GetFileTrueUrl(string url, int timeout = 60000)
        {
            if (_messageTaskCts == null)
            {
                _messageTaskCts = new CancellationTokenSource();
            }
            _messageTaskCts.Token.ThrowIfCancellationRequested();
            String newUrl = null;
            Task task = new Task(() =>
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
                    _messageTaskCts.Token.ThrowIfCancellationRequested();
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
            });
            task.Start();
            bool wait = task.Wait(timeout, _messageTaskCts.Token);
            if (wait)
                return newUrl;
            throw new TimeoutException();
        }


        #endregion

    }
}
