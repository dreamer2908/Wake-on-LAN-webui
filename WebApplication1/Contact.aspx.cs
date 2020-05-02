using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Contact : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            Sessions.readSession(sessionId, out Sessions.session ses);
            
            if (ses.isLoggedIn)
            {
                lblUsername.Text = ses.username;
                pLogin.Visible = true;
                pNotLogin.Visible = false;
                spanAdminLink.Visible = ses.isAdmin;
            }
            else
            {
                pLogin.Visible = false;
                pNotLogin.Visible = true;
                lblUsername.Text = "";
            }

            string email_to = common.readSettingDatabase("email_to", "");
            spanItEmail.InnerText = email_to;

            contactTable.Visible = true;
            contactResult.Visible = false;

            // import contents from paramenters
            txtName.Text = Request.QueryString["name"] ?? string.Empty;
            txtEmailAddress.Text = Request.QueryString["email"] ?? string.Empty;
            txtPhoneNumber.Text = Request.QueryString["phone"] ?? string.Empty;
            txtSubject.Text = Request.QueryString["subject"] ?? string.Empty;
            txtMessage.Text = Request.QueryString["message"] ?? string.Empty;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string user = txtName.Text;
            string userEmail = txtEmailAddress.Text;
            string userPhoneNumber = txtPhoneNumber.Text;
            string userSubject = txtSubject.Text;
            string userMessage = txtMessage.Text;

            string subject = string.Format("[WoL Web] Help Desk Request From <{0}>", user);
            string emailHeadline = "*** This is a system generated email, do not reply to this email id ***\n";

            string time = common.getNowString();

            string body = string.Format("{0} \nTimestamp: {1} \nUsername: {7} \nName: {2} \nEmail: {3} \nPhone: {4} \nSubject: {5} \nMessage: \n{6}", emailHeadline, time, user, userEmail, userPhoneNumber, userSubject, userMessage, lblUsername.Text);
            
            common.writeLog(lblUsername.Text, "Contact", "Sent email to " + spanItEmail.InnerText);
            common.readEmailSenderParamenter();
            common.sendEmail(subject, body);

            contactTable.Visible = false;
            contactResult.Visible = true;
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
    }
}