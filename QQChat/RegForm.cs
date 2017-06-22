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
using WebQQ2.Extends;
using WebQQ2.WebQQ2;
using System.CodeDom.Compiler;
using System.Reflection;
using QQChat.Classes;

namespace QQChat
{
    public partial class RegForm : Form
    {

        private static readonly Random _random = new Random();
        private static readonly String _baseUrl = "http://zc.qq.com/iframe/9/reg.html";
        private static readonly String _initUrl = "http://zc.qq.com/cgi-bin/iframe/numreg/init_9?id={0}";
        private static readonly String _imgUrl = "http://captcha.qq.com/getimage?aid=1007901&{0}";
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
            Task.Factory.StartNew(InitBaseInfo).ContinueWith(t => RefreshCode());
        }

        private void InitBaseInfo()
        {
            _helper.GetUrlText(_baseUrl, null);
            _helper.GetUrlText(string.Format(_initUrl, _random.NextDouble()), null, _baseUrl);
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

        private WebBrowser _webbrowser;

        private string Encrypt(string value)
        {
            string str2 = Properties.Resources.JS;

            string fun = string.Format(@"getPass('{0}')", value);

            string result = ExecuteScript(fun, str2);

            return result;
       }

        private string ExecuteScript(string sExpression, string sCode)
        {
            MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = true;
            scriptControl.Language = "JScript";
            scriptControl.AddCode(sCode);
            try
            {
                string str = scriptControl.Eval(sExpression).ToString();
                return str;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }

        private void RefreshCode()
        {
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
            var nick = textBoxNick.Text;
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
            var result = _helper.GetUrlText(_regUrl, Encoding.UTF8.GetBytes(paras), _baseUrl, headers: new Dictionary<string, string> { { "X-Requested-With", "XMLHttpRequest" } });
            SetInfo(result);
        }
    }
}
