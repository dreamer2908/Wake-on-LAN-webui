<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        table {
            border-collapse: collapse;
            border-spacing: 5px;
        }
        table, th, td {
            border: 1px solid black;
            text-align: left;
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
    <script type="text/javascript">
        function autoreloadimages() {
            var images = document.getElementsByClassName('reload');
            for (var i = 0; i < images.length; i++) {
                images[i].src = images[i].getAttribute("data-src") + '&rand=' + Math.random();
            }
        };
        setInterval(autoreloadimages, 5000);
    </script>
    <asp:Literal ID="jsCode" runat="server"></asp:Literal>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        <p>Logged in as <asp:Label ID="lblUsername" runat="server" Text="Username" />.
            &nbsp;[<asp:LinkButton ID="lnkLogout" runat="server" Text="Log out" onclick="lnkLogout_Click" />]
            &nbsp;<asp:LinkButton ID="lnkToManage" Visible="false" Enabled ="false" runat="server" Text="[Computers]" onclick="lnkToManage_Click" />
            &nbsp;<asp:LinkButton ID="lnkToUser" Visible="false" Enabled ="false" runat="server" Text="[Users]" onclick="lnkToUser_Click" />
            &nbsp;<asp:LinkButton ID="lnkToLog" runat="server" Visible="false" Enabled="false" Text="[Log]" OnClick="lnkToLog_Click"></asp:LinkButton>
        </p>
        <asp:Table ID="pcTable" runat="server" style="margin-left: 0; margin-right: auto;">
            <asp:TableRow style="font-weight: bold;">
                <asp:TableCell>Status</asp:TableCell>
                <asp:TableCell>PC Name</asp:TableCell>
                <asp:TableCell>IP Address</asp:TableCell>
                <asp:TableCell>Subnet</asp:TableCell>
                <asp:TableCell>MAC Address</asp:TableCell>
                <asp:TableCell>Action</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <iframe id="my_iframe" style="display:none;"></iframe>
    </form>
</body>
</html>
