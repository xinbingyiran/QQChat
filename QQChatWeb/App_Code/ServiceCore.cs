﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebQQ2.WebQQ2;

namespace QQChatWeb.App_Code
{
    public class ServiceCore
    {
        public static ServiceCore Instance = new ServiceCore();
        private ServiceCore()
        { }

        static ServiceCore()
        {
            Instance._qqs = new Dictionary<string, QQClient>();
            Instance._qqbindings = new Dictionary<string, string>();
        }

        private Dictionary<string, QQClient> _qqs;
        private Dictionary<string, string> _qqbindings;

        public QQClient[] GetQQs()
        {
            return _qqs.Values.ToArray();
        }

        public bool AddQQ(string session, QQ qq)
        {
            var qqid = qq.User.QQNum;
            if (_qqbindings.ContainsKey(qqid))
            {
                var oldsession = _qqbindings[qqid];
                if (oldsession != session)
                {
                    _qqs.Remove(session);
                }
            }
            else
            {
                _qqbindings.Add(qqid, session);
            }
            if (Instance._qqs.ContainsKey(session))
            {
                _qqs[session] = new QQClient { Client = qq };
            }
            else
            {
                _qqs.Add(session, new QQClient { Client = qq });
            }
            return true;
        }

        public QQ GetQQ(string session)
        {
            if (_qqs.ContainsKey(session))
            {
                return _qqs[session].Client;
            }
            else
            {
                return null;
            }
        }

        public bool ChangeSession(string qqnum, string session)
        {
            if (_qqbindings.ContainsKey(qqnum) && _qqbindings[qqnum] != session)
            {
                var oldsession = _qqbindings[qqnum];
                _qqbindings[qqnum] = session;
                var oldqq = _qqs[oldsession];
                _qqs.Remove(oldsession);
                if (_qqbindings.ContainsKey(oldqq.Client.User.QQNum))
                {
                    _qqbindings.Remove(oldqq.Client.User.QQNum);
                }
                _qqs.Add(session, oldqq);
                return true;
            }
            return false;
        }
    }
}