using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageDeal
{

    public class TranslateMessage
    {
        public static readonly string UserName = "[name]";
        public static readonly string UserNick = "[nick]";
        public static readonly string UserMarkName = "[mark]";
        public static readonly string UserNum = "[num]";
        public static readonly string ShortName = "[sname]";
        public static readonly string LongName = "[lname]";
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
        /// 获取相应的菜单项，格式为[显示内容-传递内容]
        /// </summary>
        Dictionary<string, string> Menus { get; }
        /// <summary>
        /// 处理用户消息，并返回处理结果
        /// </summary>
        /// <param name="message">要处理的消息</param>
        /// <returns>如果不需要回应，只需设置为null即</returns>
        string DealFriendMessage(string message);
        /// <summary>
        /// 处理群消息，并返回处理结果
        /// </summary>
        /// <param name="message">要处理的消息</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string DealGroupMessage(string message);
        /// <summary>
        /// 菜单处理程序
        /// </summary>
        /// <param name="menuName">菜单名称[传递内容]</param>
        /// <returns></returns>
        void MenuClicked(string menuName);
        /// <summary>
        /// 用户状态改变
        /// </summary>
        /// <param name="newStatus">新状态[传递内容]</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string StatusChanged(string newStatus);
        /// <summary>
        /// 用户正在输入
        /// </summary>
        /// <param name="newStatus">新状态[传递内容]</param>
        /// <returns>如果不需要回应，只需设置为null即可</returns>
        string Input();
    }
}
