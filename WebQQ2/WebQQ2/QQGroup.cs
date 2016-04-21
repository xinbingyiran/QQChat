using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{
    public class QQGroup
    {
        public long flag { get; set; }
        public long num { get; set; }
        public string name { get; set; }
        public long gid { get; set; }
        public long code { get; set; }
        public string memo { get; set; }
        public long face { get; set; }
        public long _class { get; set; }
        public string fingermemo { get; set; }
        public long createtime { get; set; }
        public long level { get; set; }
        public long option { get; set; }
        public QQGroupMember owner { get; set; }
        public Dictionary<long, QQGroupMember> leaders { get; set; }
        public Dictionary<long, QQGroupMember> members { get; set; }
        public Dictionary<long, QQGroupMember> allMembers { get; set; }

        public QQGroup()
        {
            allMembers = new Dictionary<long, QQGroupMember>();
            leaders = new Dictionary<long, QQGroupMember>();
            members = new Dictionary<long, QQGroupMember>();
        }

        public string ShortName
        {
            get
            {
                return string.Format("{0}", name);
            }
        }

        public string LongName
        {
            get
            {
                return string.Format("{0}[{1}]", name, num);
            }
        }

        public QQGroupMember GetGroupMember(long uin)
        {
            if (allMembers.ContainsKey(uin))
            {
                return allMembers[uin];
            }
            else if (allMembers.Count == 0)
            {
                return null;
            }
            QQGroupMember member = new QQGroupMember() { uin = uin };
            allMembers.Add(uin, member);
            return member;
        }

        public void Clear()
        {
            allMembers.Clear();
            members.Clear();
            leaders.Clear();
            owner = null;
        }
    }
}
