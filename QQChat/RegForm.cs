using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQChat;
using QQChat;
using System.CodeDom.Compiler;
using System.Reflection;
using QQChat.Classes;
using System.Web;

namespace QQChat
{
    public partial class RegForm : Form
    {

        private static readonly Random _random = new Random();
        private static readonly String _baseUrl = "http://zc.qq.com/iframe/9/reg.html";
        private static readonly String _initUrl = "http://zc.qq.com/cgi-bin/iframe/numreg/init_9?id={0}";
        private static readonly String _imgUrl = "http://captcha.qq.com/getimage?aid=1007901&{0}";
        private static readonly String _smsUrl = "http://zc.qq.com/cgi-bin/iframe/common/sms_send";
        private static readonly String _regUrl = "http://zc.qq.com/cgi-bin/iframe/numreg/get_acc_9";
        private static readonly String _n = "C4D23C2DB0ECC904FE0CD0CBBCDC988C039D79E1BDA8ED4BFD4D43754EC9693460D15271AB43A59AD6D0F0EEE95424F70920F2C4A08DFDF03661300047CA3A6212E48204C1BE71A846E08DD2D9F1CBDDFF40CA00C10C62B1DD42486C70A09C454293BCA9ED4E7D6657E3F62076A14304943252A88EFA416770E0FBA270A141E7";
        private static readonly String _e = "10001";
        private HttpHelper _helper;

        public QQ_Smart QQ
        {
            get;
            private set;
        }
        public RegForm()
        {
            InitializeComponent();
        }

        private void RegForm_Load(object sender, EventArgs e)
        {
            _helper = new HttpHelper(new System.Net.CookieContainer());
            Task.Factory.StartNew(InitBaseInfo).ContinueWith(t => { System.Threading.Thread.Sleep(1000); RefreshCode(); });
        }

        private void InitBaseInfo()
        {
            _helper.GetUrlText(_baseUrl, null);
        }

        private void SetInfo(string text)
        {
            if (text == null) { return; }
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetInfo(text)));
                return;
            }
            richTextBox1.AppendText(text + Environment.NewLine);
        }

        private void buttonCode_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(RefreshCode);
        }
        private string Encrypt(string value)
        {
            var web = Program.MainWebBrowser;
            var doc = web.Document;
            if (doc == null)
            {
                web.Navigate("about:blank");
                doc = web.Document;
            }
            var ele = doc.GetElementById("encryptPassId");
            if (ele == null)
            {
                HtmlElement script = web.Document.CreateElement("script");
                script.SetAttribute("type", "text/javascript");
                script.SetAttribute("id", "encryptPassId");
                string str2 = Properties.Resources.JS;
                script.SetAttribute("text", str2 + @"
function encryptPass(pass){
    var RSA = new RSAKey();
    RSA.setPublic(""C4D23C2DB0ECC904FE0CD0CBBCDC988C039D79E1BDA8ED4BFD4D43754EC9693460D15271AB43A59AD6D0F0EEE95424F70920F2C4A08DFDF03661300047CA3A6212E48204C1BE71A846E08DD2D9F1CBDDFF40CA00C10C62B1DD42486C70A09C454293BCA9ED4E7D6657E3F62076A14304943252A88EFA416770E0FBA270A141E7"", ""10001"");
    return RSA.encrypt(pass); 
}");
                HtmlElement head = web.Document.Body.AppendChild(script);
            }
            var result = web.Document.InvokeScript("encryptPass", new[] { value }) as string;
            return result;
        }

        private void RefreshCode()
        {
            _helper.GetUrlText(string.Format(_initUrl, _random.NextDouble()), null, _baseUrl);
            var stream = _helper.GetUrlStream(string.Format(_imgUrl, _random.NextDouble()), null, _baseUrl);
            if (stream == null)
            {
                return;
            }
            var img = new Bitmap(stream);
            this.Invoke((Action)(() => { this.pictureBox1.Image = img; }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var time = dateTimePicker1.Value;
            var year = time.Year.ToString();
            var month = time.Month.ToString("D2");
            var day = time.Day.ToString("D2");
            var nick = Uri.EscapeDataString(textBoxNick.Text);
            var sex = radioButtonMale.Checked ? "1" : "2";
            var code = textBoxCode.Text;
            var password = this.Encrypt(textBoxPass.Text);
            var paras = string.Format("nick={0}&isnongli=0&isrunyue=0&appid=9&year={1}&month={2}&day={3}&sex={4}&elevel=1&password={5}&verifycode={6}",
                nick,
                year,
                month,
                day,
                sex,
                password,
                code
                );
            var tele = textBoxTele.Text;
            var telecode = textBoxTeleCode.Text;
            if (!string.IsNullOrEmpty(tele) && !string.IsNullOrWhiteSpace(telecode))
            {
                paras += string.Format("&telphone={0}&smsvc={1}", tele, telecode);
            }
            Task.Factory.StartNew(() => DoQuery(_regUrl, paras));
        }

        private void DoQuery(string url, string paras)
        {
            string info = string.Empty;
            var result = _helper.GetUrlText(url, Encoding.UTF8.GetBytes(paras), _baseUrl, headers: new Dictionary<string, string> {
                { "X-Requested-With", "XMLHttpRequest" },
                { "Accept-Encoding", "gzip, deflate" },
                { "Accept-Language", "zh-CN,zh;q=0.8" },
                { "Origin", "http://zc.qq.com" },
                { "DNT", "1" },
            });
            if (result == null)
            {
                info = "无返回结果。";
            }
            else
            {
                var r = QQHelper.FromJson<Dictionary<string, object>>(result);
                if (r.ContainsKey("ec"))
                {
                    int ec = (int)r["ec"];
                    if(ec != 0)
                    {
                        if (r.ContainsKey("em"))
                        {
                            var em = r["em"] as string;
                            em = HttpUtility.HtmlDecode(em);
                            switch (ec)
                            {
                                case 2:
                                    info = "验证码错误：" + em;
                                    break;
                                case 4:
                                    info = "参数错误：" + em;
                                    break;
                                case 14:
                                    info = "手机操作频繁：" + em;
                                    break;
                                case 16:
                                    info = "手机验证错误：" + em;
                                    break;
                                case 20:
                                    info = "需要手机验证：" + em;
                                    break;
                                case 21:
                                    info = "操作被禁止：" + em;
                                    break;
                                default:
                                    info = "未知信息：" + em;
                                    break;
                            }
                        }
                    }                    
                    else
                    {
                        if(r.ContainsKey("em"))
                        {
                            var em = r["em"] as string;
                            em = HttpUtility.HtmlDecode(em);
                            info = "未知信息：" + em;
                        }
                        else
                        {
                            if(r.ContainsKey("uin"))
                            {
                                info = "注册成功：" + r["uin"];
                            }
                            else if(r.ContainsKey("safeverifyResult"))
                            {
                                var VerifyCodeResult = r["VerifyCodeResult"] as string;
                                var elevel = r["elevel"] as string;
                                if (ec == 0)
                                {
                                    if (VerifyCodeResult == "0")
                                    {
                                        info = "验证码发送成功：" + VerifyCodeResult;
                                    }
                                    else
                                    {
                                        info = "验证码发送失败：" + VerifyCodeResult;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(string.IsNullOrWhiteSpace(info))
            {
                info = result;
            }
            SetInfo(info);
        }

        private void buttonTele_Click(object sender, EventArgs e)
        {
            var nick = Uri.EscapeDataString(textBoxNick.Text);
            var tele = textBoxTele.Text;
            var telecode = textBoxTeleCode.Text;
            var paras = string.Format("&telphone={0}&nick={1}&elevel=3&regType=117&r={2}",
                tele,
                nick,
                telecode,
                _random.NextDouble()
                );
            Task.Factory.StartNew(() => DoQuery(_smsUrl, paras));
        }
    }
}
