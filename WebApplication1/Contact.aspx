<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="WebApplication1.Contact" UnobtrusiveValidationMode="None" %>

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
            <p id="pLogin" runat="server" visible="false">Logged in as <asp:Label ID="lblUsername" runat="server" Text="Username" />.
                &nbsp;[<asp:LinkButton ID="lnkLogout" runat="server" Text="Log out" onclick="lnkLogout_Click" />]
                <span id="spanAdminLink" runat="server">
                    &nbsp;[<asp:LinkButton ID="lnkToMain" runat="server" Text="Main" onclick="lnkToMain_Click" />]
                    &nbsp;[<asp:LinkButton ID="lnkToComputer" runat="server" Text="Computers" onclick="lnkToComputer_Click" />]
                    &nbsp;[<asp:LinkButton ID="lnkToUser" runat="server" Text="Users" onclick="lnkToUser_Click" />]
                    &nbsp;[<asp:LinkButton ID="lnkToOptions" runat="server" Text="Options" onclick="lnkToOptions_Click" />]
                    &nbsp;[<asp:LinkButton ID="lnkToLog" runat="server" Text="Log" OnClick="lnkToLog_Click"></asp:LinkButton>]
                </span>
                &nbsp;[<asp:LinkButton ID="lnkToContact" runat="server" Text="Contact" OnClick="lnkToContact_Click"></asp:LinkButton>]
            </p>
            <p id="pNotLogin" runat="server" visible="false">You are not logged in. &nbsp;[<asp:LinkButton ID="LinkButton1" runat="server" Text="Back to Main" onclick="lnkToMain_Click" />]</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td colspan="2"><b>Send an Email to IT Team</b> (<span id="spanItEmail" runat="server"></span>)</td>
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>Your Name: </td>
                    <td><asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter your name" ForeColor="Red" ValidationGroup="Send"></asp:RequiredFieldValidator>  
                    </td>
                </tr>
                <tr>
                    <td>Your Email: </td>
                    <td><asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Please enter your email address" ForeColor="Red" ValidationGroup="Send"></asp:RequiredFieldValidator>  
                    </td>
                </tr>
                <tr>
                    <td>Your Phone Number: </td>
                    <td><asp:TextBox ID="txtPhoneNumber" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPhoneNumber" ErrorMessage="Please enter your phone number" ForeColor="Red" ValidationGroup="Send"></asp:RequiredFieldValidator>  
                    </td>
                </tr>
                <tr>
                    <td>Subject: </td>
                    <td><asp:TextBox ID="txtSubject" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtSubject" ErrorMessage="Please enter your email subject" ForeColor="Red" ValidationGroup="Send"></asp:RequiredFieldValidator>  
                    </td>
                </tr>
                <tr>
                    <td>Message: </td>
                    <td><asp:TextBox TextMode="MultiLine" runat="server" ID="txtMessage" width="500px" height="250px" wrap="true"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtMessage" ErrorMessage="Please enter your message" ForeColor="Red" ValidationGroup="Send"></asp:RequiredFieldValidator>  
                    </td>
                </tr>
                <tr><td>(All fields are required.)</td></tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" ValidationGroup="Send" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
