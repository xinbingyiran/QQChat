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
using WebQQ2.Extends;
using System.Threading.Tasks;
using System.Threading;

namespace WebQQ2.WebQQ2
{

    public class QQUser
    {
        public string Uin { get; internal set; }
        public string ClientID { get; internal set; }
        public string PtWebQQ { get; internal set; }
        public string VfWebQQ { get; internal set; }
        public string VerifySession { get; internal set; }
        public string skey { get; internal set; }
        public string GTK { get; internal set; }
        public string PsessionID { get; internal set; }
        public DateTime? LoginTime { get; internal set; }
        public QQGroups QQGroups { get; internal set; }
        public QQFriends QQFriends { get; internal set; }
        public string QQNum { get; internal set; }
        public string QQName { get; internal set; }
        public string Status { get; internal set; }

        public void OnCreated()
        {
            Status = "offline";
            QQFriends = new QQFriends();
            QQGroups = new QQGroups();
            ClientID = GenerateClientID();
        }

        public QQUser()
        {
            this.QQNum = "0";
            OnCreated();
        }

        public QQUser(string qqnum)
        {
            this.QQNum = qqnum;
            OnCreated();
        }

        public bool IsPreLoged
        {
            get
            {
                return (GTK != null && GTK.Length > 0);
            }
        }

        private static string GenerateClientID()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(0, 99) + "" + QQHelper.GetTime() / 1000000;
        }

        public QQGroup GetUserGroup(long gid)
        {
            return QQGroups.GetQQGroup(gid);
        }

        public QQFriend GetUserFriend(long uin, bool canAddSess)
        {
            return QQFriends.GetQQFriend(uin, canAddSess);
        }

        public QQFriend GetUserSess(long uin)
        {
            return QQFriends.GetQQSess(uin);
        }
    }
}
