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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApi
{
    public class MyWebApi : TMessage
    {
        public override string PluginName
        {
            get { return "天气预报"; }
        }

        private static CancellationTokenSource _cts;

        private static string _defaultCity = "保定";

        public override string Setting
        {
            get
            {
                return (Enabled ? "1" : "0") + _defaultCity;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 1)
                {
                    Enabled = value[0] == '1';
                    _defaultCity = value.Substring(1);
                }
            }
        }

        public override Dictionary<string, string> Menus
        {
            get { return _menus; }
        }

        private Dictionary<string, string> _menus = new Dictionary<string, string>
        {
        };

        private static Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"天气[ 城市]","显示指定城市的天气"}
        };

        public override Dictionary<string, string> Filters
        {
            get
            {
                return _filters;
            }
        }

        public MyWebApi()
        {
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

        private static readonly string weatherurl = "http://api.36wu.com/Weather/GetMoreWeather?district={0}";

        public override string DealMessage(string messageType, Dictionary<string, object> info, string message)
        {
            if (messageType != MessageType.MessageFriend && messageType != MessageType.MessageGroup)
            {
                return null;
            }
            if (!Enabled)
                return null;
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            string[] substring = message.Split(new char[] { ' ' }, 2, StringSplitOptions.None);
            string rstr = null;
            try
            {
                if (substring.Length > 0 && substring[0] == "天气")
                {
                    string citycode = _defaultCity;
                    if (substring.Length > 1)
                    {
                            citycode = substring[1];
                    }
                    string url = string.Format(weatherurl, citycode);
                    string urlresult = GetUrlText(url);
                    if (urlresult != null)
                    {
                        var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(urlresult);
                        if (result != null && result.ContainsKey("data"))
                        {
                            var w = result["data"] as JObject;
                            if (w != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(string.Format("{0} 天气预报：", w["city"]));
                                for (int i = 1; i < 7; i++)
                                {
                                    sb.AppendLine(string.Format("{0} : {1}  {2}  {3}  {4}",
                                        w["date_" + i], 
                                        w["weather_" + i],
                                        w["temp_" + i],
                                        w["wind_" + i],
                                        w["fl_" + i]
                                        ));
                                }
                                sb.AppendLine(string.Format("温馨提示：{0}", w["index_d"]));
                                rstr = sb.ToString();
                            }
                        }
                    }
                }
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

        public override event EventHandler<EventArgs> OnMessage;

        public override void MenuClicked(string menuName)
        {
        }

        public override string AboutMessage
        {
            get
            {
                return "天气预报。\r\n信息采集于天气网。";
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
