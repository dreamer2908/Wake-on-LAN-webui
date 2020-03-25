using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Manage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            Sessions.readSession(sessionId, out Sessions.session ses);

            // redirect to login page if either not login or not admin
            if (!(ses.isLoggedIn && ses.isAdmin))
            {
                redirectToLogin(sessionId);
                return;
            }

            string username = ses.username;
            label1.Text = username;

            SqlDataSource1.ConnectionString = WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            redirectToLogin(sessionId);
        }

        private void redirectToLogin(string id)
        {
            Sessions.deleteSession(id);
            this.Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnAddNewPc_Click(object sender, EventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPcName = txtNewPcName.Text;
            string newIp = txtNewIpAddress.Text;
            string newSubnet = txtNewIpSubnet.Text;
            string newMac = txtNewMacAddress.Text;

            SqlCommand cmd = new SqlCommand("INSERT INTO Computers ([username], [name], [ip], [subnet], [mac]) VALUES (@username, @name, @ip, @subnet, @mac)");
            cmd.Parameters.AddWithValue("@username", newUsername);
            cmd.Parameters.AddWithValue("@name", newPcName);
            cmd.Parameters.AddWithValue("@ip", newIp);
            cmd.Parameters.AddWithValue("@subnet", newSubnet);
            cmd.Parameters.AddWithValue("@mac", newMac);

            int rows = common.queryDatabase(cmd, out DataTable dt);

            reloadPage();
        }

        private void reloadPage()
        {
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }
    }
}