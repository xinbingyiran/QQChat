<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="QQChatWeb.Login" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center">
            <asp:Button ID="userNumButton" runat="server" Visible="false" OnClick="userNumButton_Click" />

            <table style="width: 400px; text-align: right; margin: auto">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="info" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>用户名：
                    </td>
                    <td>
                        <asp:TextBox ID="userNum" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>密码：
                    </td>
                    <td>
                        <asp:TextBox ID="userPass" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trverify" runat="server">
                    <td>验证码：
                    </td>
                    <td>
                        <asp:TextBox ID="userVerify" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trverifyimg" runat="server">
                    <td>&nbsp;
                    </td>
                    <td>
                        <asp:Image ID="userVerifyImg" runat="server" Width="140" Height="60" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList1" runat="server" Height="16" Width="120">

                        </asp:DropDownList>
                        <asp:Button ID="lButtonSimple" runat="server" Text="浅登录" OnClick="lButtonSimple_Click" />
                        <asp:Button ID="lButton" runat="server" Text="登录" OnClick="lButton_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
