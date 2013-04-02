using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebQQ2.WebQQ2;

namespace QQChat.Classes
{
    public interface ChatFormMethod
    {
        /// <summary>
        /// 标识
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 更新窗口
        /// </summary>
        void UpdateTitle();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">要发送的消息内容</param>
        void SendMessage(string message);
        /// <summary>
        /// 收到消息并添加到窗口
        /// </summary>
        /// <param name="message">收到的消息内容</param>
        void AppendMessage(string message,object extend,DateTime time);
    }
}
