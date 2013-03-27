using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{
    public class QQGroup
    {
        public long flag { get; set; }
        public string name { get; set; }
        public long gid { get; set; }
        public long code { get; set; }
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
                return string.Format("{0}[{1}]", name, code);
            }
        }
    }
}
