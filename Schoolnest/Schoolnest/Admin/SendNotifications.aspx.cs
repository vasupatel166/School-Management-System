using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Schoolnest.Admin
{
    public partial class SendEmailNotifications : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadTeachers();
            }
        }

        protected void ddlUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUserType.SelectedValue == "Student")
            {
                pnlStudentFilters.Visible = true;
                LoadAssignedStandard();
                gvUsers.DataSource = null;
                gvUsers.DataBind();
            }
            else
            {
                pnlStudentFilters.Visible = false;
                LoadTeachers();
            }
        }

        private void LoadTeachers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TeacherID, TeacherName AS Name, Teacher_Email AS Email,
                    CASE 
                        WHEN ProfileImage IS NOT NULL THEN '~/assets/img/user-profile-img/' + ProfileImage 
                        ELSE '~/assets/img/user-profile-img/default_user_logo.png' 
                    END AS ProfileImage
                    FROM TeacherMaster 
                    WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    gvUsers.DataSource = reader;
                    gvUsers.DataBind();
                }
            }
        }

        private void LoadAssignedStandard()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Standards_StandardID, s.StandardName 
                    FROM SubjectDetail AS sd 
                    INNER JOIN Standards AS s ON sd.Standards_StandardID = s.StandardID
                    WHERE sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStandard.Items.Clear();
                    ddlStandard.Items.Add(new ListItem("Select Standard", ""));

                    while (reader.Read())
                    {
                        ddlStandard.Items.Add(new ListItem(
                            reader["StandardName"].ToString(),
                            reader["Standards_StandardID"].ToString()));
                    }
                }
            }
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStandard.SelectedValue != "")
            {
                ddlDivision.Enabled = true;
                LoadAssignedDivisions();
            }
            else
            {
                ddlDivision.Enabled = false;
                ddlDivision.Items.Clear();
                ddlDivision.Items.Add(new ListItem("Select Division", ""));
            }
        }

        private void LoadAssignedDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Divisions_DivisionID, d.DivisionName 
                    FROM SubjectDetail AS sd 
                    INNER JOIN Divisions AS d ON sd.Divisions_DivisionID = d.DivisionID
                    WHERE sd.Standards_StandardID = @StandardID 
                    AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlDivision.Items.Clear();
                    ddlDivision.Items.Add(new ListItem("Select Division", ""));

                    while (reader.Read())
                    {
                        ddlDivision.Items.Add(new ListItem(
                            reader["DivisionName"].ToString(),
                            reader["Divisions_DivisionID"].ToString()));
                    }
                }
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDivision.SelectedValue != "")
            {
                LoadStudents();
            }
            else
            {
                gvUsers.DataSource = null;
                gvUsers.DataBind();
            }
        }

        private void LoadStudents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT StudentID, Student_FullName AS Name, Student_EmailID AS Email,
                    CASE 
                        WHEN Student_ProfileImage IS NOT NULL THEN '~/assets/img/user-profile-img/' + Student_ProfileImage 
                        ELSE '~/assets/img/user-profile-img/default_user_logo.png' 
                    END AS ProfileImage
                    FROM StudentMaster 
                    WHERE Student_Standard = @StandardID 
                    AND Student_Division = @DivisionID 
                    AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    gvUsers.DataSource = reader;
                    gvUsers.DataBind();
                }
            }
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSelectAll = (CheckBox)sender;
            foreach (GridViewRow row in gvUsers.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                chkSelect.Checked = chkSelectAll.Checked;
            }
        }

        protected void btnSendNotification_Click(object sender, EventArgs e)
        {
            List<string> selectedEmails = new List<string>();

            // Check if any checkbox is selected
            bool isAnyCheckboxSelected = false;
            foreach (GridViewRow row in gvUsers.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    isAnyCheckboxSelected = true;
                    string email = gvUsers.DataKeys[row.RowIndex].Value.ToString();
                    selectedEmails.Add(email);
                }
            }

            // If no checkbox is selected, display an error message
            if (!isAnyCheckboxSelected)
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "showalert", "alert('Please select at least one recipient.');", true);
                return;
            }

            // Validate student selection if student type is selected
            if (ddlUserType.SelectedValue == "Student")
            {
                if (string.IsNullOrEmpty(ddlStandard.SelectedValue) ||
                    string.IsNullOrEmpty(ddlDivision.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "showalert", "alert('Please select both Standard and Division.');", true);
                    return;
                }
            }

            // Create Gmail draft with selected recipients
            string recipients = string.Join(",", selectedEmails);
            string subject = Uri.EscapeDataString("School Notification");
            string body = Uri.EscapeDataString("Your message body here.");
            string gmailUrl = $"https://mail.google.com/mail/?view=cm&fs=1&to={recipients}&su={subject}&body={body}";
            Response.Redirect(gmailUrl);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/SendNotifications.aspx");
        }
    }
}