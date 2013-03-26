using QQChat.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQChat
{
    public partial class LoginForm : Form
    {
        private QQUser _user;

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
            comboBox1.DataSource = QQStatus.AllStatus;
            comboBox1.DisplayMember = "Status";
            comboBox1.ValueMember = "StatusInternal";
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            panel1.Visible = false;
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
        }

        private void SetTextCode(string Text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetTextCode(Text)));
                return;
            }
            panel1.Visible = false;
            textBoxCode.Text = Text;
        }

        private void SetInfo(string Text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetInfo(Text)));
                return;
            }
            label4.Text = Text;
        }

        private void textBoxUser_Leave(object sender, EventArgs e)
        {
            string mstr = @"\d{5,12}";
            if (Regex.IsMatch(textBoxUser.Text, mstr))
            {
                if (_user == null || textBoxUser.Text != _user.QQNum)
                {
                    CreateUser(textBoxUser.Text);
                }
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
            string vcode = _user.GetVerifyCode();
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
            _user = new QQUser(qqnum);
        }

        private void pictureBoxCode_Click(object sender, EventArgs e)
        {
            new Task(GetVerifyImage).Start();
        }

        private void GetVerifyImage()
        {
            SetInfo("加载验证码中...");
            Image result = _user.GetVerifyImage();
            SetImageCode(result);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_user == null)
            {
                SetInfo("用户名不能空...");
                return;
            }
            if (textBoxPass.Text.Length < 6)
            {
                SetInfo("密码长度错误...");
                return;
            }
            LogQQ(textBoxPass.Text, textBoxCode.Text, (string)comboBox1.SelectedValue);
        }

        private void LogQQ(string pass, string code, string status)
        {
            new Task(() =>
            {
                string result = _user.LogQQ(pass, code);
                if (!_user.IsPreLoged)
                {
                    GetVerifyImage();
                    return;
                }
                else
                {
                    SetInfo(result);
                    result = _user.LogQQ2(status);
                    if (result != null)
                    {
                        SetInfo(result);
                        return;
                    }
                    InitMainForm();
                }
            }).Start();
        }

        private void InitMainForm()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(InitMainForm));
                return;
            }
            this.Hide();
            MainForm m = new MainForm();
            m.InitUser(_user);
            m.ShowDialog();
            Environment.Exit(Environment.ExitCode);
        }
    }
}
