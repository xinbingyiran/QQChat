namespace QQChat
{

    public class QQUser
    {
        public string Uin { get; internal set; }
        public string PtWebQQ { get; internal set; }
        public string skey { get; internal set; }
        public string GTK { get; internal set; }
        public string QQNum { get; internal set; }
        public string QQName { get; internal set; }


        public QQUser()
        {
            this.QQNum = "0";
        }
    }
}
