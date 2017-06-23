using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat
{
    public class QQFriend
    {
        public object tag { get; set; }
        public long num { get; set; }
        public long uin { get; set; }
        public long flag_friends { get; set; }
        public long is_vip { get; set; }
        public long vip_level { get; set; }
        public long face { get; set; }
        public long flag_info { get; set; }
        public long client_type { get; set; }
        public long shengxiao { get; set; }
        public long blood { get; set; }
        public long constel { get; set; }
        public long stat { get; set; }
        public long allow { get; set; }
        public string birthday { get; set; }
        public string occupation { get; set; }
        public string personal { get; set; }
        public string homepage { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string phone { get; set; }
        public string college { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string markname { get; set; }
        public string nick { get; set; }
        public string longnick { get; set; }
        public string gender { get; set; }
        public long categories { get; set; }
        public string status { get; set; }
        public string group_sig { get; set; }
        public long id { get; set; }
        public string token { get; set; }

        public string LongName
        {
            get
            {
                if (this.markname != null)
                {
                    return string.Format("{0}[{1}]({2})", this.nick, this.markname, this.num);
                }
                return string.Format("{0}({1})", this.nick, this.num);
            }
        }

        public string ShortName
        {
            get
            {
                if (this.markname != null)
                {
                    return string.Format("{0}[{1}]", this.nick, this.markname);
                }
                return string.Format("{0}", this.nick);
            }
        }

        public string LongNameWithStatus
        {
            get
            {
                return string.Format("{0} - {1}", LongName, status);
            }
        }

        public string Name
        {
            get
            {
                if (this.markname != null && this.markname.Length > 0)
                {
                    return string.Format("{0}", this.markname);
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

        public QQFriend()
        {
            status = "offline";
        }

    }
}
