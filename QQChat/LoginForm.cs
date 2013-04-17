using WebQQ2.WebQQ2;
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
using System.IO;
using System.Security.Cryptography;
using QQChat.Classes;
using Newtonsoft.Json;

namespace QQChat
{
    public partial class LoginForm : Form
    {
        private QQ _qq;

        private string _fileName;

        private Dictionary<string, string> _settings;

        public LoginForm()
        {
            InitializeComponent();
            _fileName = Application.StartupPath + "\\QQUser.dat";
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
            try
            {
                var lines = File.ReadAllLines(_fileName);
                textBoxUser.Text = PassHelper.AESDecrypt(lines[0]);
                textBoxPass.Text = PassHelper.AESDecrypt(lines[1]);
                _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(lines[2]);
            }
            catch (Exception) { }
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
                if (_qq == null || textBoxUser.Text != _qq.User.QQNum)
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
            string vcode = _qq.GetVerifyCode();
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
            _qq = new QQ(qqnum);
        }

        private void pictureBoxCode_Click(object sender, EventArgs e)
        {
            new Task(GetVerifyImage).Start();
        }

        private void GetVerifyImage()
        {
            Image result = _qq.GetVerifyImage();
            SetImageCode(result);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnableLog(false);
            if (_qq == null)
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
            LogQQ(textBoxPass.Text, textBoxCode.Text, (string)comboBox1.SelectedValue);
        }

        private void EnableLog(bool Enable)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { EnableLog(Enable); }));
                return;
            }
            button1.Enabled = Enable;
        }

        private void LogQQ(string pass, string code, string status)
        {
            new Task(() =>
            {
                string result = _qq.LoginQQ(pass, code);
                if (!_qq.User.IsPreLoged)
                {
                    SetInfo(result);
                    GetVerifyImage();
                    EnableLog(true);
                }
                else
                {
                    SetInfo(result);
                    result = _qq.LoginQQ2(status);
                    if (result != null)
                    {
                        SetInfo(result);
                        EnableLog(true);
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
            m.InitUser(_qq);
            m.SetSettings(_settings);
            if (m.ShowDialog() != System.Windows.Forms.DialogResult.Retry)
            {
                _settings = m.GetSettings(); ;
                SaveToFile();
                Environment.Exit(Environment.ExitCode);
            }
            new Task(GetVerifyCode).Start();
            this.Show();
            EnableLog(true);
        }

        private void LoginForm_Activated(object sender, EventArgs e)
        {
            if (textBoxUser.Text.Length == 0)
            {
                textBoxUser.Focus();
            }
            else
            {
                textBoxPass.Focus();
            }
        }

        private void textBoxPass_Leave(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void SaveToFile()
        {
            try
            {
                string[] lines = new[] { 
                    PassHelper.AESEncrypt(textBoxUser.Text),
                    PassHelper.AESEncrypt(textBoxPass.Text),
                    JsonConvert.SerializeObject(_settings)
                };
                File.WriteAllLines(_fileName, lines);
            }
            catch (Exception) { }
        }
    }
}
