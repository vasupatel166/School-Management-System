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
                    ddlStandard.Items.Add(new ListItem("Select Standard", "0"));

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
            if (ddlStandard.SelectedValue != "0")
            {
                LoadAssignedDivisions();
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.SelectedIndex = 0;
                ddlDivision.Enabled = false;
            }
            
            ClearSubjectsGrid();
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
                    ddlDivision.Items.Add(new ListItem("Select Division", "0"));

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
            if (ddlDivision.SelectedValue != "0")
            {
                LoadSubjectsGrid();
            }
            else
            {
                ClearSubjectsGrid();
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

                        // Load teachers for each row
                        foreach (GridViewRow row in gvSubjects.Rows)
                        {
                            DropDownList ddlTeacher = (DropDownList)row.FindControl("ddlTeacher");
                            string currentTeacherId = dt.Rows[row.RowIndex]["Teachers_TeacherID"].ToString();
                            LoadTeacherDropdown(ddlTeacher, currentTeacherId);

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

        //protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    UpdateTeacherDropdowns();
        //}

        //private void UpdateTeacherDropdowns()
        //{
        //    List<string> selectedTeachers = new List<string>();
        //    foreach (GridViewRow gvRow in gvSubjects.Rows)
        //    {
        //        DropDownList ddl = (DropDownList)gvRow.FindControl("ddlTeacher");
        //        if (ddl.SelectedValue != "0" && !selectedTeachers.Contains(ddl.SelectedValue))
        //        {
        //            selectedTeachers.Add(ddl.SelectedValue);
        //        }
        //    }

        //    foreach (GridViewRow gvRow in gvSubjects.Rows)
        //    {
        //        DropDownList ddl = (DropDownList)gvRow.FindControl("ddlTeacher");
        //        string currentTeacherId = ddl.SelectedValue;
        //        LoadTeacherDropdown(ddl, selectedTeachers, currentTeacherId);
        //    }
        //}

        private void LoadTeacherDropdown(DropDownList ddlTeacher, string currentTeacherId)
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
                        ddlTeacher.Items.Add(new ListItem(teacherName, teacherId));
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

        private void ClearSubjectsGrid()
        {
            gvSubjects.DataSource = null;
            gvSubjects.DataBind();
        }
    }
}