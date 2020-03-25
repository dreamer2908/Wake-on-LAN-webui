using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Web.Configuration;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
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
            bool isAdmin = false;

            string username = TextBox1.Text;
            string password = TextBox2.Text;

            // create a "principal context" - e.g. your domain (could be machine, too)
            string domainServer = WebConfigurationManager.ConnectionStrings["domainServer"].ConnectionString;
            string domainUsername = WebConfigurationManager.ConnectionStrings["domainUsername"].ConnectionString;
            string domainPassword = WebConfigurationManager.ConnectionStrings["domainPassword"].ConnectionString;
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainServer, domainUsername, domainPassword))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(username, password, ContextOptions.Negotiate);

                if (isValid)
                {
                    // set as admin if the user is in "Enterprise Admins" group
                    // won't work unless a valid username and password are specified for the principal context
                    UserPrincipal user = UserPrincipal.FindByIdentity(pc, username);
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, "Enterprise Admins");
                    if (user != null && user.IsMemberOf(group))
                    {
                        isAdmin = true;
                    }
                }
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

                Sessions.writeSession(sessionId, true, username, isAdmin);
                common.writeLog(username, "Login", "Login OK");
                Response.Redirect("Default.aspx");
            }
            else
            {
                common.writeLog(string.Empty, "Login", "Login failure: " + username);
                Label1.Text = "Your username and password is incorrect";
                Label1.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}