using System;
using System.Web.Script.Serialization;

namespace QQChat
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

        public static TEntry FromJson<TEntry>(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<TEntry>(input);
        }


        public static string getGTK(string str)
        {
            UInt32 hash = 5381;
            for (int i = 0, len = str.Length; i < len; ++i)
            {
                hash += (hash << 5) + str[i];
            }
            return (hash & 0x7fffffff).ToString();
        }
    }
}
