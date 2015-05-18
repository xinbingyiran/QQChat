using WebQQ2.WebQQ2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using QQChat.Classes;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace QQChat
{
    public partial class LoginForm : Form
    {
        public QQ QQ
        {
            get;
            private set;
        }

        public string UserString
        {
            get
            {
                return PassHelper.AESEncrypt(textBoxUser.Text);
            }
        }
        public string PassString
        {
            get
            {
                return PassHelper.AESEncrypt(textBoxPass.Text);
            }
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            InitParas();
        }

        private void InitParas()
        {
            webBrowser1.DocumentText = @"<html>
<body>
<script type=""text/javascript"">
"
                + QQChat.Properties.Resources.js +
@"
var getFun = function(a,b,c,d)
{
    return $.Encryption.getEncryption(a,b,c,d);
}
</script>
</body>
</html>";
            comboBox1.DataSource = QQStatus.AllStatus.Except(new QQStatus[] { QQStatus.StatusOffline }).ToArray();
            comboBox1.DisplayMember = "Status";
            comboBox1.ValueMember = "StatusInternal";
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            panel1.Visible = false;
            try
            {
                textBoxUser.Text = PassHelper.AESDecrypt(MainForm.mainForm.Paras[0]);
                textBoxPass.Text = PassHelper.AESDecrypt(MainForm.mainForm.Paras[1]);
            }
            catch (Exception) { }
            QQ = new WebQQ2.WebQQ2.QQ();
        }

        private void SetImageCode(Image image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetImageCode(image)));
                return;
            }
            panel1.Visible = true;
            pictureBoxCode.Image = image;
            SetInfo("需要输入验证码");
            EnableLog(true);
        }

        private void SetTextCode(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetTextCode(text)));
                return;
            }
            panel1.Visible = false;
            textBoxCode.Text = text;
            EnableLog(true);
        }

        private void SetInfo(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetInfo(text)));
                return;
            }
            label4.Text = text;
        }

        private void textBoxUser_Leave(object sender, EventArgs e)
        {
            CheckUser();
        }

        private void CheckUser()
        {
            const string mstr = @"\d{5,12}";
            if (Regex.IsMatch(textBoxUser.Text, mstr))
            {
                CreateUser(textBoxUser.Text);
                new Task(GetVerifyCode).Start();
            }
            else
            {
                SetInfo("应为5-12位数字");
            }
        }

        private void GetVerifyCode()
        {
            SetInfo("验证是否需要验证码");
            string vcode = QQ.GetVerifyCode();
            if (vcode.StartsWith("!") && vcode.Length == 4)
            {
                SetTextCode(vcode);
                SetInfo("不需要验证码");
            }
            else
            {
                GetVerifyImage();
            }
        }

        private void CreateUser(string qqnum)
        {
            QQ = new QQ(qqnum);
        }

        private void pictureBoxCode_Click(object sender, EventArgs e)
        {
            new Task(GetVerifyImage).Start();
        }

        private void GetVerifyImage()
        {
            try
            {
                Image result = QQ.GetVerifyImage();
                SetImageCode(result);
            }
            catch (Exception) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnableLog(false);
            if (QQ == null)
            {
                SetInfo("用户名不能空...");
                EnableLog(true);
                return;
            }
            if (textBoxPass.Text.Length < 6)
            {
                SetInfo("密码长度错误...");
                EnableLog(true);
                return;
            }
            if (!QQ.IsPreLoged)
            {
                LogQQ(textBoxPass.Text, textBoxCode.Text, true);
            }
            else
            {
                LogQQ2();
            }
        }

        private void EnableLog(bool Enable)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => EnableLog(Enable)));
                return;
            }
            button1.Enabled = Enable;
            button2.Enabled = Enable;
        }

        private void LogQQ(string pass, string code, bool logqq2 = false)
        {
            var tpassobj = webBrowser1.Document.InvokeScript("getFun", new[] { pass, QQ.User.Uin, code });
            new Task(() =>
            {
                MainForm.mainForm.SaveToFile();
                var tpass = tpassobj.ToString();
                string result = QQ.LoginQQ(tpass, code, false);
                if (!QQ.User.IsPreLoged)
                {
                    SetInfo(result);
                    GetVerifyImage();
                    EnableLog(true);
                }
                else
                {
                    SetInfo(result);
                    if (logqq2)
                    {
                        LogQQ2();
                    }
                    else
                    {
                        EnableLog(true);
                    }
                }
            }).Start();
        }

        private void LogQQ2()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(LogQQ2));
                return;
            }
            var status = (string)comboBox1.SelectedValue;
            new Task((logStatus) =>
            {
                var result = QQ.LoginQQ2(logStatus as string);
                if (result != null)
                {
                    SetInfo(result);
                    EnableLog(true);
                    return;
                }
                InitMainForm();
            }, status).Start();
        }

        private void InitMainForm()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(InitMainForm));
                return;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                EnableLog(true);
                if (textBoxUser.Text.Length == 0)
                {
                    textBoxUser.Focus();
                }
                else
                {
                    if (!textBoxUser.Focused)
                    {
                        CheckUser();
                    }
                    else
                    {
                        textBoxPass.Focus();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EnableLog(false);
            if (QQ == null)
            {
                SetInfo("用户名不能空...");
                EnableLog(true);
                return;
            }
            if (textBoxPass.Text.Length < 6)
            {
                SetInfo("密码长度错误...");
                EnableLog(true);
                return;
            }
            //if (!QQ.IsGlobalLog())
            //{
            MainForm.mainForm.GlobalForm.Hide();
            LogQQ(textBoxPass.Text, textBoxCode.Text, false);
            //}
            //else
            //{
            //    SetInfo("你已登录...");
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!QQ.IsPreLoged)
            {
                SetInfo("请先登录...");
                return;
            }
            MainForm.mainForm.GlobalForm.InitQQ(this.QQ);
            MainForm.mainForm.GlobalForm.Show();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                MainForm.mainForm.SaveToFile();
            }
        }

    }
}
