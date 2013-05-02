using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat.Classes
{
    public class TranslateMessageUser
    {
        public static readonly KeyValuePair<string, string> UserName = new KeyValuePair<string, string>("[fname]", "用户名");
        public static readonly KeyValuePair<string, string> UserNick = new KeyValuePair<string, string>("[fnick]", "用户昵称");
        public static readonly KeyValuePair<string, string> UserMarkName = new KeyValuePair<string, string>("[fmark]", "用户备注");
        public static readonly KeyValuePair<string, string> UserNum = new KeyValuePair<string, string>("[fnum]", "用户号码");
        public static readonly KeyValuePair<string, string> UserShortName = new KeyValuePair<string, string>("[fsname]", "短用户名");
        public static readonly KeyValuePair<string, string> UserLongName = new KeyValuePair<string, string>("[flname]", "长用户名");
        public static readonly List<KeyValuePair<string, string>> AllMessage = new List<KeyValuePair<string, string>>
        {
            UserName,
            UserNick,
            UserMarkName,
            UserNum,
            UserShortName,
            UserLongName
        };
    }
    public class TranslateMessageGroup
    {
        public static readonly KeyValuePair<string, string> GroupName = new KeyValuePair<string, string>("[gname]", "群名称");
        public static readonly KeyValuePair<string, string> GroupNum = new KeyValuePair<string, string>("[gnum]", "群号码");
        public static readonly KeyValuePair<string, string> GroupShortName = new KeyValuePair<string, string>("[gsname]", "短群名称");
        public static readonly KeyValuePair<string, string> GroupLongName = new KeyValuePair<string, string>("[glname]", "长群名称");
        public static readonly KeyValuePair<string, string> GroupMemo = new KeyValuePair<string, string>("[gmemo]", "群规");
        public static readonly KeyValuePair<string, string> MemberNum = new KeyValuePair<string, string>("[mnum]", "发言号码");
        public static readonly KeyValuePair<string, string> MemberNick = new KeyValuePair<string, string>("[mnick]", "发言昵称");
        public static readonly KeyValuePair<string, string> MemberCard = new KeyValuePair<string, string>("[mcard]", "发言名片");
        public static readonly List<KeyValuePair<string, string>> AllMessage = new List<KeyValuePair<string, string>>
        {
            GroupName,
            GroupNum,
            GroupShortName,
            GroupLongName,
            GroupMemo,
            MemberNum,
            MemberNick,
            MemberCard
        };
    }

    public class MessageType
    {
        private MessageType() { }
        public static string MessageFriend = "friend";
        public static string MessageGroup = "group";
        public static string MessageStatus = "status";
        public static string MessageInput = "input";
        public static string MessageMember = "member";
        public static string MessageAdmin = "admin";
        public static string[] AllMessage = new string[]
        {
            MessageFriend,
            MessageGroup,
            MessageStatus,
            MessageInput,
            MessageMember,
            MessageAdmin,
        };
    }
}
