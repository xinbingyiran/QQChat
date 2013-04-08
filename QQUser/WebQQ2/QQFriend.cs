using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
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
        public string markname { get; set; }
        public string nick { get; set; }
        public long categories { get; set; }
        public string status { get; set; }

        public bool ShowMarkName { get; set; }

        public string LongName
        {
            get
            {
                if (this.nick == null && this.num == 0)
                {
                    return this.uin.ToString();
                }
                if (this.markname != null && ShowMarkName)
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
                if (this.nick == null && this.num == 0)
                {
                    return this.uin.ToString();
                }
                if (this.markname != null && ShowMarkName)
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

        public bool IsValid
        {
            get { return this.nick != null; }
        }

        public bool IsFull
        {
            get { return this.num != 0; }
        }

        public QQFriend()
        {
            ShowMarkName = true;
            status = "offline";
        }

    }
}
