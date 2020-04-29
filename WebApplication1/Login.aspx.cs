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
        }

        private static bool checkWebsiteLogin(string username, string _password, ref bool isAdmin)
        {
            bool isValid = false;
            isAdmin = false;

            string password = common.getSha1HashFromText(_password);

            SqlCommand cmd = new SqlCommand("SELECT [username], [password], [admin] FROM [dbo].[Users] WHERE [username] = @username AND [password] = @password;");
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            int rows = common.queryDatabase(cmd, out DataTable dt);

            if (dt.Rows.Count > 0)
            {
                isValid = true;
                isAdmin = (dt.Rows[0].ItemArray[2].ToString() == 1.ToString());
            }

            return isValid;
        }

        private static bool checkDomainLogin(string username, string password, ref bool isAdmin)
        {
            bool isValid;
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

            return isValid;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool isValid = false;
            bool isAdmin = false;

            string username = TextBox1.Text;
            string password = TextBox2.Text;

            switch (ddlAuthentication.SelectedItem.Value)
            {
                case "0": isValid = checkDomainLogin(username, password, ref isAdmin); break;
                case "1": isValid = checkWebsiteLogin(username, password, ref isAdmin); break;
            }

            if (isValid)
            {
                string sessionId = this.Session.SessionID;

                Sessions.writeSession(sessionId, true, username, isAdmin);
                common.writeLog(username, "Login", "Login OK. Authentication = " + ddlAuthentication.SelectedItem.Text);
                Response.Redirect("Default.aspx");
            }
            else
            {
                common.writeLog(string.Empty, "Login", "Login failure: " + username + ". Authentication = " + ddlAuthentication.SelectedItem.Text);
                Label1.Text = "Your username and password is incorrect";
                Label1.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}