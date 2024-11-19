using System;

namespace Schoolnest
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] != null)
                {

                    string roleId = Session["UserRole"]?.ToString();

                    Session.Abandon();
                    Session.Clear();

                    if (roleId == "S")
                    {
                        Response.Redirect("~/Student/StudentLogin.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                }
            }
        }
    }
}