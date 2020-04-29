using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

                string email_from = common.readSettingDatabase("email_from", "");
                string email_to = common.readSettingDatabase("email_to", "");
                string email_host = common.readSettingDatabase("email_host", "");
                int email_port = common.readSettingDatabase("email_port", 25, 0, 65536);
                bool email_ssl = common.readSettingDatabase("email_ssl", false);
                bool email_login = common.readSettingDatabase("email_login", true);
                string email_user = common.readSettingDatabase("email_user", "");
                string email_password = common.readSettingDatabase("email_password", "");

                txtSender.Text = email_from;
                txtReceivers.Text = email_to;
                txtSmtpServer.Text = email_host;
                txtPort.Text = email_port.ToString();
                chbUseSsl.Checked = email_ssl;
                chbAuthenticationRequired.Checked = email_login;
                txtUsername.Text = email_user;
                txtPassword.Attributes["value"] = email_password;
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
            string sendWolTo = ddlSendWolPackageTo.SelectedItem.Value;
            common.writeSettingDatabase("sendto", sendWolTo);

            string email_from = txtSender.Text;
            string email_to = txtReceivers.Text;
            string email_host = txtSmtpServer.Text;
            string email_port = txtPort.Text;
            bool email_ssl = chbUseSsl.Checked;
            bool email_login = chbAuthenticationRequired.Checked;
            string email_user = txtUsername.Text;
            string email_password = txtPassword.Text;

            common.writeSettingDatabase("email_from", email_from);
            common.writeSettingDatabase("email_to", email_to);
            common.writeSettingDatabase("email_host", email_host);
            common.writeSettingDatabase("email_port", email_port);
            common.writeSettingDatabase("email_ssl", email_ssl);
            common.writeSettingDatabase("email_login", email_login);
            common.writeSettingDatabase("email_user", email_user);
            common.writeSettingDatabase("email_password", email_password);
        }
    }
}