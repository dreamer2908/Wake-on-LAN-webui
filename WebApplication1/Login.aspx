<%@ Page Language="C#" UnobtrusiveValidationMode="None" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication1.Login_demo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>  
    <form id="form1" runat="server">  
        <div >  
            <table style="margin-left: 0; margin-right: auto;">
                <tr><td style="text-align: center"><strong>Login Form</strong></td></tr>  
                <tr><td>&nbsp;</td></tr>
                <tr><td>Username:</td></tr>  
                <tr>
                    <td><asp:TextBox ID="TextBox1" runat="server"></asp:TextBox></td>   
                </tr>
                <tr> 
                    <td>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox1" ErrorMessage="Please Enter Your Username" ForeColor="Red"></asp:RequiredFieldValidator>  
                    </td>  
                </tr>
                <tr><td>Password:</td></tr>  
                <tr>
                    <td><asp:TextBox ID="TextBox2" TextMode="Password" runat="server"></asp:TextBox></td>    
                </tr>
                <tr>
                    <td>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox2" ErrorMessage="Please Enter Your Password" ForeColor="Red"></asp:RequiredFieldValidator>  
                    </td>  
                </tr>
                <tr>
                    <td><asp:Button ID="Button1" runat="server" Text="Log In" onclick="Button1_Click" /></td>
                </tr>
                <tr> 
                    <td><asp:Label ID="Label1" runat="server"></asp:Label></td>  
                </tr>  
                <tr style="display: none;">  
                    <td class="style2">SessionID = <asp:Label ID="label2" runat="server" Text="sessionId" /></td> 
                </tr>  
            </table>  
        </div>  
    </form>  
</body> 

</html>