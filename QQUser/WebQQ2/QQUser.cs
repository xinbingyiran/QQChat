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
                return (PtWebQQ != null && PtWebQQ.Length > 0);
            }
        }

        private static string GenerateClientID()
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(0, 99) + "" + QQHelper.GetTime() / 1000000;
        }

        public QQGroup GetUserGroup(string gid)
        {
            return QQGroups.GroupList.SingleOrDefault(ele => ele.Key == gid).Value;
        }

        public QQFriend GetUserFriend(string uin)
        {
            var i = QQFriends.FriendList.SingleOrDefault(ele => ele.Key == uin);
            //if (i.Value == null)
            //{
            //    try
            //    {
            //        return new QQFriend() { uin = Convert.ToInt64(uin) };
            //    }
            //    catch (Exception)
            //    {
            //        return null;
            //    }
            //}
            return i.Value;
        }
    }
}
