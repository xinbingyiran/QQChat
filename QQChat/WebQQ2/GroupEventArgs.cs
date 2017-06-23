using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QQChat;

namespace QQChat
{
    public class GroupEventArgs : EventArgs
    {
        private QQGroup _group = null;
        private QQGroupMember _member = null;
        private long _msg_id = 0;
        private DateTime _time;
        private Dictionary<string, object> _msgs = null;

        private MessageEventType _mtype = MessageEventType.MESSAGE_UNKNOW;

        public MessageEventType Mtype
        {
            get { return _mtype; }
        }

        public QQGroup Group
        {
            get { return _group; }
        }

        public QQGroupMember Member
        {
            get { return _member; }
        }

        public long Msg_id
        {
            get { return _msg_id; }
        }

        public DateTime Time
        {
            get { return _time; }
        }

        public Dictionary<string, object> Msgs
        {
            get { return _msgs; }
        }

        public string MsgContent
        {
            get
            {
                //return QQHelper.ToJson(Msgs);
                if (_msgs.Count == 1 && _msgs.Keys.Contains("content"))
                {
                    ArrayList content = _msgs["content"] as ArrayList;
                    if (content == null)
                        return null;
                    var contentList = new List<string>(content.Count - 1);
                    for (int i = 1; i < content.Count; i++)
                    {
                        contentList.Add(GetSimpleMsg(content[i]));
                    }
                    return string.Join(System.Environment.NewLine, contentList);
                }
                return null;
            }
        }
        public string[] MsgContents
        {
            get
            {
                var contentList = new List<string>();
                if (_msgs.Count == 1 && _msgs.Keys.Contains("content"))
                {
                    ArrayList content = _msgs["content"] as ArrayList;
                    if (content == null)
                        return null;
                    for (int i = 1; i < content.Count; i++)
                    {
                        contentList.Add(GetSimpleMsg(content[i]));
                    }
                }
                return contentList.ToArray();
            }
        }

        private string GetSimpleMsg(object o)
        {
            if (o is Dictionary<string, object>)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, object> p in o as Dictionary<string, object>)
                {
                    sb.AppendLine(string.Format("{0}:{1}", p.Key, GetSimpleMsg(p.Value)));
                }
                return sb.ToString().TrimEnd();
            }
            if (o is ArrayList)
            {
                StringBuilder sb = new StringBuilder("[");
                foreach (object p in o as ArrayList)
                {
                    sb.Append(GetSimpleMsg(p));
                    sb.Append(",");
                }
                if (sb.Length > 1)
                {
                    sb.Length--;
                }
                sb.Append("]");
                return sb.ToString().TrimEnd();
            }
            return o.ToString();
        }

        internal GroupEventArgs(QQGroup group, QQGroupMember member, long msgid, DateTime time,MessageEventType type, Dictionary<string, object> msgs)
        {
            this._group = group;
            this._msg_id = msgid;
            this._member = member;
            this._time = time;
            this._mtype = type;
            this._msgs = msgs;
        }

        internal GroupEventArgs(QQGroup group, QQGroupMember member, long msgid, DateTime time, MessageEventType type, string singleMsg)
        {
            this._group = group;
            this._msg_id = msgid;
            this._member = member;
            this._time = time;
            this._mtype = type;
            this._msgs = new Dictionary<string, object>() { { "message", singleMsg } };
        }
    }

}
