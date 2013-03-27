using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{

    public class QQFriends
    {
        public Dictionary<string, QQFriendDir> GroupList { get; set; }

        public Dictionary<string, QQFriend> FriendList { get; set; }

        public QQFriends()
        {
            GroupList = new Dictionary<string, QQFriendDir>();
            FriendList = new Dictionary<string, QQFriend>();
        }

        public void Clear()
        {
            FriendList.Clear();
            GroupList.Clear();
        }

        public void Add(QQFriendDir item)
        {
            GroupList.Add(item.index.ToString(), item);
        }

        public void Add(QQFriend item)
        {
            FriendList.Add(item.uin.ToString(), item);
        }
    }
}
