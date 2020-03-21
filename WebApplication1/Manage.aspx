<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="WebApplication1.Manage" %>

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
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>Logged in as <asp:Label ID="label1" runat="server" Text="Username" />.  [<asp:LinkButton ID="Button2" runat="server" Text="Log out" onclick="Button2_Click" />] [<asp:LinkButton ID="LinkButton3" runat="server" Text="Main" onclick="Button3_Click" />]</p>
            <p>&nbsp;</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td colspan="2"><b>Delete a computer:</b></td>
                </tr>
                <tr>
                    <td>Record Id</td>
                    <td><asp:TextBox ID="txtDeleteRecordId" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Button ID="btnDeleteComputer" runat="server" Text="Delete" onclick="btnDeleteComputer_click" /></td>
                </tr>
            </table>
            <p>&nbsp;</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td colspan="2"><b>Add a new computer:</b></td>
                </tr>
                <tr>
                    <td>Owner:</td>
                    <td><asp:TextBox ID="txtNewUsername" runat="server" Text="prsvn"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>PC Name:</td>
                    <td><asp:TextBox ID="txtNewPcName" runat="server" Text="PC-xxx"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>IP Address:</td>
                    <td><asp:TextBox ID="txtNewIpAddress" runat="server" Text="172.21.160.100"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Subnet Mask:</td>
                    <td><asp:TextBox ID="txtNewIpSubnet" runat="server" Text="255.255.255.0"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>MAC Address:</td>
                    <td><asp:TextBox ID="txtNewMacAddress" runat="server" Text="00-00-00-00-00-00"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><asp:Button ID="btnAddNewPc" runat="server" Text="Add" onclick="btnAddNewPc_Click" /></td>
                </tr>
            </table>
            <p>&nbsp;</p>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                ConnectionString="Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\Database2.mdf;Integrated Security=True;User Instance=True" 
                CancelSelectOnNullParameter="false" 
                SelectCommand="select * from Computers">
                <SelectParameters>
                    <asp:QueryStringParameter Name="id" QueryStringField="id" />
                    <asp:QueryStringParameter Name="username" QueryStringField="username" />
                    <asp:QueryStringParameter Name="name" QueryStringField="name" />
                    <asp:QueryStringParameter Name="ip" QueryStringField="ip" />
                    <asp:QueryStringParameter Name="subnet" QueryStringField="subnet" />
                    <asp:QueryStringParameter Name="mac" QueryStringField="mac" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:GridView CssClass="fullborder" ID="ComputersGridView" runat="server" DataSourceID="SqlDataSource1" AutoGenerateColumns="True"></asp:GridView>
        </div>
    </form>
</body>
</html>
