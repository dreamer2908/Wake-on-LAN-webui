using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;

namespace WebApplication1
{
    public partial class Login_demo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            label2.Text = sessionId;

            //add a sessionid Cookie
            Response.Cookies["sessionid"].Value = sessionId;
            Response.Cookies["sessionid"].Expires = DateTime.Now.AddDays(30);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool isValid = false;

            string username = TextBox1.Text;
            string password = TextBox2.Text;

            // create a "principal context" - e.g. your domain (could be machine, too)
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "PRS-VN"))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(username, password, ContextOptions.Negotiate);
            }

            if (isValid)
            {
                string sessionId = this.Session.SessionID;
                //if (Request.Cookies["sessionid"] != null)
                //{
                //    sessionId = Server.HtmlEncode(Request.Cookies["sessionid"].Value);
                //}
                //else
                //{
                //    //add a sessionid Cookie
                //    Response.Cookies["sessionid"].Value = sessionId;
                //    Response.Cookies["sessionid"].Expires = DateTime.Now.AddDays(30);
                //}

                Sessions.writeSession(sessionId, true, username);
                Response.Redirect("Default.aspx");
            }
            else
            {
                Label1.Text = "Your username and password is incorrect";
                Label1.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}