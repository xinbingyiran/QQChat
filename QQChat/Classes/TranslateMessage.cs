using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat.Classes
{
    public class TranslateMessageUser
    {
        public static readonly string UserName = "[fname]";
        public static readonly string UserNick = "[fnick]";
        public static readonly string UserMarkName = "[fmark]";
        public static readonly string UserNum = "[fnum]";
        public static readonly string UserShortName = "[fsname]";
        public static readonly string UserLongName = "[flname]";
    }
    public class TranslateMessageGroup
    {
        public static readonly string GroupName = "[gname]";
        public static readonly string GroupNum = "[gnum]";
        public static readonly string GroupShortName = "[gsname]";
        public static readonly string GroupLongName = "[glname]";
        public static readonly string GroupMemo = "[gmemo]";
        public static readonly string MemberNum = "[mnum]";
        public static readonly string MemberNick = "[mnick]";
        public static readonly string MemberCard = "[mcard]";
    }
}
