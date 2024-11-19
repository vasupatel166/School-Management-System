using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class SendNotifications : Page
    {

        private string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string Username;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            Username = Session["Username"]?.ToString();

            if (!IsPostBack)
            {
                LoadAssignedStandard();
            }
        }

        private void LoadAssignedStandard()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Standards_StandardID, s.StandardName FROM SubjectDetail AS sd 
                    INNER JOIN Standards AS s ON sd.Standards_StandardID = s.StandardID
                    WHERE sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStandard.Items.Clear();
                    ddlStandard.Items.Add(new ListItem("Select Standard", ""));

                    while (reader.Read())
                    {
                        ddlStandard.Items.Add(new ListItem(reader["StandardName"].ToString(),reader["Standards_StandardID"].ToString()));
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
                ddlDivision.SelectedIndex = 0;
            }
        }

        private void LoadAssignedDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Divisions_DivisionID, d.DivisionName FROM SubjectDetail AS sd 
                    INNER JOIN Divisions AS d ON sd.Divisions_DivisionID = d.DivisionID
                    WHERE sd.Standards_StandardID = @StandardID AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlDivision.Items.Clear();
                    ddlDivision.Items.Add(new ListItem("Select Division", ""));

                    while (reader.Read())
                    {
                        ddlDivision.Items.Add(new ListItem(reader["DivisionName"].ToString(),reader["Divisions_DivisionID"].ToString()));

                    }
                }
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDivision.SelectedValue != "0")
            {
                LoadStudents();
            }
            else
            {
                gvStudents.DataSource = null;
                gvStudents.DataBind();
            }
        }

        private void LoadStudents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT StudentID, Student_FullName AS StudentName, Student_EmailID AS StudentEmail, 
                CASE 
                    WHEN Student_ProfileImage IS NOT NULL THEN '~/assets/img/user-profile-img/' + Student_ProfileImage 
                    ELSE '~/assets/img/user-profile-img/default_user_logo.png' 
                END AS ProfileImage
                FROM StudentMaster WHERE Student_Standard = @StandardID AND Student_Division = @DivisionID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    gvStudents.DataSource = reader;
                    gvStudents.DataBind();
                }
            }
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSelectAll = (CheckBox)sender;
            foreach (GridViewRow row in gvStudents.Rows)
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
            foreach (GridViewRow row in gvStudents.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    isAnyCheckboxSelected = true;
                    string email = gvStudents.DataKeys[row.RowIndex].Value.ToString();
                    selectedEmails.Add(email);
                }
            }

            // If no checkbox is selected, display an error message
            if (!isAnyCheckboxSelected)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showalert", "alert('Please select at least one student.');", true);
                return;
            }

            // If checkboxes are selected, proceed with email redirection
            string recipients = string.Join(",", selectedEmails);
            string subject = Uri.EscapeDataString("Your Subject Here");
            string body = Uri.EscapeDataString("Your message body here.");

            // Create Gmail draft URL
            string gmailUrl = $"https://mail.google.com/mail/?view=cm&fs=1&to={recipients}&su={subject}&body={body}";

            // Redirect to Gmail
            Response.Redirect(gmailUrl);
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Teacher/SendNotifications.aspx");
        }
    }
}
