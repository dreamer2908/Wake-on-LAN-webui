using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = this.Session.SessionID;
            // use the session cookie if available
            if (Request.Cookies["session"] != null)
            {
                sessionId = Request.Cookies["session"].Value;
            }
            Sessions.readSession(sessionId, out Sessions.session ses);
            if (ses.isLoggedIn)
            {
                common.writeLog(ses.username, "Logout", "Logout OK");
            }

            Sessions.deleteSession(ses);
            this.Session.Abandon();

            // delete the session cookie
            Response.Cookies["session"].Value = string.Empty;
            Response.Cookies["session"].Expires = DateTime.Now.AddDays(-1);

            Response.Redirect("Login.aspx");
        }
    }
}