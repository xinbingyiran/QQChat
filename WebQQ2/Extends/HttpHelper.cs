using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebQQ2.Extends
{
    public class HttpHelper
    {
        private CookieContainer _cookiecontainer;
        public HttpHelper(CookieContainer cookiecontainer)
        {
            _cookiecontainer = cookiecontainer;
        }

        public string GetUrlText(string url, byte[] postData, string refer = null, int timeout = 60000, Dictionary<string, string> headers = null)
        {
            try
            {
                HttpWebResponse myResponse = GetResponse(url, postData, refer, timeout,headers);
                if (myResponse == null)
                {
                    return null;
                }
                Stream newStream = GetResponseStream(myResponse);
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

        public Stream GetUrlStream(string url, byte[] postData, string refer = null, int timeout = 60000,Dictionary<string,string> headers = null)
        {
            try
            {
                HttpWebResponse myResponse = GetResponse(url, postData, refer, timeout,headers);
                return GetResponseStream(myResponse);
            }
            catch (Exception)
            {
                return null;
            }
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
                    response = (HttpWebResponse)myRequest.GetResponse();
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
                    myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                    myRequest.AllowAutoRedirect = true;
                    myRequest.KeepAlive = true;
                    myRequest.Accept = "*/*";
                    if(headers != null)
                    {
                        foreach(var item in headers)
                        {
                            if(myRequest.Headers.AllKeys.Contains(item.Key))
                            {
                                myRequest.Headers[item.Key] = item.Value;
                            }
                            else
                            {
                                myRequest.Headers.Add(item.Key,item.Value);
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
                    response = (HttpWebResponse)myRequest.GetResponse();
                }
                catch (Exception) { }
            });
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return response;
            throw new TimeoutException();
        }

        public string GetFileTrueUrl(string url, string refer = null, int timeout = 60000)
        {
            String newUrl = null;
            Task task = new Task(() =>
            {
                try
                {
                    HttpWebRequest myRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                    myRequest.Method = "GET";
                    myRequest.Referer = refer;
                    myRequest.CookieContainer = _cookiecontainer;
                    myRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.802.30 Safari/535.1 SE 2.X MetaSr 1.0";
                    myRequest.AllowAutoRedirect = false;
                    myRequest.KeepAlive = true;
                    myRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    myRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                    myRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                    myRequest.Headers.Add("Accept-Charset", "GBK,utf-8;q=0.7,*;q=0.3");
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();//1
                    WebHeaderCollection headers = myResponse.Headers;
                    if ((myResponse.StatusCode == System.Net.HttpStatusCode.Found) ||
                      (myResponse.StatusCode == System.Net.HttpStatusCode.Redirect) ||
                      (myResponse.StatusCode == System.Net.HttpStatusCode.Moved) ||
                      (myResponse.StatusCode == System.Net.HttpStatusCode.MovedPermanently))
                    {
                        newUrl = headers["Location"];
                        newUrl = newUrl.Trim();
                    }
                    myResponse.Close();
                }
                catch (Exception)
                {
                    newUrl = null;
                }
            });
            task.Start();
            bool wait = task.Wait(timeout);
            if (wait)
                return newUrl;
            throw new TimeoutException();
        }
    }
}
