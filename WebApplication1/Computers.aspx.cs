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

            if (btnAddNewPc.Text == "Add")
            {
                txtNewUsername.Text = username;
            }

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
            string newAnydesk = txtNewAnyDeskId.Text;
            string id = txtComputerId.Text;
            bool update = (btnAddNewPc.Text == "Update");

            SqlCommand cmd;
            if (update)
            {
                cmd = new SqlCommand(@"
DECLARE @new_name VARCHAR(50) = @name;

                DECLARE @x INT = 0;
                DECLARE @i INT = 0;

                WHILE(@x = 0)
BEGIN
    BEGIN TRY
        UPDATE[dbo].[Computers] SET username = @username, name = @new_name, ip = @ip, subnet = @subnet, mac = @mac, anydesk = @anydesk WHERE id = @id

        SET @x = 1;
        END TRY

    BEGIN CATCH

        SET @x = 0;
        SET @i = @i + 1;
        SET @new_name = CONCAT(@name, ' (', @i, ')');
        END CATCH
END
                ");
            }
            else
            {
                cmd = new SqlCommand("INSERT INTO Computers ([username], [name], [ip], [subnet], [mac], [anydesk]) VALUES (@username, @name, @ip, @subnet, @mac, @anydesk)");
            }
            cmd.Parameters.AddWithValue("@username", newUsername);
            cmd.Parameters.AddWithValue("@name", newPcName);
            cmd.Parameters.AddWithValue("@ip", newIp);
            cmd.Parameters.AddWithValue("@subnet", newSubnet);
            cmd.Parameters.AddWithValue("@mac", newMac);
            cmd.Parameters.AddWithValue("@anydesk", newAnydesk);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = common.queryDatabase(cmd, out DataTable dt);

            Sessions.readSession(this.Session.SessionID, out Sessions.session ses);
            string action = update ? "Update PC id = " + id + "," : "Add PC";
            common.writeLog(ses.username, "Computers", string.Format("{4} username = {0}, pcname = {1}, ip = {2}, mac = {3}", newUsername, newPcName, newIp, newMac, action));

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

        protected void btnGridRowDelete_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            Sessions.readSession(this.Session.SessionID, out Sessions.session ses);
            bool verbose = common.readSettingDatabase("verbose", false);

            if (e.CommandName == "Delete")
            {
                common.writeLog(ses.username, "Computers", "Delete computer id = " + id);

                if (verbose)
                {
                    Response.Write("<script>alert('Deleted OK!');</script>");
                }
            }
            else if (e.CommandName == "Mod")
            {
                // read PC info from the database
                string sqlcommand = "SELECT * FROM Computers WHERE id=@di";
                SqlCommand cmd = new SqlCommand(sqlcommand);
                cmd.Parameters.AddWithValue("@di", id);

                int rows = common.queryDatabase(cmd, out DataTable dt);

                if (dt.Rows.Count > 0)
                {
                    var pc = dt.Rows[0];
                    string username = pc.ItemArray[1].ToString();
                    string pcname = pc.ItemArray[2].ToString();
                    string ip = pc.ItemArray[3].ToString();
                    string subnet = pc.ItemArray[4].ToString();
                    string mac = pc.ItemArray[5].ToString();
                    string anydesk = pc.ItemArray[6].ToString();

                    // load data to above form
                    txtNewUsername.Text = username;
                    txtNewPcName.Text = pcname;
                    txtNewIpAddress.Text = ip;
                    txtNewIpSubnet.Text = subnet;
                    txtNewMacAddress.Text = mac;
                    txtNewAnyDeskId.Text = anydesk;
                    txtComputerId.Text = id;

                    btnAddNewPc.Text = "Update";
                }
            }
            else if (e.CommandName == "Wake")
            {
                common.writeLog(ses.username, "Computers", string.Format("Wake up computer id = {0}", id));

                // read PC info from the database
                string sqlcommand = "SELECT * FROM Computers WHERE id=@di";
                SqlCommand cmd = new SqlCommand(sqlcommand);
                cmd.Parameters.AddWithValue("@di", id);

                int rows = common.queryDatabase(cmd, out DataTable dt);

                if (dt.Rows.Count > 0)
                {
                    var pc = dt.Rows[0];
                    string pcname = pc.ItemArray[2].ToString();
                    string ip = pc.ItemArray[3].ToString();
                    string subnet = pc.ItemArray[4].ToString();
                    string mac = pc.ItemArray[5].ToString();

                    string username = lblUsername.Text;
                    string sendToMode = common.readSettingDatabase("sendto", 0, 0, 3).ToString();

                    // call the api without going through web address
                    var api = new Controllers.WolController();
                    api.Get(mac, ip, subnet, username, pcname, sendToMode);
                }

                if (verbose)
                {
                    Response.Write("<script>alert('Sent OK!');</script>");
                }
            }
        }

        protected void btnGetAnyDeskId_Click(object sender, EventArgs e)
        {
            string ip = txtNewIpAddress.Text;
            bool found = false;

            // TODO: find a way to do
            found = false;
            string id = string.Empty;

            if (found)
            {
                txtNewAnyDeskId.Text = id;
                lblErrorGetAnyDesk.Text = string.Empty;
            }
            else
            {
                lblErrorGetAnyDesk.Text = "Not implemented.";
            }
        }
    }
}