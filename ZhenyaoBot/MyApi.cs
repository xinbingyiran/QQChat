using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ZhenyaoBot
{
    public class MyApi : TMessage
    {
        private Int32 _botid;
        private const Int32 defaultbotid = 728;
        private static CancellationTokenSource _cts;

        public bool _friendEnable;
        public bool _groupEnable;

        public override string Setting
        {
            get
            {
                return (Enabled ? "1" : "0") + (_friendEnable ? "1" : "0") + (_groupEnable ? "1" : "0") + _botid;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 3)
                {
                    Enabled = value[0] == '1';
                    _friendEnable = value[1] == '1';
                    _groupEnable = value[2] == '1';
                    _botid = defaultbotid;
                    if (!Int32.TryParse(value.Substring(3), out _botid))
                    {
                        _botid = defaultbotid;
                    }
                }
            }
        }

        public override string PluginName
        {
            get { return "真药机器人"; }
        }

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
        };

        public override Dictionary<string, string> Filters
        {
            get
            {
                return _filters;
            }
        }

        public MyApi()
        {
            _friendEnable = true;
            _groupEnable = true;
            _botid = defaultbotid;
        }


        private HttpWebResponse GetUrlResponse(string url, int timeout = 60000)
        {
            if (_cts == null)
            {
                _cts = new CancellationTokenSource();
            }
            _cts.Token.ThrowIfCancellationRequested();
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                myRequest.Method = "GET";
                myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                myRequest.AllowAutoRedirect = true;
                myRequest.KeepAlive = true;
                response = (HttpWebResponse)myRequest.GetResponse();
            });
            task.Start();
            bool wait = task.Wait(timeout, _cts.Token);
            if (wait)
                return response;
            throw new TimeoutException();
        }

        public string GetUrlText(string url, int timeout = 60000)
        {
            try
            {
                HttpWebResponse myResponse = GetUrlResponse(url, timeout);
                Stream newStream = myResponse.GetResponseStream();
                if (newStream != null)
                {
                    StreamReader reader = new StreamReader(newStream, Encoding.GetEncoding(myResponse.CharacterSet));
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        private HttpWebResponse GetPostResponse(string url, byte[] postData, int timeout = 60000)
        {
            if (_cts == null)
            {
                _cts = new CancellationTokenSource();
            }
            _cts.Token.ThrowIfCancellationRequested();
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                myRequest.ContentLength = postData.Length;
                using (var sw = myRequest.GetRequestStream())
                {
                    sw.Write(postData, 0, postData.Length);
                }
                response = (HttpWebResponse)myRequest.GetResponse();
            });
            task.Start();
            bool wait = task.Wait(timeout, _cts.Token);
            if (wait)
                return response;
            throw new TimeoutException();
        }

        public string PostUrlText(string url, byte[] postData, int timeout = 60000)
        {
            try
            {
                HttpWebResponse myResponse = GetPostResponse(url, postData, timeout);
                Stream newStream = myResponse.GetResponseStream();
                if (newStream != null)
                {
                    StreamReader reader = new StreamReader(newStream, Encoding.GetEncoding(myResponse.CharacterSet));
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }


        private static string spacialurl = "http://lover.zhenyao.net/chat-g.json";

        private static string spacialurl_post = "score=true&botid={0}&msg={1}&type=3";

        private string GetBotMessage(string msg)
        {
            string postd = string.Format(spacialurl_post, _botid, System.Web.HttpUtility.UrlEncode(msg));
            string rstr = PostUrlText(spacialurl, Encoding.Default.GetBytes(postd));
            if (rstr != null)
            {
                try
                {
                    System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                    Dictionary<string, object> dss = (Dictionary<string, object>)jss.Deserialize(rstr, typeof(Dictionary<string, object>));
                    System.Collections.ArrayList arr = dss["messages"] as System.Collections.ArrayList;
                    StringBuilder sb = new StringBuilder();
                    foreach (Dictionary<string, object> item in arr)
                    {
                        sb.AppendLine(item["message"].ToString());
                    }
                    return sb.ToString().Trim();
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public override string DealMessage(string messageType, Dictionary<string, object> info, string message)
        {
            if (messageType != MessageType.MessageFriend && messageType != MessageType.MessageGroup)
            {
                return null;
            }
            if (!Enabled)
                return null;
            if (messageType == MessageType.MessageFriend && !_friendEnable)
                return null;
            if (messageType == MessageType.MessageGroup && !_groupEnable)
                return null;
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            string rstr = null;
            try
            {
                rstr = GetBotMessage(message);
            }
            catch (Exception)
            {
                rstr = null;
            }
            if (rstr != null)
            {
                return rstr.Replace(@"<br/>", Environment.NewLine);
            }
            return null;
        }

        public override string AboutMessage
        {
            get
            {
                return "真药网机器人。\r\n信息来自 http://lover.zhenyao.net/ 。";
            }
        }

        public override void OnExited()
        {
            if (_cts != null)
            {
                _cts.Cancel(false);
                _cts = null;
            }
            base.OnExited();
        }
    }
}
