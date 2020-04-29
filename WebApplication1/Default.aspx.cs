﻿using System;
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
                lnkToManage.Enabled = lnkToManage.Visible = isAdmin;
                lnkToLog.Enabled = lnkToLog.Visible = isAdmin;
                lnkToUser.Enabled = lnkToUser.Visible = isAdmin;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<script  type='text/javascript'>");

                // read list of PCs

                SqlCommand cmd = new SqlCommand("SELECT * FROM Computers WHERE username=@username ORDER BY id");
                cmd.Parameters.AddWithValue("@username", username);

                int rows = common.queryDatabase(cmd, out DataTable dt);
                string sendToMode = common.readSettingDatabase_sendTo();

                foreach (DataRow pc in dt.Rows)
                {
                    string pcname = pc.ItemArray[2].ToString();
                    string ip = pc.ItemArray[3].ToString();
                    string subnet = pc.ItemArray[4].ToString();
                    string mac = pc.ItemArray[5].ToString();
                    TableRow row = new TableRow();
                    // icon
                    TableCell cell0 = new TableCell();
                    cell0.Text = string.Format("<img class='reload' data-src='/api/ping?ip={0}' src='/api/ping?ip={0}' />", ip);
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

                    // action
                    String uuid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    TableCell cell5 = new TableCell();
                    cell5.Text = string.Format("<button id='{0}' onClick='f_{0}(); return false;'>Wake Up</a>", uuid);
                    sb.AppendLine(string.Format("function f_{0}(){{", uuid));
                    sb.AppendLine(string.Format("document.getElementById('my_iframe').src = '/api/wol?mac={0}&ip={1}&subnet={2}&pcname={3}&username={4}&sendto={5}'", mac, ip, subnet, pcname, username, sendToMode));
                    sb.AppendLine(string.Format("document.getElementById('{0}').textContent = '{1}'", uuid, "Sent OK"));
                    sb.AppendLine("}");
                    row.Cells.Add(cell5);

                    pcTable.Rows.Add(row);
                }

                sb.AppendLine("</script>");
                jsCode.Text = sb.ToString();
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            redirectToLogin();
        }

        private void redirectToLogin()
        {
            Response.Redirect("Logout.aspx");
        }

        protected void lnkToManage_Click(object sender, EventArgs e)
        {
            Response.Redirect("Manage.aspx");
        }

        protected void lnkToUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }

        protected void lnkToLog_Click(object sender, EventArgs e)
        {
            Response.Redirect("Log.aspx");
        }
    }
}