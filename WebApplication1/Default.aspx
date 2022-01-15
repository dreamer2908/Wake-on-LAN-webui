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
        function jsNotice() {
            document.getElementById("lblJsNotice").remove();
        };
        window.onload = jsNotice;

        function autoreloadimages() {
            var images = document.getElementsByClassName('reload');
            for (var i = 0; i < images.length; i++) {
                images[i].src = images[i].getAttribute("data-src") + '&rand=' + Math.random();
            }
        };

        function autoRefreshPingStatus() {
            var images = document.getElementsByClassName('reload');
            for (var i = 0; i < images.length; i++) {
                getPingResult(images[i].id);
            }
        };
        autoRefreshPingStatus();
        setInterval(autoRefreshPingStatus, 5000);

        function autoUpdatePingStatus() {
            var images = document.getElementsByClassName('reload');
            for (var i = 0; i < images.length; i++) {
                status = images[i].getAttribute("data-ping-status");
                if (status == "") {
                    images[i].src = "/Images/blank-32.png";
                    images[i].title = images[i].alt = "No status";
                }
                else if (status == "0") {
                    images[i].src = "/Images/ok-32.png";
                    images[i].title = images[i].alt = "Online";
                }
                else
                {
                    images[i].src = "/Images/warning-32.png";
                    images[i].title = images[i].alt = "Offline";
                }
            }
        };
        setInterval(autoUpdatePingStatus, 1000);

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
                    document.getElementById(buttonId).textContent = "Online! Open AnyDesk.";
                    document.getElementById(buttonId).className = "ok";
                    document.getElementById(buttonId).setAttribute("onclick", "openAnyDeskPopup('" + buttonId + "'); return false;");
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
            // call the wol api
            var wolUrl = document.getElementById(buttonId).getAttribute("data-wol-url");
            touchUrl(wolUrl);

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

        function touchUrl(url) {
            // request the url, ignore any response
	        var xhttp = new XMLHttpRequest();
	        xhttp.onreadystatechange = function() {
		        if (this.readyState == 4 && this.status == 200) {
		        }
	        };
	        xhttp.open("GET", url, true);
	        xhttp.send();
        }

        function openContactPopup(buttonId) {
            var url = document.getElementById(buttonId).getAttribute("data-contact-url");
            openPopupWindow(url, "Contact");
        }

        function openAnyDeskPopup(buttonId) {
            var url = document.getElementById(buttonId).getAttribute("data-anydesk-url");
            openPopupWindow(url, "AnyDesk");
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
        <label id="lblJsNotice">Your browser doesn't support or allow Javascript. This page won't work. Please try on another browser or device.</label>
        <%--<button id="ft" data-ip="172.21.160.244" onclick="countDown('ft', 'Waiting ', 100); getHostStatus('ft'); return false;">Wake Up</button>--%>
        <asp:CheckBox ID="chbShowAllPcs" Text="Show All Computers From All Users" runat="server" Checked="false" OnCheckedChanged="chbShowAllPcs_CheckedChanged" AutoPostBack="true" Visible="false"></asp:CheckBox>
        <p id="displayAsUsernameTool" runat="server">Display as username:
            <span>&nbsp;<asp:TextBox ID="txtUsername" runat="server" Width="20em"></asp:TextBox></span>
            <span>&nbsp;<asp:Button ID="btnShowAsUsername" runat="server" Text="Apply" OnClick="btnShowAsUsername_Click" /></span>
        </p>
        <asp:Table ID="pcTable" runat="server" style="margin-left: 0; margin-right: auto;">
            <asp:TableHeaderRow style="font-weight: bold;">
                <asp:TableCell>Status</asp:TableCell>
                <asp:TableCell>PC Name</asp:TableCell>
                <asp:TableCell>IP Address</asp:TableCell>
                <asp:TableCell>Subnet</asp:TableCell>
                <asp:TableCell>MAC Address</asp:TableCell>
                <asp:TableCell>AnyDesk ID</asp:TableCell>
                <asp:TableCell>Action</asp:TableCell>
            </asp:TableHeaderRow>
        </asp:Table>
        <iframe id="my_iframe" style="display:none;"></iframe>
    </form>
</body>
</html>
