using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageDeal;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace WebApi
{
    public class MyWebApi:IMessageDeal
    {

        bool _enabled = false;

        public string IName
        {
            get { return "Web信息处理"; }
        }
        
            //new cxitem("@ip","IP地址","@ip ip地址","http://api.liqwei.com/location/?ip={0}"),
            //new cxitem("@mo","手机号码","@mo 手机号码","http://api.liqwei.com/location/?mobile={0}"),
            //new cxitem("@tq","天气预报","@tq 城市名","http://api.liqwei.com/weather/?city={0}"),
            //new cxitem("@b6e","Base64加密","@b6e 字符串","http://api.liqwei.com/security/?base64encode={0}"),
            //new cxitem("@b6d","Base64解密","@b6d 加密字符串","http://api.liqwei.com/security/?base64decode={0}"),

        private static readonly Dictionary<string, string> _menus = new Dictionary<string, string>
        {
            {"状态","status"}
        };

        public Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private static readonly Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"@ip ip地址","IP地址"},
            {"@mo/手机 手机号码","手机号码"},
            {"@tq/天气 城市名","天气预报"},
            {"@b6e 字符串","Base64加密"},
            {"@b6d 字符串","Base64解密"},
            {"@i 对话","与机器人对话"},
        };

        public Dictionary<string, string> Filters
        {
            get { return _filters; }
        }


        private HttpWebResponse GetUrlResponse(string url, int timeout = 60000)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                token.ThrowIfCancellationRequested();
                HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                myRequest.Method = "GET";
                myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                myRequest.AllowAutoRedirect = true;
                myRequest.KeepAlive = true;
                response = (HttpWebResponse)myRequest.GetResponse();
            }, token);
            task.Start();
            bool wait = task.Wait(timeout);
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
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                token.ThrowIfCancellationRequested();
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
            }, token);
            task.Start();
            bool wait = task.Wait(timeout);
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

        private static string spacialurl_post = "score=true&botid=1366&msg={0}&type=3";

        private string GetBotMessage(string msg)
        {
            string postd = string.Format(spacialurl_post, System.Web.HttpUtility.UrlEncode(msg));
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

        private string DealAllMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            if (message.Length < 2 || message[0] != '@')
                return null;
            message = message.Substring(1);
            string[] substring = message.Split(new char[]{' '},StringSplitOptions.None);
            string rstr = null;
            try
            {
                switch (substring[0])
                {
                    //{"ip","http://api.liqwei.com/location/?ip={0}"},
                    //{"mo","http://api.liqwei.com/location/?mobile={0}"},
                    //{"tq","http://api.liqwei.com/weather/?city={0}"},
                    //{"b6e","http://api.liqwei.com/security/?base64encode={0}"},
                    //{"b6d","http://api.liqwei.com/security/?base64decode={0}"},
                    case "ip":
                        rstr = GetUrlText(string.Format("http://api.liqwei.com/location/?ip={0}",
                            System.Web.HttpUtility.UrlEncode(substring[1], Encoding.GetEncoding("GBK"))));
                        break;
                    case "mo":
                    case "手机":
                        rstr = GetUrlText(string.Format("http://api.liqwei.com/location/?mobile={0}",
                        System.Web.HttpUtility.UrlEncode(substring[1], Encoding.GetEncoding("GBK"))));
                        break;
                    case "tq":
                    case "天气":
                        rstr = GetUrlText(string.Format("http://api.liqwei.com/weather/?city={0}",
                        System.Web.HttpUtility.UrlEncode(substring[1], Encoding.GetEncoding("GBK"))));
                        break;
                    case "b6e":
                        rstr = GetUrlText(string.Format("http://api.liqwei.com/security/?base64encode={0}",
                        System.Web.HttpUtility.UrlEncode(substring[1], Encoding.GetEncoding("GBK"))));
                        break;
                    case "b6d":
                        rstr = GetUrlText(string.Format("http://api.liqwei.com/security/?base64decode={0}",
                        System.Web.HttpUtility.UrlEncode(substring[1], Encoding.GetEncoding("GBK"))));
                        break;
                    case "i":
                        rstr = GetBotMessage(substring[1]);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                rstr = null;
            }
            if (rstr != null)
            {
                return rstr.Replace(@"<br/>",Environment.NewLine);
            }
            return null;
        }

        public string DealFriendMessage(Dictionary<string, object> info, string message)
        {
            return DealAllMessage(message);
        }

        public string DealGroupMessage(Dictionary<string, object> info, string message)
        {
            return DealAllMessage(message);
        }

        public void MenuClicked(string menuName)
        {
            if (menuName == "status")
            {
                _enabled = !_enabled;
                MessageBox.Show("现在的状态是" + (_enabled ? "启用" : "停用"), "状态指示");
            }
        }

        public string StatusChanged(Dictionary<string, object> info, string newStatus)
        {
            return null;
        }

        public string Input(Dictionary<string, object> info)
        {
            return null;
        }
    }
}
