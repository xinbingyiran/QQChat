using QQChatWeb.App_Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebQQ2.WebQQ2;

namespace QQChatWeb
{
    public partial class Login : System.Web.UI.Page
    {
        private QQ QQItem
        {
            get { return ServiceCore.Instance.GetQQ(Session.SessionID); }
            set { ServiceCore.Instance.AddQQ(Session.SessionID, value); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            userNum.Attributes["onblur"] = ClientScript.GetPostBackEventReference(userNumButton, null);
            if (!IsPostBack)
            {
                InitParas();
            }
        }


        private void InitParas()
        {
            DropDownList1.DataSource = QQStatus.AllStatus.Except(new QQStatus[] { QQStatus.StatusOffline }).ToArray();
            DropDownList1.DataTextField = "Status";
            DropDownList1.DataValueField = "StatusInternal";
            DropDownList1.DataBind();
            if (DropDownList1.Items.Count > 0)
            {
                DropDownList1.SelectedIndex = 0;
            }
            trverify.Visible = false;
            trverifyimg.Visible = false;
        }

        protected void userNumButton_Click(object sender, EventArgs e)
        {
            CheckUser();
        }
        private void CheckUser()
        {
            const string mstr = @"\d{5,12}";
            if (Regex.IsMatch(userNum.Text, mstr))
            {
                CreateUser(userNum.Text);
                GetVerifyCode();
            }
            else
            {
                SetInfo("应为5-12位数字");
            }
        }

        private void GetVerifyCode()
        {
            SetInfo("验证是否需要验证码");
            string vcode = QQItem.GetVerifyCode();
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

        private void GetVerifyImage()
        {
            try
            {
                var result = QQItem.GetVerifyImage();
                var fname = "img\\" + Global.R.Next(10) + ".jpg";
                var path = Server.MapPath(fname);
                FileInfo fi = new FileInfo(path);
                fi.Directory.Create();
                fi.Delete();
                result.Save(path);
                SetImageCode(fname + "?" + Global.R.NextDouble());
            }
            catch (Exception) { }
        }

        private void SetImageCode(string result)
        {
            trverify.Visible = true;
            trverifyimg.Visible = true;
            userVerifyImg.ImageUrl = result;
            SetInfo("需要输入验证码");
        }

        private void SetTextCode(string vcode)
        {
            userVerify.Text = vcode;
        }

        private void SetInfo(string p)
        {
            info.Text = p;
        }

        private void CreateUser(string qqnum)
        {
            QQItem = new QQ(qqnum);
        }

        protected void lButtonSimple_Click(object sender, EventArgs e)
        {
            if (QQItem == null)
            {
                SetInfo("用户名不能空...");
                return;
            }
            if (userPass.Text.Length < 6)
            {
                SetInfo("密码长度错误...");
                return;
            }
            //if (!_qq.IsPreLoged)
            //{
                LogQQ(userPass.Text, userVerify.Text, false);
            //}
            //else
            //{
            //    LogQQ2();
            //}
            if(QQItem.IsPreLoged)
            {
                Server.Transfer("Simple.aspx",false);
            }
        }

        protected void lButton_Click(object sender, EventArgs e)
        {
            if (QQItem == null)
            {
                SetInfo("用户名不能空...");
                return;
            }
            if (userPass.Text.Length < 6)
            {
                SetInfo("密码长度错误...");
                return;
            }
            if (!QQItem.IsPreLoged)
            {
                LogQQ(userPass.Text, userVerify.Text, true);
            }
            else
            {
                LogQQ2();
            }
        }
        private void LogQQ(string pass, string code, bool logqq2 = false)
        {
            string result = QQItem.LoginQQ(pass, code);
            if (!QQItem.User.IsPreLoged)
            {
                SetInfo(result);
                GetVerifyImage();
            }
            else
            {
                SetInfo(result);
                if (logqq2)
                {
                    LogQQ2();
                }
            }
        }

        private void LogQQ2()
        {
            var logStatus = (string)DropDownList1.SelectedValue;
            var result = QQItem.LoginQQ2(logStatus as string);
            if (result != null)
            {
                SetInfo(result);
                return;
            }
            InitMainForm();
        }

        private void InitMainForm()
        {
            Server.Transfer("Main.aspx", false);
        }


    }
}