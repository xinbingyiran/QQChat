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

        public string DealMessage(string message, out bool isDealed)
        {
            isDealed = _enabled;
            if (_enabled)
            {
                return message + "_示例";
            }
            return message;
        }

        public bool OnMenu(string menuName)
        {
            if (menuName == "setting")
            {
                _enabled = !_enabled;
                MessageBox.Show("现在的状态是" + (_enabled?"启用":"停用"),"状态指示");
                return true;
            }
            else if (menuName == "about")
            {
                MessageBox.Show("这只是一个示例","关于");
                return true;
            }
            return false;
        }
    }
}
