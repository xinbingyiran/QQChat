<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Simple.aspx.cs" Inherits="QQChatWeb.Simple" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div style="text-align: center">
            <div>
                <asp:Label runat="server" ID="userName"></asp:Label>
            </div>
            <div>
                <asp:TextBox runat="server" ID="info" TextMode="MultiLine" Width="720px" Height="72px" ReadOnly="true"></asp:TextBox>
            </div>
            <table style="margin: auto">
                <tr>
                    <td style="width: 240px; vertical-align: top">

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                用户列表：<asp:Button runat="server" ID="refreshUser" Text="刷新" OnClick="refreshUser_Click" />
                                <div style="height: 180px; overflow: scroll">
                                    <asp:Repeater ID="userList" runat="server" OnItemCommand="userList_ItemCommand">
                                        <ItemTemplate>
                                            <p> <asp:Image ID="LinkButton1" runat="server" ImageUrl='<%#Eval("img")%>'></asp:Image>
                                                <asp:LinkButton ID="fu" runat="server" CommandName="l" CommandArgument="<%#Container.ItemIndex %>" Text='<%#Eval("name") + "[" + Eval("uin") + "]" %>'></asp:LinkButton>
                                            </p>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <asp:Button runat="server" ID="exportUser" Text="导出" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="userList" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </td>
                    <td style="width: 240px; vertical-align: top">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                群组列表：<asp:Button runat="server" ID="refreshGroup" Text="刷新" OnClick="refreshGroup_Click" />
                                <div style="height: 180px; overflow: scroll">
                                    <asp:Repeater ID="groupList" runat="server">
                                        <ItemTemplate>
                                            <div><%# Eval("groupname") %></div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <asp:Button runat="server" ID="exportGroup" Text="导出" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 240px; vertical-align: top">

                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                成员列表：<asp:Button runat="server" ID="refreshMember" Text="刷新" OnClick="refreshMember_Click" />
                                <div style="height: 180px; overflow: scroll">
                                    <asp:Repeater ID="memberList" runat="server">
                                        <ItemTemplate>
                                            <div><% Eval("Name"); %></div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <asp:Button runat="server" ID="exportMember" Text="导出" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
