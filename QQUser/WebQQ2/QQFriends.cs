using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{

    public class QQFriends
    {
        public Dictionary<long, QQFriendDir> GroupList { get; set; }

        public Dictionary<long, QQFriend> FriendList { get; set; }

        public Dictionary<long, QQFriend> SessList { get; set; }

        public QQFriends()
        {
            GroupList = new Dictionary<long, QQFriendDir>();
            FriendList = new Dictionary<long, QQFriend>();
            SessList = new Dictionary<long, QQFriend>();
        }

        public void Clear()
        {
            SessList.Clear();
            FriendList.Clear();
            GroupList.Clear();
        }

        public void Add(QQFriendDir item)
        {
            GroupList.Add(item.index, item);
        }

        public void Add(QQFriend item)
        {
            FriendList.Add(item.uin, item);
        }

        public void AddSess(QQFriend item)
        {
            SessList.Add(item.uin, item);
        }

        public QQFriend GetQQFriend(long uin, bool canAddSess)
        {
            if (FriendList.ContainsKey(uin))
            {
                return FriendList[uin];
            }
            if (canAddSess)
            {
                var member = new QQFriend() { categories = -1, uin = uin };
                FriendList.Add(uin, member);
                return member;
            }
            return null;
        }

        public QQFriend GetQQSess(long uin)
        {
            if (SessList.ContainsKey(uin))
            {
                return SessList[uin];
            }
            var member = new QQFriend() { categories = 99999999, uin = uin };
            SessList.Add(uin, member);
            return member;
        }
    }
}
