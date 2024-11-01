using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class AssignSubjectToClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load data into dropdowns ddlClass and ddlSubject here
            }
        }
        protected void ddlSearchAssigned_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Code to handle the event when a different item is selected in ddlSearchAssigned
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Code to handle the submission of assigned subject to class
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Code to handle form cancellation
        }
    }
}