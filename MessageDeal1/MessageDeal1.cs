using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Windows.Forms;

namespace MessageDeal1
{
    public class MessageDeal1 : IMessageDeal
    {

        private bool _enabled = true;

        public string IName
        {
            get { return "示例1"; }
        }

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"设置","setting"},
            {"关于","about"},
        };

        public Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        public string DealFriendMessage(string message)
        {
            if (_enabled)
            {
                return message + "_用户示例";
            }
            return null;
        }

        public string DealGroupMessage(string message)
        {
            if (_enabled)
            {
                return message + "_群示例";
            }
            return null;
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "setting")
            {
                _enabled = !_enabled;
                MessageBox.Show("现在的状态是" + (_enabled ? "启用" : "停用"), "状态指示");
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这只是一个示例", "关于");
            }
        }

        public string StatusChanged(string newStatus)
        {
            if (_enabled)
            {
                QQStatus status = QQStatus.GetQQStatusByInternal(newStatus);
                if (status != null && status != QQStatus.StatusOffline)
                {
                    return string.Format("{0} 你好，你现在的状态是:{1}", TranslateMessage.UserNick, status.Status);
                }
            }
            return null;
        }

        public string Input()
        {
            if (_enabled)
            {
                return string.Format("{0} 你好，我正在等待你的输入。", TranslateMessage.UserNick);
            }
            return null;
        }
    }
}
