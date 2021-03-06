﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            // use the session cookie if available
            if (Request.Cookies["session"] != null)
            {
                sessionId = Request.Cookies["session"].Value;
            }
            Sessions.readSession(sessionId, out Sessions.session ses);

            // redirect to login page if either not login or not admin
            if (!(ses.isLoggedIn && ses.isAdmin))
            {
                redirectToLogin();
                return;
            }

            string username = ses.username;
            lblUsername.Text = username;

            SqlDataSource1.ConnectionString = WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;


        }

        #region admin links
        private void reloadPage()
        {
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            redirectToLogin();
        }

        private void redirectToLogin()
        {
            Response.Redirect("Logout.aspx");
        }

        protected void lnkToMain_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void lnkToComputer_Click(object sender, EventArgs e)
        {
            Response.Redirect("Computers.aspx");
        }

        protected void lnkToUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }

        protected void lnkToOptions_Click(object sender, EventArgs e)
        {
            Response.Redirect("Options.aspx");
        }

        protected void lnkToLog_Click(object sender, EventArgs e)
        {
            Response.Redirect("Log.aspx");
        }

        protected void lnkToContact_Click(object sender, EventArgs e)
        {
            Response.Redirect("Contact.aspx");
        }
        #endregion

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPassword = common.getSha1HashFromText(txtNewPassword.Text);
            string isAdmin = ddlNewIsAdmin.SelectedItem.Value;

            SqlCommand cmd = new SqlCommand(@"
declare @kye varchar(50)
declare @val varchar(50)
declare @val2 varchar(50)
set @kye = @username
set @val = @password
set @val2 = @admin

IF (NOT EXISTS(SELECT * FROM [dbo].[Users] where username = @kye)) 
BEGIN
    INSERT INTO [dbo].[Users] ([username], [password], [admin]) VALUES (@kye, @val, @val2)
END 
ELSE 
BEGIN 
    UPDATE [dbo].[Users] 
    SET [password] = @val, [admin]=@val2
    WHERE [username] = @kye
END ");
            cmd.Parameters.AddWithValue("@username", newUsername);
            cmd.Parameters.AddWithValue("@password", newPassword);
            cmd.Parameters.AddWithValue("@admin", isAdmin);

            int rows = common.queryDatabase(cmd, out DataTable dt);

            Sessions.readSession(this.Session.SessionID, out Sessions.session ses);
            common.writeLog(ses.username, "Users", "Add/Edit user " + newUsername);

            reloadPage();
        }

        protected void btnGridRowDelete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                string username = e.CommandArgument.ToString();
                Sessions.readSession(this.Session.SessionID, out Sessions.session ses);
                common.writeLog(ses.username, "Users", "Delete user " + username);

                reloadPage();
            }
        }
    }
}