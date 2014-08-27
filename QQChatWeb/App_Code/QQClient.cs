using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebQQ2.WebQQ2;

namespace QQChatWeb.App_Code
{
    public class QQClient
    {
        private static readonly string PluginPath = Global.ApplicationPath + "\\plugin";
        private static readonly string FacePath = Global.ApplicationPath + "\\face";
        private static readonly Dictionary<string, dynamic> Plugins = new Dictionary<string, dynamic>();

        private QQ _qq;
        public QQ Client
        {
            get
            {
                return _qq;
            }
            set
            {
                _qq = value;
            }
        }
        public QQClient()
        {

        }
        public void Run()
        {
            if (_qq == null)
                throw new ArgumentNullException();
            _qq.MessageFriendReceived += QQMessageFriendReceived;
            _qq.MessageGroupReceived += QQMessageGroupReceived;
            _qq.GetMessageError += QQ_GetMessageError;
            CheckQQStatus();
            Task.Factory.StartNew(() =>
            {
                GetAllFriends();
                Thread.Sleep(500);
                GetAllGroups();
                Thread.Sleep(500);
                _qq.StartGetMessage();
            });
        }
        private void GetAllFriends()
        {
            _qq.RefreshFriendList();
            _qq.GetOnlineUsers();
        }
        private void GetAllGroups()
        {
            _qq.RefreshGroupList();
        }

        private void QQMessageGroupReceived(object sender, GroupEventArgs e)
        {
            switch (e.Mtype)
            {
                case MessageEventType.MESSAGE_COMMON:
                    {

                        if (e.MsgContent == null)
                            return;
                        new Task(() =>
                        {
                            if (e.Group.num == 0)
                            {
                                _qq.GetGroupQQNum(e.Group);
                            }
                            if (e.Member.num == 0)
                            {
                                _qq.GetGroupMemberInfos(e.Group, e.Member);
                            }
                            SetGroupText(e.Group, e.Member, e.MsgContent, e.Time, e.Msg_id);
                            if (e.Time > _qq.User.LoginTime)
                            {
                                string rmsg = InternalPickMessage(e.MsgContent);
                                if (rmsg != null)
                                {
                                    SendGroupMessage(e.Group, e.Member, rmsg);
                                }
                                else
                                {
                                    var info = new Dictionary<string, object>
                                    {
                                        {TranslateMessageGroup.GroupNum.Key,e.Group == null?0:e.Group.num},
                                        {TranslateMessageGroup.GroupName.Key,e.Group == null?"":e.Group.name},
                                        {TranslateMessageGroup.MemberNum.Key,e.Member == null?0:e.Member.num},
                                        {TranslateMessageGroup.MemberNick.Key,e.Member == null?"":e.Member.nick},
                                        {TranslateMessageGroup.MemberCard.Key,e.Member == null?"":e.Member.card},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealMessage(MessageType.MessageGroup, info, e.MsgContent);
                                        if (rmsg != null)
                                        {
                                            SendGroupMessage(e.Group, e.Member, rmsg);
                                            break;
                                        }
                                    }
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_USER:
                    {
                        if (e.Member == null)
                        {
                            //is group message
                            RefreshGroup(e.Group);
                        }
                        //else
                        //{
                        //    //is member message
                        //}
                    }
                    break;
                default:
                    break;
            }
        }

        private void SendGroupMessage(QQGroup qQGroup, QQGroupMember qQGroupMember, string rmsg)
        {
            throw new NotImplementedException();
        }

        private void SetGroupText(QQGroup qQGroup, QQGroupMember qQGroupMember, string p1, DateTime dateTime, long p2)
        {
            throw new NotImplementedException("SetGroupText");
        }


        public void SetFriendText(QQFriend friend, string msg, DateTime time, long msg_id)
        {
            throw new NotImplementedException("SetFriendText");
        }
        private void RefreshGroup(QQGroup qQGroup)
        {
            throw new NotImplementedException("RefreshGroup");
        }

        private void RefreshUser(QQFriend friend, bool? isMessage = null)
        {
            throw new NotImplementedException("RefreshUser");
        }


        private void QQMessageFriendReceived(object sender, FriendEventArgs e)
        {
            switch (e.Mtype)
            {
                case MessageEventType.MESSAGE_COMMON:
                    {
                        SetFriendText(e.User, e.MsgContent, e.Time, e.Msg_id);
                        if (e.Time > _qq.User.LoginTime)
                        {
                            string rmsg = InternalPickMessage(e.MsgContent);
                            if (rmsg != null)
                            {
                                SendFriendMessage(e.User, rmsg);
                            }
                            else
                            {
                                new Task(() =>
                                {
                                    if (e.User.num == 0)
                                    {
                                        _qq.GetFriendInfos(e.User);
                                    }
                                    var info = new Dictionary<string, object>
                                    {
                                        {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                        {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                        {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealMessage(MessageType.MessageFriend, info, e.MsgContent);
                                        if (rmsg != null)
                                        {
                                            SendFriendMessage(e.User, rmsg);
                                            break;
                                        }
                                    }
                                }).Start();
                            }
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_SESS:
                    {
                        SetSessText(e.User, e.MsgContent, e.Time, e.Msg_id);
                        if (e.Time > _qq.User.LoginTime)
                        {
                            string rmsg = InternalPickMessage(e.MsgContent);
                            if (rmsg != null)
                            {
                                SendSessMessage(e.User, rmsg);
                            }
                            else
                            {
                                new Task(() =>
                                {
                                    var info = new Dictionary<string, object>
                                    {
                                        {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                        {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                        {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealMessage(MessageType.MessageFriend, info, e.MsgContent);
                                        if (rmsg != null)
                                        {
                                            SendSessMessage(e.User, rmsg);
                                            break;
                                        }
                                    }
                                }).Start();
                            }
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_FILE:
                    {
                        if (e.Msgs["mode"].ToString() == "recv")
                        {
                            //string accurl = _user.GetFileURL(e.Msgs["session_id"].ToString(), e.Msgs["name"].ToString(), e.Msgs["from_uin"].ToString());
                            //accurl = _user.GetFileTrueUrl(accurl);
                            //string refurl = _user.RefuseFileURL(e.Msgs["from_uin"].ToString(), e.Msgs["session_id"].ToString());
                            //refurl = _user.GetFileTrueUrl(refurl);
                            string msg = string.Format("对方尝试发送文件[{0}]:{1}", e.Msgs["session_id"], e.Msgs["name"]);
                            SetFriendText(e.User, msg, e.Time, e.Msg_id);
                            //告知对方发送离线文件
                            msg = string.Format("不能接收文件[{0}],请发离线或邮箱。", e.Msgs["name"]);
                            SendFriendMessage(e.User, msg);
                        }
                        else if (e.Msgs["mode"].ToString() == "refuse")
                        {
                            SetFriendText(e.User, string.Format("对方取消发送文件[{0}]", e.Msgs["session_id"]), e.Time, e.Msg_id);
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_OFFLINE:
                    {
                        string accurl = _qq.GetOfffileURL(e.Msgs["ip"].ToString(), e.Msgs["port"].ToString(), e.Msgs["name"].ToString(), e.Msgs["rkey"].ToString());
                        //string refurl = _user.RefuleOfffileURL(e.Msgs["from_uin"].ToString(), e.Msgs["name"].ToString(), e.Msgs["size"].ToString());
                        string msg = string.Format("对方发送离线文件:{0}\r\n下载:{1}", e.Msgs["name"], accurl);
                        SetFriendText(e.User, msg, e.Time, e.Msg_id);
                    }
                    break;
                case MessageEventType.MESSAGE_STATUS:
                    {
                        new Task(() =>
                        {
                            if (e.User.num == 0)
                            {
                                _qq.GetFriendInfos(e.User);
                            }
                            var info = new Dictionary<string, object>
                            {
                                {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                            };
                            if (e.User != null)
                            {
                                var state = QQStatus.GetQQStatusByInternal(e.User.status);
                                string messagestate = string.Format("状态更改：{0} => {1} @ {2}", e.User.LongName, state == null ? e.User.status : state.Status, e.Time);
                                SetSystemText(messagestate, e.User, e.Time, e.Msg_id);
                                RefreshUser(e.User);
                                foreach (var p in Plugins)
                                {
                                    if (!p.Value.Enabled)
                                    {
                                        continue;
                                    }
                                    string rmsg = p.Value.DealMessage(MessageType.MessageStatus, info, QQStatus.GetQQStatusByInternal(e.User.status).Status);
                                    if (rmsg != null)
                                    {
                                        SendFriendMessage(e.User, rmsg);
                                        break;
                                    }
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_SHAKE:
                    {
                        const string msg = "抖动";
                        SetFriendText(e.User, msg, e.Time, e.Msg_id);
                    }
                    break;
                case MessageEventType.MESSAGE_USER:
                    {
                        RefreshUser(e.User);
                    }
                    break;
                case MessageEventType.MESSAGE_INPUT:
                    {
                        new Task(() =>
                        {
                            if (e.User.num == 0)
                            {
                                _qq.GetFriendInfos(e.User);
                            }
                            var info = new Dictionary<string, object>
                            {
                                {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                            };
                            //string msg = "-----正在输入-----";
                            //SetFriendText(e.User, msg, e.Time);
                            foreach (var p in Plugins)
                            {
                                if (!p.Value.Enabled)
                                {
                                    continue;
                                }
                                string rmsg = p.Value.DealMessage(MessageType.MessageInput, info, null);
                                if (rmsg != null)
                                {
                                    SendFriendMessage(e.User, rmsg);
                                    break;
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_KICK:
                    {
                        string msg = string.Format("掉线：@ {0}\r\n{1}", e.Time, e.Msgs["reason"]);
                        SetSystemText(msg, null, e.Time, e.Msg_id);
                    }
                    break;
                case MessageEventType.MESSAGE_SYSTEM:
                case MessageEventType.MESSAGE_UNKNOW:
                    {
                        if (e.Msgs["poll_type"] as string == "system_message")
                        {
                            if (e.Msgs["type"] as string == "verify_required")
                            {
                                long gid = 0;
                                if (_qq.User.QQFriends.GroupList.Any())
                                {
                                    gid = _qq.User.QQFriends.GroupList.First().Key;
                                }
                                if (gid < 0) gid = 0;
                                long uin = _qq.AllowFriendAddAndAddFriend(Convert.ToInt64(e.Msgs["account"]), gid, "");
                                if (uin > 0)
                                {
                                    SetSystemText("新用户添加", null, e.Time, e.Msg_id);
                                    new Task(GetAllFriends).Start();
                                    break;
                                }
                            }
                        }
                        else if (e.Msgs["poll_type"] as string == "sys_g_message")
                        {
                            if (e.Msgs["type"] as string == "group_join")
                            {
                                SetSystemText("新群添加 by " + e.Msgs["admin_nick"] as string, null, e.Time, e.Msg_id);
                                new Task(GetAllGroups).Start();
                                break;
                            }
                            else if (e.Msgs["type"] as string == "group_leave")
                            {
                                SetSystemText("旧群移除 by " + e.Msgs["admin_nick"] as string, null, e.Time, e.Msg_id);
                                new Task(GetAllGroups).Start();
                                break;
                            }
                        }
                        SetSystemText(JsonConvert.SerializeObject(e.Msgs), null, e.Time, e.Msg_id);
                    }
                    break;
                default:
                    {
                        SetSystemText(JsonConvert.SerializeObject(e.Msgs), null, e.Time, e.Msg_id);
                    }
                    break;
            }
        }

        private void SendSessMessage(QQFriend qQFriend, string rmsg)
        {
            throw new NotImplementedException();
        }

        private void SetSessText(QQFriend qQFriend, string p1, DateTime dateTime, long p2)
        {
            throw new NotImplementedException();
        }

        private void SetSystemText(string messagestate, QQFriend qQFriend, DateTime dateTime, long p)
        {
            throw new NotImplementedException();
        }

        private void SendFriendMessage(QQFriend qQFriend, string rmsg)
        {
            throw new NotImplementedException();
        }

        private void QQ_GetMessageError(object sender, ErrorEventArgs e)
        {
            if (e.GetException().Message == "网络中断")
            {
                ReturnToLogIn();
            }
            else
            {
                new Task(() =>
                {
                    try
                    {
                        _qq.LoginQQ2(QQStatus.StatusOnline.StatusInternal);
                        GetAllFriends();
                        Thread.Sleep(500);
                        GetAllGroups();
                        Thread.Sleep(500);
                        _qq.StartGetMessage();
                        return;
                    }
                    catch (Exception)
                    {
                    }
                    ReturnToLogIn();
                }).Start();
            }
        }

        private void ReturnToLogIn()
        {
            throw new NotImplementedException();
        }

        private void CheckQQStatus()
        {
            throw new NotImplementedException();
        }


        public string InternalPickMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            switch (message)
            {
                case "--help":
                case "--帮助":
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("--help/帮助  ->  \"显示此帮助\"");
                        foreach (var plugin in Plugins.Values)
                        {
                            if (!plugin.Enabled)
                            {
                                continue;
                            }
                            foreach (KeyValuePair<string, string> filter in plugin.Filters)
                            {
                                sb.AppendFormat("{0}  ->  \"{1}\"{2}", filter.Key, filter.Value, Environment.NewLine);
                            }
                        }
                        return sb.ToString();
                    }
            }
            return null;
        }

        public List<IMsg> TransMessageFace(List<IMsg> oldmessages)
        {
            Regex r = new Regex(@"\[face,(?<faceid>\d*)\]", RegexOptions.IgnoreCase);
            List<IMsg> newmessages = new List<IMsg>();
            foreach (IMsg oldmessage in oldmessages)
            {
                if (oldmessage.MessageType != MsgType.TYPETEXT)
                {
                    newmessages.Add(oldmessage);
                    continue;
                }
                string strMsg = oldmessage.Message as string;
                if (strMsg != null)
                {
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            Bitmap bm;
                            try
                            {
                                bm = GetFace(QQ.T_TRANSFER_TABLE[m.Groups["faceid"].Value]);
                            }
                            catch (Exception) { bm = null; }
                            if (bm == null)
                            {
                                try
                                {
                                    string url = QQ.GetFaceUrl(m.Groups["faceid"]);
                                    Stream s = _qq.GetUrlStream(url);
                                    Bitmap nbm = Image.FromStream(s) as Bitmap;
                                    bm = nbm;
                                }
                                catch (Exception) { bm = null; }
                            }
                            if (bm == null)
                            {
                                bm = new Bitmap(10,10);
                            }
                            if (strMsg != null)
                            {
                                string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                                if (t[0].Length > 0)
                                {
                                    newmessages.Add(new MsgText(t[0]));
                                }
                                //newmessages.Add(new MsgImgUrl(bm));
                                strMsg = t.Length > 1 ? t[1] : "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(new MsgText(strMsg));
                        }
                    }
                }
            }
            return newmessages;
        }


        public List<IMsg> TransMessageCFace(List<IMsg> oldmessages, string uin, long msg_id)
        {
            Regex r = new Regex(@"\[cface,name:(?<name>[^\]]*)\r\nfile_id:(?<file_id>\d*)\r\nkey:(?<key>[^\]]*)\r\nserver:(?<server>[^\]]*)\]", RegexOptions.IgnoreCase);
            List<IMsg> newmessages = new List<IMsg>();
            foreach (IMsg oldmessage in oldmessages)
            {
                if (oldmessage.MessageType != MsgType.TYPETEXT)
                {
                    newmessages.Add(oldmessage);
                    continue;
                }
                string strMsg = oldmessage.Message as string;
                if (strMsg != null)
                {
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            Bitmap bm;
                            try
                            {
                                string url = m.Groups["name"].Value;
                                if (url.StartsWith("{"))
                                {
                                    url = _qq.GetCFaceUrl(url, m.Groups["file_id"].Value);
                                }
                                else
                                {
                                    url = _qq.GetCFace2Url(msg_id.ToString(), url, uin);
                                }
                                Stream s = _qq.GetUrlStream(url);
                                bm = (Bitmap)Image.FromStream(s);
                            }
                            catch (Exception)
                            {
                                bm = null;
                            }
                            if (strMsg != null)
                            {
                                string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                                if (t[0].Length > 0)
                                {
                                    newmessages.Add(new MsgText(t[0]));
                                }
                                if (bm == null)
                                {
                                    newmessages.Add(new MsgText(m.Value));
                                }
                                else
                                {
                                    //newmessages.Add(new MsgImgUrl(bm));
                                }
                                strMsg = t.Length > 1 ? t[1] : "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(new MsgText(strMsg));
                        }
                    }
                }
            }
            return newmessages;
        }


        public List<IMsg> TransMessageOffpic(List<IMsg> oldmessages, string uin)
        {
            Regex r = new Regex(@"\[offpic,success:1\r\nfile_path:(?<picpath>\/[^\]]*)\]", RegexOptions.IgnoreCase);
            List<IMsg> newmessages = new List<IMsg>();
            foreach (IMsg oldmessage in oldmessages)
            {
                if (oldmessage.MessageType != MsgType.TYPETEXT)
                {
                    newmessages.Add(oldmessage);
                    continue;
                }
                string strMsg = oldmessage.Message as string;
                if (strMsg != null)
                {
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            Bitmap bm;
                            try
                            {
                                Stream s = _qq.GetOffPic(m.Groups["picpath"].Value, uin);
                                bm = (Bitmap)Image.FromStream(s);
                            }
                            catch (Exception)
                            {
                                bm = null;
                            }
                            if (strMsg != null)
                            {
                                string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                                if (t[0].Length > 0)
                                {
                                    newmessages.Add(new MsgText(t[0]));
                                }
                                if (bm == null)
                                {
                                    newmessages.Add(new MsgText(m.Value));
                                }
                                else
                                {
                                    //newmessages.Add(new MsgImgUrl(bm));
                                }
                                strMsg = t.Length > 1 ? t[1] : "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(new MsgText(strMsg));
                        }
                    }
                }
            }
            return newmessages;
        }

        public List<IMsg> TransMessage(string msg, string uin, long msg_id)
        {
            if (msg == null)
                return null;
            List<IMsg> messages = new List<IMsg>();
            messages.Add(new MsgText(msg));
            return TransMessageOffpic(TransMessageCFace(TransMessageFace(messages), uin, msg_id), uin);
        }


        public static ArrayList TransSendFaceMessage(ArrayList oldmessages)
        {
            if (oldmessages == null || oldmessages.Count == 0)
                return oldmessages;
            Regex r = new Regex(@"\[face,(?<faceid>\d*)\]", RegexOptions.IgnoreCase);
            ArrayList newmessages = new ArrayList();
            foreach (var oldmessage in oldmessages)
            {
                if (oldmessage is string)
                {
                    string strMsg = oldmessage as string;
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                            if (t[0].Length > 0)
                            {
                                newmessages.Add(t[0]);
                            }
                            newmessages.Add(new ArrayList { "face", Convert.ToInt64(m.Groups["faceid"].Value) });
                            strMsg = t.Length > 1 ? t[1] : "";
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(strMsg);
                        }
                    }
                }
                else
                {
                    newmessages.Add(oldmessage);
                }
            }
            return newmessages;
        }

        public static ArrayList TransSendMessage(string msg)
        {
            if (msg == null)
                return null;
            ArrayList oldmessages = new ArrayList { msg };
            return TransSendFaceMessage(oldmessages);
        }

        public static Bitmap GetFace(int faceid)
        {
            var filePath = string.Format("{0}\\{1}.gif", FacePath, faceid);
            if (File.Exists(filePath))
            {
                return new Bitmap(filePath);
            }
            throw new FileNotFoundException(filePath);
        }
    }
}