using System;
using System.Collections.Generic;
using System.Data;
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
            string username = ses.username;

            // redirect to login page if either not login or not admin
            if (!(ses.isLoggedIn && ses.isAdmin))
            {
                redirectToLogin(sessionId);
                return;
            }
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

        protected void btnDeleteComputer_click(object sender, EventArgs e)
        {
            // delete record with id
            string recordId = txtDeleteRecordId.Text;

            SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\Database2.mdf;Integrated Security=True;User Instance=True");
            SqlCommand cmd = new SqlCommand("DELETE FROM Computers where id=@id", con);
            cmd.Parameters.AddWithValue("@id", recordId);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            reloadPage();
        }

        protected void btnAddNewPc_Click(object sender, EventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPcName = txtNewPcName.Text;
            string newIp = txtNewIpAddress.Text;
            string newSubnet = txtNewIpSubnet.Text;
            string newMac = txtNewMacAddress.Text;

            SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\Database2.mdf;Integrated Security=True;User Instance=True");
            SqlCommand cmd = new SqlCommand("INSERT INTO Computers ([username], [name], [ip], [subnet], [mac]) VALUES (@username, @name, @ip, @subnet, @mac)", con);
            cmd.Parameters.AddWithValue("@username", newUsername);
            cmd.Parameters.AddWithValue("@name", newPcName);
            cmd.Parameters.AddWithValue("@ip", newIp);
            cmd.Parameters.AddWithValue("@subnet", newSubnet);
            cmd.Parameters.AddWithValue("@mac", newMac);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            reloadPage();
        }

        private void reloadPage()
        {
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }
    }
}