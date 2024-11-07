using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class AssignTeacherToSubject : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadStandardsWithDivisions();
            }
        }

        private void LoadStandardsWithDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT s.StandardID AS ID, s.StandardName AS Name, s.SchoolMaster_SchoolID, 'Standard' AS Type 
                    FROM Standards s 
                    WHERE s.SchoolMaster_SchoolID = @SchoolID 
                    UNION ALL
                    SELECT d.DivisionID AS ID, d.DivisionName AS Name, d.SchoolMaster_SchoolID, 'Division' AS Type 
                    FROM Divisions d 
                    WHERE d.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStandard.Items.Clear();
                    ddlDivision.Items.Clear();

                    ddlStandard.Items.Add(new ListItem("Select Standard", ""));
                    ddlDivision.Items.Add(new ListItem("Select Division", ""));

                    while (reader.Read())
                    {
                        string id = reader["ID"].ToString();
                        string name = reader["Name"].ToString();
                        string type = reader["Type"].ToString();

                        if (type == "Standard")
                            ddlStandard.Items.Add(new ListItem(name, id));
                        else if (type == "Division")
                            ddlDivision.Items.Add(new ListItem(name, id));
                    }
                }
            }
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlStandard.SelectedValue) &&
                !string.IsNullOrEmpty(ddlDivision.SelectedValue))
            {
                LoadSubjectsGrid();
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlStandard.SelectedValue) &&
                !string.IsNullOrEmpty(ddlDivision.SelectedValue))
            {
                LoadSubjectsGrid();
            }
        }

        private void LoadSubjectsGrid()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT sd.SubjectDetailID, sm.SubjectName, sd.Teachers_TeacherID FROM SubjectDetail sd 
                    INNER JOIN SubjectMaster sm ON sd.SubjectMaster_SubjectID = sm.SubjectID
                    WHERE sd.Standards_StandardID = @StandardID AND sd.Divisions_DivisionID = @DivisionID AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        gvSubjects.DataSource = dt;
                        gvSubjects.DataBind();

                        // Get list of already assigned teachers
                        List<string> assignedTeachers = new List<string>();
                        foreach (DataRow row in dt.Rows)
                        {
                            string teacherId = row["Teachers_TeacherID"].ToString();
                            if (!string.IsNullOrEmpty(teacherId))
                            {
                                assignedTeachers.Add(teacherId);
                            }
                        }

                        // Load teachers for each row
                        foreach (GridViewRow row in gvSubjects.Rows)
                        {
                            DropDownList ddlTeacher = (DropDownList)row.FindControl("ddlTeacher");
                            string currentTeacherId = dt.Rows[row.RowIndex]["Teachers_TeacherID"].ToString();
                            LoadTeacherDropdown(ddlTeacher, assignedTeachers, currentTeacherId);

                            // Set selected teacher if exists
                            if (!string.IsNullOrEmpty(currentTeacherId))
                            {
                                ddlTeacher.SelectedValue = currentTeacherId;
                            }
                        }

                        btnSubmit.Enabled = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "HideError", "$('.alert-danger').hide();", true);
                    }
                    else
                    {
                        gvSubjects.DataSource = null;
                        gvSubjects.DataBind();
                        btnSubmit.Enabled = false;

                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                            "Swal.fire({" +
                            "  icon: 'warning'," +
                            "  title: 'No Subjects Found'," +
                            "  text: 'No subjects are available for the selected Standard and Division.'," +
                            "  confirmButtonColor: '#3085d6'," +
                            "  confirmButtonText: 'OK'" +
                            "});", true);
                    }
                }
            }
        }

        protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTeacherDropdowns();
        }

        private void UpdateTeacherDropdowns()
        {
            List<string> selectedTeachers = new List<string>();
            foreach (GridViewRow gvRow in gvSubjects.Rows)
            {
                DropDownList ddl = (DropDownList)gvRow.FindControl("ddlTeacher");
                if (ddl.SelectedValue != "0" && !selectedTeachers.Contains(ddl.SelectedValue))
                {
                    selectedTeachers.Add(ddl.SelectedValue);
                }
            }

            foreach (GridViewRow gvRow in gvSubjects.Rows)
            {
                DropDownList ddl = (DropDownList)gvRow.FindControl("ddlTeacher");
                string currentTeacherId = ddl.SelectedValue;
                LoadTeacherDropdown(ddl, selectedTeachers, currentTeacherId);
            }
        }

        private void LoadTeacherDropdown(DropDownList ddlTeacher, List<string> selectedTeachers, string currentTeacherId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT TeacherID, TeacherName FROM TeacherMaster WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    ddlTeacher.Items.Clear();
                    ddlTeacher.Items.Add(new ListItem("Select Teacher", "0"));

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string teacherId = reader["TeacherID"].ToString();
                        string teacherName = reader["TeacherName"].ToString();

                        ListItem item = new ListItem(teacherName, teacherId);

                        if (selectedTeachers.Contains(teacherId) && teacherId != currentTeacherId)
                        {
                            // Option 1: Disable the item (visible but non-selectable)
                            item.Enabled = false;

                            // Option 2: Exclude the item (comment this line if you prefer to disable instead of hide)
                            // continue;
                        }

                        ddlTeacher.Items.Add(item);
                    }
                }
            }

            // Restore the current selection if applicable
            if (!string.IsNullOrEmpty(currentTeacherId))
            {
                ddlTeacher.SelectedValue = currentTeacherId;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"UPDATE SubjectDetail SET Teachers_TeacherID = @TeacherID WHERE SubjectDetailID = @SubjectDetailID", conn))
                    {
                        foreach (GridViewRow row in gvSubjects.Rows)
                        {
                            DropDownList ddlTeacher = (DropDownList)row.FindControl("ddlTeacher");
                            HiddenField hdnSubjectDetailId = (HiddenField)row.FindControl("hdnSubjectDetailId");

                            cmd.Parameters.Clear();

                            // Check if the selected value is the first option (no value) and set @TeacherID accordingly
                            if (ddlTeacher.SelectedValue == "0" || string.IsNullOrEmpty(ddlTeacher.SelectedValue))
                            {
                                cmd.Parameters.AddWithValue("@TeacherID", DBNull.Value); 
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@TeacherID", ddlTeacher.SelectedValue);
                            }

                            cmd.Parameters.AddWithValue("@SubjectDetailID", hdnSubjectDetailId.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('Teachers assigned successfully!');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AssignTeacherToSubject.aspx");
        }
    }
}