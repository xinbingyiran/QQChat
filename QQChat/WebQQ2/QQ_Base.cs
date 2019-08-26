using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QQChat
{
    public class QQ_Base
    {
        protected Random _random;
        protected CookieContainer _cookiecontainer;
        protected HttpHelper _helper;
        protected QQUser _user;
        protected string _baseRefer = "https://qun.qq.com/member.html";
        public QQUser User => _user;

        private static readonly string qq_qun_myinfo = "https://qun.qq.com/cgi-bin/qunwelcome/myinfo?callback=_&bkn={0}";
        private static readonly string qq_qun_friend = "https://qun.qq.com/cgi-bin/qun_mgr/get_friend_list";
        private static readonly string qq_qun_group = "https://qun.qq.com/cgi-bin/qun_mgr/get_group_list";
        private static readonly string qq_qun_member = "https://qun.qq.com/cgi-bin/qun_mgr/search_group_members";



        protected virtual void OnInit()
        {
            _random = new Random();
            _cookiecontainer = new CookieContainer();
            _helper = new HttpHelper(_cookiecontainer);
        }

        public QQ_Base()
        {
            this._user = new QQUser();
            this.OnInit();
        }

        #region QuickOperation


        public void VisitUrl(string url, string refer = null, int timeout = 60000)
        {
            _helper.GetNoRedirectResponse(url, refer ?? _baseRefer, timeout);
        }

        public Dictionary<string, object> GetMyInfo()
        {
            var furl = string.Format(qq_qun_myinfo, _user.GTK);
            string fresult = _helper.GetUrlText(furl, null, _baseRefer);
            var dict = QQHelper.FromJson<Dictionary<string, object>>(fresult);
            if ((int)dict["retcode"] == 0)
            {
                var data = dict["data"] as Dictionary<string, object>;
                if (data != null)
                {
                    _user.Uin = data["uin"].ToString();
                    _user.QQNum = data["uin"].ToString();
                    _user.QQName = (string)data["nickName"];
                }
            }
            return dict;
        }

        public Dictionary<string, object> GetFriendInfoFromQun()
        {
            var furl = qq_qun_friend;
            string para = "bkn=" + _user.GTK;
            string fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            return QQHelper.FromJson<Dictionary<string, object>>(fresult);
        }
        public Dictionary<string, object> GetGroupInfoFromQun()
        {
            var furl = qq_qun_group;
            string para = "bkn=" + _user.GTK;
            string fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            return QQHelper.FromJson<Dictionary<string, object>>(fresult);
        }
        public Dictionary<string, object> GetMemberInfoFromQun(long gcode, int st, int end)
        {
            var furl = qq_qun_member;
            string para = "gc=" + gcode + "&st=" + st + "&end=" + end + "&sort=0&bkn=" + _user.GTK;
            string fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            var dict = QQHelper.FromJson<Dictionary<string, object>>(fresult);
            return dict;
        }
        #endregion

        public void AnylizeCookie(string cookie)
        {
            var cks = cookie.Split(new[] { ';' }, StringSplitOptions.None);
            foreach (var ck in cks)
            {
                var kv = ck.Trim().Split(new[] { '=' }, StringSplitOptions.None);
                if (kv.Length == 2)
                {
                    if (kv[0] == "skey")
                    {
                        _user.skey = kv[1];
                        _user.GTK = QQHelper.getGTK(_user.skey);
                    }
                    else if (kv[0] == "uin")
                    {
                        var uin = kv[1].TrimStart(new[] { 'o', '0' });
                        _user.QQNum = uin;
                        _user.Uin = uin;
                    }
                    else if (kv[0] == "ptnick_" + _user.QQNum)
                    {
                        var utf8name = kv[1];
                        if (!string.IsNullOrWhiteSpace(utf8name))
                        {
                            if (utf8name.Length % 2 == 0)
                            {
                                var bytes = new byte[utf8name.Length / 2];
                                for (int i = 0; i < utf8name.Length; i += 2)
                                {
                                    bytes[i / 2] = byte.Parse(utf8name.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                                }
                                _user.QQName = Encoding.UTF8.GetString(bytes);
                            }
                        }
                    }
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "qq.com"));
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "qun.qq.com"));
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "ptlogin2.qq.com"));
                }
            }
        }
    }
}
