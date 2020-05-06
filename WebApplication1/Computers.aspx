<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Computers.aspx.cs" Inherits="WebApplication1.Manage" %>

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
                    <td><asp:LinkButton ID="btnGetIpFromPcName" runat="server" Text="Get from PC Name" OnClick="btnGetIpFromPcName_Click"></asp:LinkButton></td>
                    <td><asp:Label ID="lblErrorGetIpFromPcName" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                </tr>
                <tr>
                    <td>Subnet Mask:</td>
                    <td><asp:TextBox ID="txtNewIpSubnet" runat="server" Text="255.255.255.0"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>MAC Address:</td>
                    <td><asp:TextBox ID="txtNewMacAddress" runat="server" Text="00-00-00-00-00-00"></asp:TextBox></td>
                    <td><asp:LinkButton ID="btnGetMacFromIp" runat="server" Text="Get from IP Address" OnClick="btnGetMacFromIp_Click" ></asp:LinkButton></td>
                    <td><asp:Label ID="lblErrorGetMacFromIp" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                </tr>
                <tr>
                    <td><asp:Button ID="btnAddNewPc" runat="server" Text="Add" onclick="btnAddNewPc_Click" /></td>
                </tr>
            </table>
            <p>&nbsp;</p>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                CancelSelectOnNullParameter="false" 
                SelectCommand="SELECT id, username, name, ip, subnet, mac FROM Computers ORDER BY id"
                DeleteCommand="DELETE FROM Computers where id=@id"
                UpdateCommand="
DECLARE @new_name VARCHAR(50) = @name;

DECLARE @x INT = 0;
DECLARE @i INT = 0;

WHILE (@x = 0)
BEGIN
	BEGIN TRY
		UPDATE [dbo].[Computers] SET username=@username, name=@new_name, ip=@ip, subnet=@subnet, mac=@mac WHERE id=@id
		SET @x = 1;
	END TRY
	BEGIN CATCH
		SET @x = 0;
		SET @i = @i + 1;
		SET @new_name = CONCAT(@name, ' (', @i, ')');
	END CATCH
END
                " >
                <DeleteParameters>
                    <asp:ControlParameter ControlID="ComputersGridView" Name="id" PropertyName="SelectedDataKey" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="id" Type="String" />
                    <asp:Parameter Name="username" Type="String" />
                    <asp:Parameter Name="name" Type="String" />
                    <asp:Parameter Name="ip" Type="String" />
                    <asp:Parameter Name="subnet" Type="String" />
                    <asp:Parameter Name="mac" Type="String" />
                </UpdateParameters>
            </asp:SqlDataSource>
            <asp:GridView CssClass="fullborder" ID="ComputersGridView" runat="server"
                ShowHeaderWhenEmpty="true"
                DataSourceID="SqlDataSource1" DataKeyNames="id"
                AutoGenerateColumns="false"
                AutoGenerateEditButton="false"
                AutoGenerateDeleteButton="false">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="id" ReadOnly="true" />
                    <asp:BoundField DataField="username" HeaderText="Owner" />
                    <asp:BoundField DataField="name" HeaderText="PC Name" />
                    <asp:BoundField DataField="ip" HeaderText="IP Address" />
                    <asp:BoundField DataField="subnet" HeaderText="Subnet Mask" />
                    <asp:BoundField DataField="mac" HeaderText="MAC Address" />
                    <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <asp:Button ID="btnGridRowEdit" runat="server" CausesValidation="false" CommandName="Edit" Text="Edit"/>
                            <asp:Button ID="btnGridRowDelete" runat="server" CausesValidation="false" CommandName="Delete" Text="Delete" OnCommand="btnGridRowDelete_Command" CommandArgument='<%# Eval("id") %>' />
                            <asp:Button ID="btnGridRowWakeUp" runat="server" CausesValidation="false" CommandName="Wake" Text="Wake Up" OnCommand="btnGridRowDelete_Command" CommandArgument='<%# Eval("id") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Button ID="btnGridRowUpdate" runat="server" CausesValidation="false" CommandName="Update" Text="Save" OnCommand="btnGridRowDelete_Command" CommandArgument='<%# Eval("id") %>' />
                            <asp:Button ID="btnGridRowCancel" runat="server" CausesValidation="false" CommandName="Cancel" Text="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
