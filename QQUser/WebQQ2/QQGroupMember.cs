using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{
    public class QQGroupMember
    {
        public long uin { get; set; }
        public long num { get; set; }
        public string nick { get; set; }
        public string card { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string gender { get; set; }
        public long client_type { get; set; }
        public long stat { get; set; }
        public long mflag { get; set; }
        public long is_vip { get; set; }
        public long vip_level { get; set; }
        public bool IsValid
        {
            get
            {
                return this.nick != null;
            }
        }

        public QQFriend ToQQFriend()
        {
            return new QQFriend()
            {
                uin = this.uin,
                nick = this.nick,
                vip_level = this.vip_level,
                is_vip = this.is_vip,
            };
        }
    }
}
