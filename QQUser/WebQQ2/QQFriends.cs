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

        public QQFriends()
        {
            GroupList = new Dictionary<long, QQFriendDir>();
            FriendList = new Dictionary<long, QQFriend>();
        }

        public void Clear()
        {
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

        public QQFriend GetQQFriend(long uin)
        {
            var member = FriendList.FirstOrDefault(ele => ele.Value.uin == uin).Value;
            if (member == null)
            {
                member = new QQFriend() { uin = uin };
                FriendList.Add(uin,member);
            }
            return member;
        }
    }
}
