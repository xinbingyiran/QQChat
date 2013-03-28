using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebQQ2.WebQQ2
{
    public class GroupEventArgs : EventArgs
    {
        private QQGroup _group = null;
        private QQGroupMember _member = null;
        private long _msg_id = 0;
        private DateTime? _time = null;
        private Dictionary<string, object> _msgs = null;

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

        public DateTime? Time
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
                if (_msgs.Count == 1 && _msgs.Keys.Contains("content"))
                {
                    ArrayList content = _msgs["content"] as ArrayList;
                    if (content.Count == 2)
                    {
                        return GetSimpleMsg(content[1]);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 1; i < content.Count; i++)
                        {
                            sb.Append(GetSimpleMsg(content[i]));
                        }
                        return sb.ToString();
                    }
                }
                return null;
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

        internal GroupEventArgs(QQGroup group, QQGroupMember member, long msgid, DateTime? time, Dictionary<string, object> msgs)
        {
            this._group = group;
            this._msg_id = msgid;
            this._member = member;
            this._time = time;
            this._msgs = msgs;
        }

        internal GroupEventArgs(QQGroup group, QQGroupMember member, long msgid, DateTime? time, string singleMsg)
        {
            this._group = group;
            this._msg_id = msgid;
            this._member = member;
            this._time = time;
            this._msgs = new Dictionary<string, object>() { { "message", singleMsg } };
        }
    }

}
