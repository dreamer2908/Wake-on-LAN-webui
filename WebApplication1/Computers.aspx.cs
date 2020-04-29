using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

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

        protected void btnGetIpFromPcName_Click(object sender, EventArgs e)
        {
            string host = txtNewPcName.Text;
            bool found = false;
            found = common.getIpfromHost(host, out string ip);

            if (found)
            {
                txtNewIpAddress.Text = ip.ToString();
                lblErrorGetIpFromPcName.Text = string.Empty;
            }
            else
            {
                lblErrorGetIpFromPcName.Text = "Hostname not found";
            }

        }

        protected void btnGetMacFromIp_Click(object sender, EventArgs e)
        {
            string ip = txtNewIpAddress.Text;
            bool found = false;
            found = common.getMacFromIp(ip, out string mac);

            if (found)
            {
                txtNewMacAddress.Text = mac;
                lblErrorGetMacFromIp.Text = string.Empty;
            }
            else
            {
                lblErrorGetMacFromIp.Text = "Not found in ARP";
            }
        }
    }
}