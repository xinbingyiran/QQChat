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

    public class QQStatus
    {
        public string Status { get; private set; }
        public string StatusInternal { get; private set; }
        private QQStatus(string status, string statusInternal)
        {
            Status = status;
            StatusInternal = statusInternal;
        }
        public static QQStatus StatusOnline = new QQStatus("我在线上", "online");
        public static QQStatus StatusCallme = new QQStatus("Q我吧", "callme");
        public static QQStatus StatusAway = new QQStatus("离开", "away");
        public static QQStatus StatusBusy = new QQStatus("忙碌", "busy");
        public static QQStatus StatusSilent = new QQStatus("请勿打扰", "silent");
        public static QQStatus StatusHidden = new QQStatus("隐身", "hidden");
        public static QQStatus StatusOffline = new QQStatus("离线", "offline");
        public static QQStatus[] AllStatus = new QQStatus[]
        {
            StatusOnline,
            StatusCallme,
            StatusAway,
            StatusBusy,
            StatusSilent,
            StatusHidden,
            StatusOffline
        };
        public static QQStatus GetQQStatusByInternal(string statusInternal)
        {
            foreach (var st in AllStatus)
            {
                if (st.StatusInternal == statusInternal)
                    return st;
            }
            return null;
        }
        public static QQStatus GetQQStatusByStatus(string status)
        {
            foreach (var st in AllStatus)
            {
                if (st.Status == status)
                    return st;
            }
            return null;
        }

        public override string ToString()
        {
            return Status;
        }
    }

    public interface IMessageDeal
    {
        string Setting { get; set; }
        /// <summary>
        /// 插件名称
        /// </summary>
        string IName { get; }
        /// <summary>
        /// 插件状态
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// 获取相应的菜单项，格式为[显示内容-传递内容]
        /// </summary>
        Dictionary<string, string> Menus { get; }
        /// <summary>
        /// 获取相应的过滤信息，格式为[处理内容-格式说明]
        /// </summary>
        Dictionary<string, string> Filters { get; }
        /// <summary>
        /// 处理用户消息，并返回处理结果
        /// </summary>
        /// <param name="info">用户信息UserName,UserNick,UserMarkName</param>
        /// <param name="message">要处理的消息</param>
        /// <returns>如果不需要回应，只需设置为null即</returns>
        string DealFriendMessage(Dictionary<string, object> info, string message);
        /// <summary>
        /// 处理群消息，并返回处理结果
        /// </summary>
        /// 
        /// <param name="info">用户信息GroupName,GroupNum,MemberNum,MemberNick,MemberCard</param>
        /// <param name="message">要处理的消息</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string DealGroupMessage(Dictionary<string, object> info, string message);
        /// <summary>
        /// 菜单处理程序
        /// </summary>
        /// <param name="menuName">菜单名称[传递内容]</param>
        /// <returns></returns>
        void MenuClicked(string menuName);
        /// <summary>
        /// 用户状态改变
        /// </summary>
        /// <param name="info">用户信息UserName,UserNick,UserMarkName</param>
        /// <param name="newStatus">新状态[传递内容]</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string StatusChanged(Dictionary<string, object> info, string newStatus);
        /// <summary>
        /// 用户正在输入
        /// </summary>
        /// <param name="info">用户信息UserName,UserNick,UserMarkName</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string Input(Dictionary<string, object> info);
    }
}
