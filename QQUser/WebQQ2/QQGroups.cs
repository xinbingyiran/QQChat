using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
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
            var member = GroupList.FirstOrDefault(ele => ele.Value.gid == gid).Value;
            //if (member == null)
            //{
            //    member = new QQGroup() { gid = gid,owner = null };
            //    GroupList.Add(gid, member);
            //}
            return member;
        }
    }
}
