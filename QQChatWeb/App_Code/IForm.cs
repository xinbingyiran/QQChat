using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WebQQ2.WebQQ2;

namespace QQChatWeb.App_Code
{
    public interface IMsg
    {
        MsgType MessageType { get; }
        object Message { get; }
        Color? MessageColor { get; set; }
    }
    public enum MsgType
    {
        TYPETEXT = 1,
        TYPEIMGURL = 2,
    }

    public class MsgText : IMsg
    {
        public MsgType MessageType
        {
            get { return MsgType.TYPETEXT; }
        }

        public object Message
        {
            get;
            private set;
        }

        public Color? MessageColor
        {
            get;
            set;
        }

        public MsgText(string message)
        {
            Message = message;
        }
    }

    public class MsgImgUrl : IMsg
    {

        public MsgType MessageType
        {
            get { return MsgType.TYPEIMGURL; }
        }

        public object Message
        {
            get;
            private set;
        }

        public Color? MessageColor
        {
            get;
            set;
        }

        public MsgImgUrl(string url)
        {
            Message = url;
        }
    }
    public interface IForm
    {
        /// <summary>
        /// 标识
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 标识
        /// </summary>
        bool HasMessage { get; }
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
        /// <param name="sender">发送者</param>
        /// <param name="time">发送时间</param>
        /// <param name="messages">收到的消息内容</param>
        void AppendMessage(object sender, DateTime time, params IMsg[] messages);
    }
}
