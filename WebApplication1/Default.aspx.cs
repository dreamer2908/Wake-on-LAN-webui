using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            Sessions.readSession(sessionId, out Sessions.session ses);
            string username = ses.username;
            bool isAdmin = ses.isAdmin;

            if (!ses.isLoggedIn)
            {
                redirectToLogin();
                return;
            }
            else
            {
                lblUsername.Text = username;

                // display the manage link if it's an admin
                spanAdminLink.Visible = isAdmin;
                // chbShowAllPcs.Visible = isAdmin;

                loadPcList(username);
            }
        }

        private bool canShowAllPc()
        {
            string sessionId = this.Session.SessionID;
            Sessions.readSession(sessionId, out Sessions.session ses);
            string username = ses.username;
            bool isAdmin = ses.isAdmin;
            return isAdmin;
        }

        private void loadPcList(string username)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script  type='text/javascript'>");

            // read list of PCs
            string sqlcommand = "SELECT * FROM Computers WHERE username=@username COLLATE SQL_Latin1_General_CP1_CI_AS ORDER BY id";
            if (chbShowAllPcs.Checked && canShowAllPc())
            {
                sqlcommand = "SELECT * FROM Computers ORDER BY id";
            }
            SqlCommand cmd = new SqlCommand(sqlcommand);
            cmd.Parameters.AddWithValue("@username", username);

            int rows = common.queryDatabase(cmd, out DataTable dt);
            string sendToMode = common.readSettingDatabase("sendto", 0, 0, 3).ToString();
            string countdown = common.readSettingDatabase("countdown", 100, 0, 65536).ToString();

            while (pcTable.Rows.Count > 1) pcTable.Rows.RemoveAt(1);

            foreach (DataRow pc in dt.Rows)
            {
                string pcname = pc.ItemArray[2].ToString();
                string ip = pc.ItemArray[3].ToString();
                string subnet = pc.ItemArray[4].ToString();
                string mac = pc.ItemArray[5].ToString();
                string anydesk = pc.ItemArray[6].ToString();
                String uuid = Guid.NewGuid().ToString().Replace("-", string.Empty);

                TableRow row = new TableRow();
                // icon
                TableCell cell0 = new TableCell();
                cell0.Text = string.Format("<img class='reload' data-src='/api/ping?ip={0}' src='/Images/blank-32.png' id='img-{1}' data-ip='{0}' data-ping-status='' />", ip, uuid);
                row.Cells.Add(cell0);
                // pc name
                TableCell cell1 = new TableCell();
                cell1.Text = pcname;
                row.Cells.Add(cell1);
                // ip address
                TableCell cell2 = new TableCell();
                cell2.Text = ip;
                row.Cells.Add(cell2);
                // subnet
                TableCell cell3 = new TableCell();
                cell3.Text = subnet;
                row.Cells.Add(cell3);
                // mac
                TableCell cell4 = new TableCell();
                cell4.Text = mac;
                row.Cells.Add(cell4);
                // anydesk
                TableCell cellAnyDesk = new TableCell();
                cellAnyDesk.Text = anydesk;
                row.Cells.Add(cellAnyDesk);

                // action
                TableCell cell5 = new TableCell();
                string wolurl = string.Format("/api/wol?mac={0}&ip={1}&subnet={2}&pcname={3}&username={4}&sendto={5}", mac, ip, subnet, pcname, username, sendToMode);
                string contacturl = string.Format("/Contact.aspx?username={2}&name={2}&message=My%20computer%20won%27t%20wake%20up.%20Name%20%3D%20{3}.%20IP%20%3D%20{1}.%20MAC%20%3D%20{4}.&subject=Computer%20{3}%20Failed", uuid, ip, username, pcname, mac);
                string anydeskurl = "anydesk:" + anydesk.Replace(" ", string.Empty);
                cell5.Text = string.Format("<button id='{0}' data-ip='{1}' data-wol-url='{2}' data-contact-url='{3}' data-anydesk-url='{5}' onclick=\"countDown('{0}', 'Waiting ', {4}); getHostStatus('{0}'); return false; \">Wake Up</button>", uuid, ip, wolurl, contacturl, countdown, anydeskurl);
                row.Cells.Add(cell5);

                pcTable.Rows.Add(row);
            }

            sb.AppendLine("</script>");
            jsCode.Text = sb.ToString();
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

        protected void chbShowAllPcs_CheckedChanged(object sender, EventArgs e)
        {
            loadPcList(string.Empty);
        }
    }
}