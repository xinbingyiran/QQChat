using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQChat
{
    public class HttpHelper
    {
        private readonly CookieContainer _cookiecontainer;
        public HttpHelper(CookieContainer cookiecontainer)
        {
            _cookiecontainer = cookiecontainer;
        }

        public string GetUrlText(string url, byte[] postData, string refer = null, int timeout = 60000, Dictionary<string, string> headers = null)
        {
            try
            {
                using (HttpWebResponse myResponse = GetResponse(url, postData, refer, timeout, headers))
                {
                    if (myResponse == null)
                    {
                        return null;
                    }
                    using (Stream newStream = GetResponseStream(myResponse))
                    {
                        if (newStream != null)
                        {
                            Encoding encoding = null;
                            try
                            {
                                if (string.IsNullOrWhiteSpace(myResponse.CharacterSet))
                                {
                                    encoding = Encoding.UTF8;
                                }
                                else
                                {
                                    encoding = Encoding.GetEncoding(myResponse.CharacterSet);
                                }
                            }
                            catch
                            {
                                encoding = Encoding.UTF8;
                            }
                            StreamReader reader = new StreamReader(newStream, encoding);
                            string result = reader.ReadToEnd();
                            return result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Stream GetResponseStream(HttpWebResponse myResponse)
        {
            var stream = myResponse.GetResponseStream();
            if (string.IsNullOrWhiteSpace(myResponse.ContentEncoding))
            {
                return stream;
            }
            if (myResponse.ContentEncoding.ToLower().Contains("gzip"))
            {
                return new GZipStream(stream, CompressionMode.Decompress);
            }
            else if (myResponse.ContentEncoding.ToLower().Contains("deflate"))
            {
                return new DeflateStream(stream, CompressionMode.Decompress);
            }
            return stream;
        }

        public HttpWebResponse GetNoRedirectResponse(string url, string refer, int timeout)
        {
            var messageTaskCts = new CancellationTokenSource();
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                try
                {
                    HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                    myRequest.Method = "GET";
                    myRequest.Referer = refer;
                    myRequest.CookieContainer = _cookiecontainer;
                    myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                    myRequest.AllowAutoRedirect = false;
                    myRequest.KeepAlive = true;
                    try
                    {
                        response = (HttpWebResponse)myRequest.GetResponse();
                    }
                    catch (Exception)
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                        if (myRequest != null)
                        {
                            myRequest.Abort();
                        }
                    }
                }
                catch (Exception) { }
            });
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return response;
            throw new TimeoutException();
        }

        private HttpWebResponse GetResponse(string url, byte[] postData, string refer, int timeout, Dictionary<string, string> headers)
        {
            HttpWebResponse response = null;
            Task task = new Task(() =>
            {
                try
                {
                    HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                    myRequest.Referer = refer;
                    myRequest.CookieContainer = _cookiecontainer;
                    myRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    myRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.3051.400 QQBrowser/9.6.11301.400";
                    myRequest.AllowAutoRedirect = true;
                    myRequest.KeepAlive = true;
                    myRequest.Accept = "*/*";
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            if (myRequest.Headers.AllKeys.Contains(item.Key))
                            {
                                myRequest.Headers[item.Key] = item.Value;
                            }
                            else
                            {
                                myRequest.Headers.Add(item.Key, item.Value);
                            }
                        }
                    }
                    if (postData != null)
                    {
                        myRequest.Method = "POST";
                        myRequest.ServicePoint.Expect100Continue = false;
                        myRequest.ContentLength = postData.Length;
                        if (postData.Length > 0)
                        {
                            using (var sw = myRequest.GetRequestStream())
                            {
                                sw.Write(postData, 0, postData.Length);
                            }
                        }
                    }
                    else
                    {
                        myRequest.Method = "Get";
                    }
                    try
                    {
                        response = (HttpWebResponse)myRequest.GetResponse();
                    }
                    catch (Exception)
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                        if (myRequest != null)
                        {
                            myRequest.Abort();
                        }
                    }
                }
                catch (Exception)
                {
                }
            });
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return response;
            throw new TimeoutException();
        }
    }
}
