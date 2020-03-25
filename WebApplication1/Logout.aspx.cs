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
            string id = this.Session.SessionID;
            Sessions.readSession(id, out Sessions.session ses);
            if (ses.isLoggedIn)
            {
                common.writeLog(ses.username, "Logout", "Logout OK");
            }

            Sessions.deleteSession(ses);
            this.Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}