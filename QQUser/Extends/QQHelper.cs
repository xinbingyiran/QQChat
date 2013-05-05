using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WebQQ2.WebQQ2;

namespace WebQQ2.Extends
{
    public class QQHelper
    {
        public static long GetTime()
        {
            DateTime startDate = new DateTime(1970, 1, 1);
            DateTime endDate = DateTime.Now.ToUniversalTime();
            TimeSpan span = endDate - startDate;
            return (long)(span.TotalMilliseconds + 0.5);
        }

        public static DateTime ToTime(long time)
        {
            DateTime dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            dtZone = dtZone.AddSeconds(time);
            return dtZone.ToLocalTime();
        }

        public static string GetPassword(string uin, string password, string verficode)
        {
            string rpass = HEXMD5.hexchar2bin(HEXMD5.Md5(password));
            rpass = HEXMD5.Md5(rpass + uin);
            rpass = HEXMD5.Md5(rpass + verficode.ToUpper());
            return rpass;
        }

        public static string GetPassword(string password, string verifycode)
        {
            return Md5(Md5_3(password) + verifycode.ToUpper());
        }

        public static string Md5_3(string input)
        {
            MD5 md = MD5.Create();
            byte[] buffer = md.ComputeHash(Encoding.UTF8.GetBytes(input));
            buffer = md.ComputeHash(buffer);
            buffer = md.ComputeHash(buffer);
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static string Md5(string input)
        {
            byte[] buffer = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static TEntry FromJson<TEntry>(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<TEntry>(input);
        }

        public static object FromJson(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize(input, typeof(object));
        }

        public static string ToJson(object input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(input);
        }

        public static string ToPostData(object input)
        {
            string str = ToJson(input);
            return "r=" + HttpUtility.UrlEncode(str);
        }

        public static string GetToken(QQUser user)
        {
            return GetToken(user.QQNum, user.PtWebQQ);
        }


        public static string GetToken(string b, string i)
        {
            var a = i + "password error";
            var s = "";
            var j = "";
            for (; ; )
            {
                if (s.Length <= a.Length)
                {
                    s += b;
                    if (s.Length == a.Length)
                        break;
                }
                else
                {
                    s = s.Substring(0, a.Length);
                    break;
                }
            }
            for (var d = 0; d < s.Length; d++)
                j += (char)(s[d] ^ a[d]);
            var a2 = "0123456789ABCDEF";
            s = "";
            for (var d = 0; d < j.Length; d++)
            {
                s += a2[j[d] >> 4 & 15];
                s += a2[j[d] & 15];
            }
            return s;
        }
    }
}
