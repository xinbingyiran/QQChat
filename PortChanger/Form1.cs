using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PortChanger
{
    public partial class Form1 : Form
    {
        public const Int32 DefaultPortNum = 3389;
        private static RegistryKey Currentkey;
        private static RegistryKey Remotekey;
        private static String PortNumberString = "PortNumber";
        private static RegistryValueKind ValueKind = RegistryValueKind.DWord;


        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {//默认
            textBoxMe.Text = DefaultPortNum.ToString();
            textBoxRem.Text = DefaultPortNum.ToString();
            if (SetValue())
            {
                ShowMessage("成功还原为默认");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {//查询
            GetValue();
        }

        private void button2_Click(object sender, EventArgs e)
        {//更新
            SetValue();
        }

        private void GetValue()
        {
            textBoxMe.Text = (Currentkey.GetValue(PortNumberString, DefaultPortNum) as Int32?).ToString();
            textBoxRem.Text = (Remotekey.GetValue(PortNumberString, DefaultPortNum) as Int32?).ToString();
            ShowMessage("查询成功");
        }

        private bool SetValue()
        {
            Int32 tvalue, rvalue;
            try
            {
                tvalue = Convert.ToInt32(textBoxMe.Text);
            }
            catch (Exception)
            {
                ShowMessage("开放端口只能为数字");
                textBoxMe.Focus();
                return false;
            }
            try
            {
                rvalue = Convert.ToInt32(textBoxRem.Text);
            }
            catch (Exception)
            {
                ShowMessage("默认端口只能为数字");
                textBoxRem.Focus();
                return false;
            }
            Currentkey.SetValue(PortNumberString, tvalue, ValueKind);
            Remotekey.SetValue(PortNumberString, rvalue, ValueKind);
            ShowMessage("设置成功");
            return true;
        }

        private void ShowMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    ShowMessage(message);
                }));
                return;
            }
            label3.Text = DateTime.Now.ToString("HH:mm:ss") + message;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //hkey_local_machine\system\currentcontrolset\control\terminal server\wds\rdpwd\tds\tcp
            //hkey_local_machine\system\currentcontrolset\control\terminal server\WINSTATIONS\RDP-TCP
            try
            {
                RegistryKey basekey = Registry.LocalMachine
                    .OpenSubKey("SYSTEM")
                    .OpenSubKey("CurrentControlSet")
                    .OpenSubKey("Control")
                    .OpenSubKey("Terminal Server");
                Currentkey = basekey
                    .OpenSubKey("WinStations")
                    .OpenSubKey("RDP-Tcp", true);
                Remotekey = basekey
                    .OpenSubKey("Wds")
                    .OpenSubKey("rdpwd")
                    .OpenSubKey("Tds")
                    .OpenSubKey("tcp", true);
            }
            catch (Exception)
            {
                MessageBox.Show("应用程序错误，请确定本机是否支持远程桌面服务并开启。", "错误");
                System.Environment.Exit(-1);
            }
            GetValue();
            ShowMessage("初始化完成");
        }
    }
}
