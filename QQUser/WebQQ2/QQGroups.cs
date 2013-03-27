using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{
    public class QQGroups
    {

        public Dictionary<string, QQGroup> GroupList { get; set; }

        public QQGroups()
        {
            GroupList = new Dictionary<string, QQGroup>();
        }

        public void Add(QQGroup item)
        {
            GroupList.Add(item.gid.ToString(), item);
        }

        public void Clear()
        {
            GroupList.Clear();
        }
    }
}
