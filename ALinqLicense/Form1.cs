using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Net;
using System.Web;

namespace ALinqLicense
{
    public partial class Form1 : Form
    {
        internal const string buchong = "ADECRYPT";
        private const string b = "TRIAL";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private string CreateKey(string username,string licenseName)
        {
            string encryptKey = licenseName + buchong.Substring(0, 8 - licenseName.Length);
            string key = EncryptDES(username, encryptKey);
            key = buchong.Substring(0, 8 - licenseName.Length) + key;
            return key;
        }

        private string CheckKey(string key,string licenseName)
        {
            string encryptKey = licenseName + buchong.Substring(0, 8 - licenseName.Length);
            string s2 = DecryptDES(key.Substring(8 - licenseName.Length), encryptKey);
            return s2;
        }

        internal static string DecryptDES(string decryptString, string decryptKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] buffer = new byte[decryptString.Length / 2];
            for (int i = 0; i < (decryptString.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(decryptString.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }
            provider.Key = Encoding.Default.GetBytes(decryptKey);
            provider.IV = Encoding.Default.GetBytes(decryptKey);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Encoding.Default.GetString(stream.ToArray());
        }


        internal static string EncryptDES(string encryptString, string encryptKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] buffer = Encoding.Default.GetBytes(encryptString);
            provider.Key = Encoding.Default.GetBytes(encryptKey);
            provider.IV = Encoding.Default.GetBytes(encryptKey);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return BitConverter.ToString(stream.ToArray()).Replace("-","");
        }

        private void CreateTrailFile(string productname)
        {
            if (!string.IsNullOrEmpty(productname))
            {
                WebClient webClient = new WebClient();
                byte[] numArray = webClient.DownloadData(HttpUtility.HtmlEncode("http://www.alinq.org/Admin/GenerateTrialLicenseKey.ashx"));
                string str = Encoding.UTF8.GetString(numArray);
                object[] upper = new object[] { productname.ToUpper(), null, null };
                Hashtable hashtables = new Hashtable();
                hashtables.Add("ALinq.Access", str);
                hashtables.Add("ALinq.SQLite", str);
                hashtables.Add("ALinq.MySQL", str);
                hashtables.Add("ALinq.Oracle", str);
                hashtables.Add("ALinq.Oracle.Odp", str);
                hashtables.Add("ALinq.Firebird", str);
                hashtables.Add("ALinq.DB2", str);
                hashtables.Add("ALinq.PostgreSQL", str);
                Hashtable hashtables1 = hashtables;
                upper[2] = hashtables1;
                string str1 = Path.Combine(Application.StartupPath, "ALinq.lic");
                using (FileStream fileStream = new FileStream(str1, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, upper);
                }
            }
        }
        private void CreateLicenseFile(string productname,string username)
        {
            if (!string.IsNullOrEmpty(productname))
            {
                string str = CreateKey(username, buchong);
                object[] upper = new object[] { productname.ToUpper(), null, null };
                Hashtable hashtables = new Hashtable();
                hashtables.Add("ALinq.Access", CreateKey(username + Environment.NewLine + CreateKey(username, "Access"), buchong));
                hashtables.Add("ALinq.SQLite", CreateKey(username + Environment.NewLine + CreateKey(username, "SQLite"), buchong));
                hashtables.Add("ALinq.MySQL", CreateKey(username + Environment.NewLine + CreateKey(username, "MySQL"), buchong));
                hashtables.Add("ALinq.Oracle", CreateKey(username + Environment.NewLine + CreateKey(username, "Oracle"), buchong));
                hashtables.Add("ALinq.Oracle.Odp", CreateKey(username + Environment.NewLine + CreateKey(username, "Oracle"), buchong));
                hashtables.Add("ALinq.Firebird", CreateKey(username + Environment.NewLine + CreateKey(username, "Firebird"), buchong));
                hashtables.Add("ALinq.DB2", CreateKey(username + Environment.NewLine + CreateKey(username, "DB2"), buchong));
                hashtables.Add("ALinq.PostgreSQL", CreateKey(username + Environment.NewLine + CreateKey(username, "Postgre"), buchong));
                Hashtable hashtables1 = hashtables;
                upper[2] = hashtables1;
                string str1 = Path.Combine(Application.StartupPath, "ALinq.lic");
                using (FileStream fileStream = new FileStream(str1, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, upper);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox_User.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入用户名");
                return;
            }
            string username = textBox_User.Text.Trim();
            textBoxAcc1.Text = CreateKey(username, "Access");
            textBoxSqlite1.Text = CreateKey(username, "SQLite");
            textBoxMysql1.Text = CreateKey(username, "MySQL");
            textBoxOracle1.Text = CreateKey(username, "Oracle");
            textBoxOdp1.Text = CreateKey(username, "Oracle");
            textBoxFirebird1.Text = CreateKey(username, "Firebird");
            textBoxDB21.Text = CreateKey(username, "DB2");
            textBoxPostgre1.Text = CreateKey(username, "Postgre");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox_Product.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入产品名");
                return;
            }
            if (textBox_User.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入用户名");
                return;
            }
            CreateLicenseFile(textBox_Product.Text.Trim(),textBox_User.Text.Trim());
            MessageBox.Show("生成 ALinq.lic 成功");
        }
    }
}
