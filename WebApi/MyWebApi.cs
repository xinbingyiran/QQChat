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
        private static string _defaultCityCode = "101090201";
        private static string _filepath;

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
            {"重载","reload"},
        };

        private static Dictionary<string, string> _filters = new Dictionary<string, string>
        {
            {"天气[ 城市]","显示指定城市的天气"}
        };

        private static Dictionary<string, string> _citycode = new Dictionary<string, string>
        {
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
            var assemblay = this.GetType().Assembly;
            var filedir = assemblay.Location;
            filedir = filedir.Substring(0, filedir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _filepath = filedir + this.GetType().FullName + ".db";
            new Task(() =>
            {
                LoadPara();
            }).Start();
        }

        private void LoadPara()
        {
            _citycode.Clear();
            string[] lines;
            try
            {
                if (!File.Exists(_filepath))
                {
                    lines = WebApi.Properties.Resources.DefaultCode.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    File.WriteAllText(_filepath, WebApi.Properties.Resources.DefaultCode);
                }
                else
                {
                    lines = File.ReadAllLines(_filepath);
                }
                var len = lines.Length;
                for (int i = 0; i < len; i++)
                {
                    try
                    {
                        var jsonstrs = JsonConvert.DeserializeObject<string[]>(lines[i]);
                        if (jsonstrs.Length == 2 && !_citycode.ContainsKey(jsonstrs[0]))
                        {
                            _citycode.Add(jsonstrs[0], jsonstrs[1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (OnMessage != null)
                        {
                            LastMessage = ex.Message + "@" + (i + 1);
                            OnMessage(this, EventArgs.Empty);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
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

        private static readonly string weatherurl = "http://m.weather.com.cn/data/{0}.html";

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
                    string citycode = _defaultCityCode;
                    if (substring.Length > 1)
                    {
                        if (_citycode.ContainsKey(substring[1]))
                        {
                            citycode = _citycode[substring[1]];
                        }
                        else
                        {
                            throw new Exception("天气代码未找到");
                        }
                    }
                    string url = string.Format(weatherurl, citycode);
                    string urlresult = GetUrlText(url);
                    if (urlresult != null)
                    {
                        var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(urlresult);
                        if (result != null && result.ContainsKey("weatherinfo"))
                        {
                            var w = result["weatherinfo"];
                            if (w != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(string.Format("{0}  {1}  {2}：", w["date_y"], w["week"], w["city"]));
                                sb.AppendLine(string.Format("今日天气: {0}  {1}  {2}", w["weather1"], w["wind1"], w["temp1"]));
                                //sb.AppendLine(string.Format("穿衣指数：{0}  {1}", w["index"], w["index_d"]));
                                sb.AppendLine(string.Format("紫外线强度：{0}", w["index_uv"]));
                                sb.AppendLine(string.Format("人体舒适度：{0}", w["index_co"]));
                                sb.AppendLine(string.Format("晨练指数：{0}", w["index_cl"]));
                                sb.AppendLine(string.Format("晾晒指数：{0}", w["index_ls"]));
                                sb.AppendLine(string.Format("明日天气：{0}  {1}  {2}", w["weather2"], w["wind2"], w["temp2"]));
                                sb.AppendLine(string.Format("后日天气：{0}  {1}  {2}", w["weather3"], w["wind3"], w["temp3"]));
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
            if (menuName == "reload")
            {
                LoadPara();
                LastMessage = "重载参数完成，请刷新查看";
                if (OnMessage != null)
                {
                    OnMessage(this, EventArgs.Empty);
                }
            }
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
