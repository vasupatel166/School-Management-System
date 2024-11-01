using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class AssignTeacherToSubject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load data into ddlTeacher and ddlSubject dropdowns here
            }
        }

        protected void ddlSearchAssignedTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Code to handle when a different item is selected in ddlSearchAssignedTeacher
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Code to handle form submission for assigning a teacher to a subject
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Code to handle form cancellation
        }
    }
}