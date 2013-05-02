using System;
using System.Runtime;
using System.Collections.Generic;

namespace MessageDeal
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

    public interface IMessageDeal
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string PluginName { get; }
        /// <summary>
        /// 插件状态
        /// </summary>
        string AboutMessage { get; }
        /// <summary>
        /// 插件状态
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// 插件设置
        /// </summary>
        string Setting { get; set; }
        /// <summary>
        /// 获取相应的菜单项，格式为[显示内容-传递内容]
        /// </summary>
        Dictionary<string, string> Menus { get; }
        /// <summary>
        /// 获取相应的过滤信息，格式为[处理内容-格式说明]
        /// </summary>
        Dictionary<string, string> Filters { get; }
        /// <summary>
        /// 菜单处理程序
        /// </summary>
        /// <param name="menuName">菜单名称[传递内容]</param>
        /// <returns></returns>
        void MenuClicked(string menuName);
        /// <summary>
        /// 处理用户消息，并返回处理结果
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="info">消息内容</param>
        /// <param name="message">要处理的消息</param>
        /// <returns>如果不需要回应，只需设置为null即</returns>
        string DealMessage(string messageType,Dictionary<string, object> info, string message);

        /// <summary>
        /// 退出时发送此消息，进行最后保存，时间不要太长，否则可能引起反感
        /// </summary>
        void OnExited();

        string LastMessage { get; }

        event EventHandler<EventArgs> OnMessage;
    }

    public abstract class TMessage : IMessageDeal
    {

        public virtual string PluginName
        {
            get { return null; }
        }

        public virtual bool Enabled { get; set; }

        public virtual string Setting { get; set; }

        public virtual Dictionary<string, string> Menus
        {
            get { return null; }
        }

        public virtual Dictionary<string, string> Filters
        {
            get { return null; }
        }

        public virtual void MenuClicked(string menuName)
        {
            return;
        }

        public virtual void OnExited()
        {
            return;
        }


        public virtual string DealMessage(string messageType, Dictionary<string, object> info, string message)
        {
            return null;
        }

        public virtual string LastMessage
        {
            get;
            protected set;
        }

        public virtual event EventHandler<EventArgs> OnMessage;


        public virtual string AboutMessage
        {
            get;
            protected set;
        }
    }
}
