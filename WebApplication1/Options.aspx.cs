using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Options : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                ddlSendWolPackageTo.SelectedValue = common.readSettingDatabase("sendto", 0, 0, 3).ToString();
                txtCountDown.Text = common.readSettingDatabase("countdown", 100, 0, 65536).ToString();

                string email_from = common.readSettingDatabase("email_from", "");
                string email_to = common.readSettingDatabase("email_to", "");
                string email_host = common.readSettingDatabase("email_host", "");
                int email_port = common.readSettingDatabase("email_port", 25, 0, 65536);
                bool email_ssl = common.readSettingDatabase("email_ssl", false);
                bool email_login = common.readSettingDatabase("email_login", true);
                string email_user = common.readSettingDatabase("email_user", "");
                string email_password = common.readSettingDatabase("email_password", "");
                string email_subject = common.readSettingDatabase("email_subject", "");

                txtSender.Text = email_from;
                txtRecipients.Text = email_to;
                txtSmtpServer.Text = email_host;
                txtPort.Text = email_port.ToString();
                chbUseSsl.Checked = email_ssl;
                chbAuthenticationRequired.Checked = email_login;
                txtUsername.Text = email_user;
                txtPassword.Attributes["value"] = email_password;
                txtSubject.Text = email_subject;

                string domainName = WebConfigurationManager.ConnectionStrings["domainName"].ConnectionString;
                ddlAuthentication.Items[0].Text = (domainName ?? "Domain") + " User";
                ddlAuthentication.SelectedValue = common.readSettingDatabase("authentication", 0, 0, 1).ToString();

                bool displayShowAll = common.readSettingDatabase("displayShowAll", false);
                chbDisplayShowAll.Checked = displayShowAll;

                bool verbose = common.readSettingDatabase("verbose", false);
                chbVerbose.Checked = verbose;
            }
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

        protected void btnApplyOptions_Click(object sender, EventArgs e)
        {
            string authentication = ddlAuthentication.SelectedItem.Value;
            common.writeSettingDatabase("authentication", authentication);

            bool displayShowAll = chbDisplayShowAll.Checked;
            common.writeSettingDatabase("displayShowAll", displayShowAll);

            bool verbose = chbVerbose.Checked;
            common.writeSettingDatabase("verbose", verbose);

            string sendWolTo = ddlSendWolPackageTo.SelectedItem.Value;
            common.writeSettingDatabase("sendto", sendWolTo);

            string countdown = txtCountDown.Text;
            common.writeSettingDatabase("countdown", countdown);

            string email_from = txtSender.Text;
            string email_to = txtRecipients.Text;
            string email_host = txtSmtpServer.Text;
            string email_port = txtPort.Text;
            bool email_ssl = chbUseSsl.Checked;
            bool email_login = chbAuthenticationRequired.Checked;
            string email_user = txtUsername.Text;
            string email_password = txtPassword.Text;
            string email_subject = txtSubject.Text;

            common.writeSettingDatabase("email_from", email_from);
            common.writeSettingDatabase("email_to", email_to);
            common.writeSettingDatabase("email_host", email_host);
            common.writeSettingDatabase("email_port", email_port);
            common.writeSettingDatabase("email_ssl", email_ssl);
            common.writeSettingDatabase("email_login", email_login);
            common.writeSettingDatabase("email_user", email_user);
            common.writeSettingDatabase("email_password", email_password);
            common.writeSettingDatabase("email_subject", email_subject);

            string sessionId = this.Session.SessionID;
            Sessions.readSession(sessionId, out Sessions.session ses);
            common.writeLog(ses.username, "Options", "Changed options");
        }
    }
}