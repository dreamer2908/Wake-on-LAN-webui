<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="WebApplication1.Log" %>

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
            <p>Logged in as <asp:Label ID="lblUsername" runat="server" Text="Username" />.&nbsp;
                [<asp:LinkButton ID="lnkLogOut" runat="server" Text="Log out" onclick="lnkLogOut_Click" />]&nbsp;
                [<asp:LinkButton ID="lnkToMain" runat="server" Text="Main" onclick="lnkToMain_Click" />]&nbsp;
                [<asp:LinkButton ID="lnkToManage" runat="server" Text="Manage" OnClick="lnkToManage_Click" />]
            </p>
            <p>&nbsp;</p>
            <table style="margin-left: 0; margin-right: auto; border: none;">
                <tr>
                    <td>Delete log upto id </td>
                    <td><asp:TextBox ID="txtDeleteLogId" runat="server" Text="1"></asp:TextBox></td>
                    <td><asp:Button ID="btnDeleteLog" runat="server" Text="Delete" OnClick="btnDeleteLog_Click" /></td>
                </tr>
            </table>
            <p>&nbsp;</p>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                CancelSelectOnNullParameter="false" 
                SelectCommand="SELECT * FROM Log ORDER BY id"
                >
            </asp:SqlDataSource>
            <asp:GridView CssClass="fullborder" ID="LogGridView" runat="server"
                DataSourceID="SqlDataSource1" DataKeyNames="id"
                ShowHeaderWhenEmpty="true"
                AutoGenerateColumns="false"
                >
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="id" ReadOnly="true" />
                    <asp:BoundField DataField="timestamp" HeaderText="Timestamp" DataFormatString="{0:yyyy-MM-dd hh:mm:ss}" />
                    <asp:BoundField DataField="ip" HeaderText="Source IP" />
                    <asp:BoundField DataField="username" HeaderText="Username" />
                    <asp:BoundField DataField="action" HeaderText="Action" />
                    <asp:BoundField DataField="detail" HeaderText="Detail" HtmlEncode="false" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
