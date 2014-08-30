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
                    <td style="width: 240px;">
                        <div>
                            用户列表：
                                
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:Button runat="server" ID="refreshUser" Text="刷新" OnClick="refreshUser_Click" />

                                <asp:Repeater ID="userList" runat="server" OnItemCommand="userList_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="l" CommandArgument="<%#Container.ItemIndex %>" Text='<%#Eval("name") + "[" + Eval("uin") + "]" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Button runat="server" ID="exportUser" Text="导出" />
                                    </FooterTemplate>
                                </asp:Repeater>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        </div>
                    </td>
                    <td style="width: 240px;">
                        <div>
                            群组列表：<asp:Button runat="server" ID="refreshGroup" Text="刷新" OnClick="refreshGroup_Click" />

                            <asp:Repeater ID="groupList" runat="server">
                                <ItemTemplate>
                                    <div><%# Eval("groupname") %></div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button runat="server" ID="exportGroup" Text="导出" />
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </td>
                    <td style="width: 240px;">
                        <div>
                            成员列表：<asp:Button runat="server" ID="refreshMember" Text="刷新" OnClick="refreshMember_Click" />

                            <asp:Repeater ID="memberList" runat="server">
                                <ItemTemplate>
                                    <div><% Eval("Name"); %></div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button runat="server" ID="exportMember" Text="导出" />
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
