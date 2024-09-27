using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the session exists before abandoning it
                if (Session["UserRole"] != null)
                {
                    Session.Abandon();

                    Session.Clear();

                    // Redirect to the login page
                    Response.Redirect("~/Login.aspx");
                }
            }
        }
    }
}