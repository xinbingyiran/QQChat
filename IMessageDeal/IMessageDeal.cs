using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageDeal
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
        /// <param name="info">用户信息{"num":用户号码,"nick":"用户名","mark":"标志名"}</param>
        /// <param name="message">要处理的消息</param>
        /// <returns>如果不需要回应，只需设置为null即</returns>
        string DealFriendMessage(Dictionary<string,object> info,string message);
        /// <summary>
        /// 处理群消息，并返回处理结果
        /// </summary>
        /// 
        /// <param name="info">用户信息{"gnum":群号码,"gname":"群名称","fnum":用户号码,"fnick":"用户名","fcard":用户名片}</param>
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
        /// <param name="info">用户信息{"num":用户号码,"nick":"用户名","mark":"标志名"}</param>
        /// <param name="newStatus">新状态[传递内容]</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string StatusChanged(Dictionary<string, object> info, string newStatus);
        /// <summary>
        /// 用户正在输入
        /// </summary>
        /// <param name="info">用户信息{"num":用户号码,"nick":"用户名","mark":"标志名"}</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string Input(Dictionary<string, object> info);
    }
}
