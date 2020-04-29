<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Options.aspx.cs" Inherits="WebApplication1.Options" UnobtrusiveValidationMode="None" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        table {
            border-collapse: collapse;
            border-spacing: 5px;
        }
        table.fullborder {
            border: 1px solid black;
            text-align: left;
            padding: 5px;
            border-spacing: 5px;
            border-collapse: collapse;
        }
        th, td {
            padding: 5px;
        }
        td img {
            display: block;
            margin-left: auto;
            margin-right: auto;
            text-align: center;
            vertical-align: middle;
        }

    </style>
    <asp:Literal ID="jsCode" runat="server"></asp:Literal>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>Logged in as <asp:Label ID="lblUsername" runat="server" Text="Username" />.
                &nbsp;[<asp:LinkButton ID="lnkLogout" runat="server" Text="Log out" onclick="lnkLogout_Click" />]
                &nbsp;[<asp:LinkButton ID="lnkToMain" runat="server" Text="Main" onclick="lnkToMain_Click" />]
                &nbsp;[<asp:LinkButton ID="lnkToComputer" runat="server" Text="Computers" onclick="lnkToComputer_Click" />]
                &nbsp;[<asp:LinkButton ID="lnkToUser" runat="server" Text="Users" onclick="lnkToUser_Click" />]
                &nbsp;[<asp:LinkButton ID="lnkToOptions" runat="server" Text="Options" onclick="lnkToOptions_Click" />]
                &nbsp;[<asp:LinkButton ID="lnkToLog" runat="server" Text="Log" OnClick="lnkToLog_Click"></asp:LinkButton>]
                &nbsp;[<asp:LinkButton ID="lnkToContact" runat="server" Text="Contact" OnClick="lnkToContact_Click"></asp:LinkButton>]
            </p>
            <p>&nbsp;</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td>Send the Wake-on-LAN packet to</td>
                    <td>
                        <asp:DropDownList ID="ddlSendWolPackageTo" runat="server">
                            <asp:ListItem Text="All" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Broadcast Address 255.255.255.255" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Broadcast Address According to IP Address" Value="2"></asp:ListItem>
                            <asp:ListItem Text="IP Address of the device" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <p>&nbsp;</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td><b>Mail Server</b></td>
                </tr>
                <tr>
                    <td>Sender: </td>
                    <td><asp:TextBox ID="txtSender" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Receivers: </td>
                    <td><asp:TextBox ID="txtReceivers" runat="server"></asp:TextBox></td>
                    <td></td>
                </tr>
                <tr>
                    <td>SMTP Server: </td>
                    <td><asp:TextBox ID="txtSmtpServer" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Port: </td>
                    <td><asp:TextBox ID="txtPort" TextMode="Number" runat="server" min="0" max="65535" step="1"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>SSL: </td>
                    <td><asp:CheckBox ID="chbUseSsl" runat="server"></asp:CheckBox></td>
                </tr>
                <tr>
                    <td>Authentication: </td>
                    <td><asp:CheckBox ID="chbAuthenticationRequired" runat="server"></asp:CheckBox></td>
                </tr>
                <tr>
                    <td>Username: </td>
                    <td><asp:TextBox ID="txtUsername" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Password: </td>
                    <td><asp:TextBox ID="txtPassword" TextMode="Password" runat="server"></asp:TextBox></td>
                </tr>
            </table>
            <p>&nbsp;</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td>
                        <asp:Button ID="btnApplyOptions" runat="server" Text="Apply" OnClick="btnApplyOptions_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
