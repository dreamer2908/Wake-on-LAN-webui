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
        </div>
    </form>
</body>
</html>
