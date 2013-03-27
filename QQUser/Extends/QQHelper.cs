using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

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
            string rpass = HEXMD5.hexchar2bin(HEXMD5.md5(password));
            rpass = HEXMD5.md5(rpass + uin);
            rpass = HEXMD5.md5(rpass + verficode.ToUpper());
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
            return (TEntry)jss.Deserialize(input, typeof(TEntry));
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
    }
}
