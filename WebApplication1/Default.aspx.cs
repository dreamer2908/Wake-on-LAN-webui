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
    public partial class Redirectpage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            Sessions.readSession(sessionId, out Sessions.session ses);
            string username = ses.username;

            if (!ses.isLoggedIn)
            {
                redirectToLogin(sessionId);
                return;
            }
            else
            {
                label1.Text = username;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<script  type='text/javascript'>");

                // read list of PCs

                SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\Database2.mdf;Integrated Security=True;User Instance=True");
                SqlCommand cmd = new SqlCommand("select * from Computers where username=@username", con);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();


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
                    cell5.Text = string.Format("<button id='{0}' onClick='f_{0}()'>Wake Up</a>", uuid);
                    sb.AppendLine(string.Format("function f_{0}(){{", uuid));
                    sb.AppendLine(string.Format("document.getElementById('my_iframe').src = '/api/wol?mac={0}&ip={1}&subnet={2}'", mac, ip, subnet));
                    sb.AppendLine(string.Format("document.getElementById('{0}').textContent = '{1}'", uuid, "Sent OK"));
                    sb.AppendLine("}");
                    row.Cells.Add(cell5);

                    pcTable.Rows.Add(row);
                }

                sb.AppendLine("</script>");
                jsCode.Text = sb.ToString();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string mac = "6C-F0-49-68-07-00";
            string ip = "172.21.160.244";
            string subnet = "255.255.255.0";
            wol.wake(mac, ip, subnet);
            Button1.Text = "Online soon";
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
    }
}