using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat
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
        public string mobile { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public long face { get; set; }
        public string birthday { get; set; }
        public string phone { get; set; }
        public string occupation { get; set; }
        public long allow { get; set; }
        public string college { get; set; }
        public long blood { get; set; }
        public long constel { get; set; }
        public string homepage { get; set; }
        public string personal { get; set; }
        public long shengxiao { get; set; }


        public string LongName
        {
            get
            {
                if (this.card != null)
                {
                    return string.Format("{0}[{1}]({2})", this.nick, this.card, this.num);
                }
                return string.Format("{0}({1})", this.nick, this.num);
            }
        }

        public string ShortName
        {
            get
            {
                if (this.card != null)
                {
                    return string.Format("{0}[{1}]", this.nick, this.card);
                }
                return string.Format("{0}", this.nick);
            }
        }

        public string Name
        {
            get
            {
                if (this.card != null && this.card.Length > 0)
                {
                    return string.Format("{0}", this.card);
                }
                if (this.nick != null && this.nick.Length > 0)
                {
                    return string.Format("{0}", this.nick);
                }
                if (this.num != 0)
                {
                    return string.Format("QQUser:{0}", this.num);
                }
                return string.Format("UIN:{0}", this.uin);
            }
        }

        public bool IsFull
        {
            get { return this.num != 0; }
        }
    }
}
