﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" %>

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
        button.ok {
            background-color: #008000;
            color: white;
            font-weight: bold;
            cursor: pointer;
        }
        button.failed {
            background-color: #FF0000;
            color: white;
            font-weight: bold;
            cursor: pointer;
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

        const delay = ms => new Promise(res => setTimeout(res, ms));
        const countDown = async (buttonId, pretext, max) => {
            document.getElementById(buttonId).disabled = true; // lock the button until it finishes
            document.getElementById(buttonId).className = ""; // clear class
            document.getElementById(buttonId).setAttribute("data-continue", "1"); // external signal, will stop countdown when it's changed from "1"
            document.getElementById(buttonId).setAttribute("data-counting", "1"); // signal for others, "1" means it's counting, "0" after it's timeout

            var count;
            var cont;
            var status;
            for (count = max; count >= 0; count = count - 1) {
                cont = document.getElementById(buttonId).getAttribute("data-continue");
                status = document.getElementById(buttonId).getAttribute("data-ping-status");

                if (status == "0") { // stop when the host is online
                    document.getElementById(buttonId).setAttribute("data-continue", "0");
                    document.getElementById(buttonId).textContent = "Online!";
                    document.getElementById(buttonId).className = "ok";
                }
                else
                {
                    if (cont == "1") {
                        document.getElementById(buttonId).textContent = pretext + count;
                        await delay(1000);
                    } else {
                        break;
                    }
                }
            }

            if (status != "0") {
                // show a link to contact admin
                document.getElementById(buttonId).textContent = "Failed! Contact Admin.";
                document.getElementById(buttonId).className = "failed";
                document.getElementById(buttonId).setAttribute("onclick", "openContactPopup('" + buttonId + "'); return false;");
            }

            document.getElementById(buttonId).setAttribute("data-counting", "0");
            document.getElementById(buttonId).disabled = false;
        };

        const getHostStatus = async (buttonId) => {
            // get ping status again and again until countdown stops
            var counting;
            do {
                getPingResult(buttonId);
                counting = document.getElementById(buttonId).getAttribute("data-counting");
                await delay(1000);
            }
            while (counting == "1");
        }

        function getPingResult(buttonId, ip) {
            // get ping result and write to data-ping-status attr
	        var xhttp = new XMLHttpRequest();
	        xhttp.onreadystatechange = function() {
		        if (this.readyState == 4 && this.status == 200) {
                    document.getElementById(buttonId).setAttribute("data-ping-status", this.responseText);
                    document.getElementById(buttonId).setAttribute("data-ping-ip", ip);
		        }
	        };
            var ip = document.getElementById(buttonId).getAttribute("data-ip");
	        xhttp.open("GET", "/api/ping2?ip=" + ip, true);
	        xhttp.send();
        }

        function openContactPopup(buttonId) {
            var url = document.getElementById(buttonId).getAttribute("data-contact-url");
            openPopupWindow(url, "Contact");
        }

        function openPopupWindow(url, title) {
            myRef = window.open(url, title);
            myRef.focus();
        }

    </script>
    <asp:Literal ID="jsCode" runat="server"></asp:Literal>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        <p>Logged in as <asp:Label ID="lblUsername" runat="server" Text="Username" />.
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
        <%--<button id="ft" data-ip="172.21.160.244" onclick="countDown('ft', 'Waiting ', 100); getHostStatus('ft'); return false;">Wake Up</button>--%>
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
