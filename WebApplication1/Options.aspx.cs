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
                ddlSendWolPackageTo.SelectedValue = common.readSettingDatabase_sendTo();
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
            common.writeSettingDatabase_sendTo(sendWolTo);
        }
    }
}