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
            // use the session cookie if available
            if (Request.Cookies["session"] != null)
            {
                sessionId = Request.Cookies["session"].Value;
            }

            // back to main if already logged in
            Sessions.readSession(sessionId, out Sessions.session ses);
            if (ses.isLoggedIn)
            {
                Response.Redirect("Default.aspx");
            }

            label2.Text = sessionId;

            if (!IsPostBack)
            {
                // get domain display name from web.config
                string domainName = WebConfigurationManager.ConnectionStrings["domainName"].ConnectionString;
                ddlAuthentication.Items[0].Text = (domainName ?? "Domain") + " User";
                ddlAuthentication.SelectedValue = common.readSettingDatabase("authentication", 0, 0, 1).ToString();
            }
        }

        private static bool checkWebsiteLogin(string username, string _password, ref bool isAdmin)
        {
            bool isValid = false;
            isAdmin = false;

            string password = common.getSha1HashFromText(_password);

            SqlCommand cmd = new SqlCommand("SELECT [username], [password], [admin] FROM [dbo].[Users] WHERE [username] = @username COLLATE SQL_Latin1_General_CP1_CI_AS AND [password] = @password COLLATE SQL_Latin1_General_CP1_CI_AS;");
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

        private static bool checkDomainLogin(string username, string password, ref bool isAdmin, ref string error)
        {
            // create a "principal context" - e.g. your domain (could be machine, too)
            string domainServer = WebConfigurationManager.ConnectionStrings["domainServer"].ConnectionString;
            string domainUsername = WebConfigurationManager.ConnectionStrings["domainUsername"].ConnectionString;
            string domainPassword = WebConfigurationManager.ConnectionStrings["domainPassword"].ConnectionString;

            bool isValid = checkDomainLoginSub(username, password, ref isAdmin, ref error, domainServer, domainUsername, domainPassword);

            return isValid;
        }

        private static bool checkDomainLoginSub(string username, string password, ref bool isAdmin, ref string error, string domainServer, string domainUsername, string domainPassword)
        {
            bool isValid;
            try
            {
                // must have a valid username and password for the principal context
                // otherwise can't verify user's group membership
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainServer, domainUsername, domainPassword))
                {
                    // validate the credentials
                    isValid = context.ValidateCredentials(username, password, ContextOptions.Negotiate);

                    if (isValid)
                    {
                        // set as admin if the user is an domain administrator
                        string admin1 = "Enterprise Admins";
                        string admin2 = "Domain Admins";
                        string admin3 = "Administrators";
                        bool isAdmin1 = checkIfUserInGroup(username, context, admin1);
                        bool isAdmin2 = checkIfUserInGroup(username, context, admin2);
                        bool isAdmin3 = checkIfUserInGroup(username, context, admin3);
                        isAdmin = isAdmin1 || isAdmin2 || isAdmin3;
                        common.writeLog("System", "Authentication", "Checking domain user: " + username + ". isAdmin = " + isAdmin.ToString() + ". Group1 '" + admin1 + "' = " + isAdmin1.ToString() + ". Group2 '" + admin2 + "' = " + isAdmin2.ToString() + ". Group3 '" + admin3 + "' = " + isAdmin3.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                error = e.ToString();
                isValid = false;
                isAdmin = false;
            }

            return isValid;
        }

        private static bool checkIfUserInGroup(string username, PrincipalContext context, string groupName)
        {
            UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
            if (user == null) return false; // it will be null if the user is from another connected domain, not the main domain on the domain server

            using (PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups())
            {
                return groups.OfType<GroupPrincipal>().Any(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool isValid = false;
            bool isAdmin = false;
            string error = string.Empty;

            string username = TextBox1.Text;
            string password = TextBox2.Text;

            switch (ddlAuthentication.SelectedItem.Value)
            {
                case "0": isValid = checkDomainLogin(username, password, ref isAdmin, ref error); break;
                case "1": isValid = checkWebsiteLogin(username, password, ref isAdmin); break;
            }

            if (isValid)
            {
                string sessionId = this.Session.SessionID;

                Sessions.writeSession(sessionId, true, username, isAdmin);
                common.writeLog(username, "Login", "Login OK. Authentication = " + ddlAuthentication.SelectedItem.Text + ". isAdmin = " + isAdmin.ToString());
                //add a session Cookie
                Response.Cookies["session"].Value = sessionId;
                Response.Cookies["session"].Expires = DateTime.Now.AddDays(10);
                Response.Redirect("Default.aspx");
            }
            else
            {
                common.writeLog(string.Empty, "Login", string.Format("Login failure: {0}. Authentication: {1}. Error: {2}", username, ddlAuthentication.SelectedItem.Text, error));
                Label1.Text = "Your username and password is incorrect";
                Label1.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void lnkToContact_Click(object sender, EventArgs e)
        {
            Response.Redirect("Contact.aspx");
        }
    }
}