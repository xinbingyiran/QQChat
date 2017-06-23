using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat
{
    public class QQGroups
    {

        public Dictionary<long, QQGroup> GroupList { get; set; }

        public QQGroups()
        {
            GroupList = new Dictionary<long, QQGroup>();
        }

        public void Add(QQGroup item)
        {
            GroupList.Add(item.gid, item);
        }

        public void Clear()
        {
            GroupList.Clear();
        }

        public QQGroup GetQQGroup(long gid)
        {
            if (GroupList.ContainsKey(gid))
            {
                return GroupList[gid];
            }
            //    member = new QQGroup() { gid = gid,owner = null };
            //    GroupList.Add(gid, member);
            return null;
        }
    }
}
