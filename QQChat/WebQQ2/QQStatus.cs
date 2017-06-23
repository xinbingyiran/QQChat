using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat
{
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
}
