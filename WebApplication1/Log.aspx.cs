using System;
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
    public partial class Log : System.Web.UI.Page
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

        protected void lnkLogOut_Click(object sender, EventArgs e)
        {
            redirectToLogin();
        }

        public void redirectToLogin()
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

        protected void btnDeleteLog_Click(object sender, EventArgs e)
        {
            string logId = txtDeleteLogId.Text;

            SqlCommand cmd = new SqlCommand("DELETE FROM Log WHERE id <= @id");
            cmd.Parameters.AddWithValue("@id", logId);

            int rows = common.queryDatabase(cmd, out DataTable dt);

            common.writeLog(lblUsername.Text, "Delete Log", "Delete log upto id " + logId);

            reloadPage();
        }

        private void reloadPage()
        {
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }

        protected void lnkToUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }
    }
}