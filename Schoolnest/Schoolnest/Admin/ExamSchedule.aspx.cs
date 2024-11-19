using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class ExamSchedule : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var context = HttpContext.Current;
                string schoolID = context.Session["SchoolID"]?.ToString();
                BindDropdowns(schoolID);
                BindExamScheduleGrid(schoolID);
            }
        }

        private void BindExamScheduleGrid(string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("select ES.ExamScheduleID AS ExamScheduleID,E.ExamName AS ExamName,ES.ExamDate AS ExamDate,SM.SubjectName AS [Subject],SD.StandardName AS [Standard]," +
       " DV.DivisionName AS [Division],SC.SectionName AS [Section] " +
       " from examschedule ES " +
       " Left Join Exam E ON E.ExamID=ES.ExamID " +
       " Left Join Standards SD ON SD.StandardID=ES.StandardID " +
       " Left Join Divisions DV ON DV.DivisionID=ES.DivisionID " +
       " Left Join SubjectMaster SM ON SM.SubjectID=ES.SubjectID " +
       " Left Join Sections SC ON SC.SectionID=ES.SectionID " +
       " Where ES.SchoolID=@SchoolID ", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    conn.Open();
                    gvExamSchedule.DataSource = cmd.ExecuteReader();
                    gvExamSchedule.DataBind();
                }
            }
        }

        private void BindDropdowns(string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDropdownDataExamSchedule", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolID));
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Bind Subject dropdown
                        if (reader.HasRows)
                        {
                            ddlSubject.DataSource = reader;
                            ddlSubject.DataTextField = "SubjectName";
                            ddlSubject.DataValueField = "SubjectID";
                            ddlSubject.DataBind();
                            ddlSubject.Items.Insert(0, new ListItem("--Select Subject--", "0"));
                        }

                        // Move to next result set (Sections)
                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlSection.DataSource = reader;
                            ddlSection.DataTextField = "SectionName";
                            ddlSection.DataValueField = "SectionID";
                            ddlSection.DataBind();
                            ddlSection.Items.Insert(0, new ListItem("--Select Section--", "0"));
                        }

                        // Move to next result set (Standards)
                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlStandard.DataSource = reader;
                            ddlStandard.DataTextField = "StandardName";
                            ddlStandard.DataValueField = "StandardID";
                            ddlStandard.DataBind();
                            ddlStandard.Items.Insert(0, new ListItem("--Select Standard--", "0"));
                        }

                        // Move to next result set (Divisions)
                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlDivision.DataSource = reader;
                            ddlDivision.DataTextField = "DivisionName";
                            ddlDivision.DataValueField = "DivisionID";
                            ddlDivision.DataBind();
                            ddlDivision.Items.Insert(0, new ListItem("--Select Division--", "0"));
                        }

                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlExam.DataSource = reader;
                            ddlExam.DataTextField = "ExamName";
                            ddlExam.DataValueField = "ExamID";
                            ddlExam.DataBind();
                            ddlExam.Items.Insert(0, new ListItem("--Select Exam--", "0"));
                        }
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ddlSubject.SelectedIndex = 0;
            ddlStandard.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
            ddlSection.SelectedIndex = 0;
            ddlExam.SelectedIndex = 0;
            txtDateOfExam.Text = string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            saveSchedule();
            gvExamSchedule.EditIndex = -1;
            BindExamScheduleGrid(schoolID);
        }

        private void saveSchedule()
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateExamSchedule", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ExamScheduleID", txtExamScheduleID.Text));
                    cmd.Parameters.Add(new SqlParameter("@SubjectID", ddlSubject.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@StandardID", ddlStandard.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", ddlDivision.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@SectionID", ddlSection.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@ExamID", ddlExam.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@ExamDate", txtDateOfExam.Text));
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolID));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (string.IsNullOrEmpty(txtExamScheduleID.Text))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Exam Schedule Created Successfully');", true);
                            ClearForm();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Exam Schedule Updated Successfully');", true);
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                    }
                }
            }
        }

        protected void gvExamSchedule_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Get the ExamScheduleID from the GridView and populate the form with this record's data
            GridViewRow row = gvExamSchedule.Rows[e.NewEditIndex];
            string ExamScheduleID = gvExamSchedule.DataKeys[e.NewEditIndex].Value.ToString();

            // Ensure the dropdowns are populated
            var schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();
            BindDropdowns(schoolID);

            // Assign values to the form fields
            txtExamScheduleID.Text = ExamScheduleID;

            // Set the selected values if they exist in the dropdown list
            SetDropdownSelectedValue(ddlExam, row.Cells[1].Text);       // Exam Name
            SetDropdownSelectedValue(ddlSubject, row.Cells[3].Text);    // Subject
            SetDropdownSelectedValue(ddlStandard, row.Cells[4].Text);   // Standard
            SetDropdownSelectedValue(ddlDivision, row.Cells[5].Text);   // Division
            SetDropdownSelectedValue(ddlSection, row.Cells[6].Text);    // Section
        }

        private void SetDropdownSelectedValue(DropDownList ddl, string value)
        {
            ListItem item = ddl.Items.FindByText(value);
            if (item != null)
            {
                ddl.SelectedValue = item.Value;
            }
            else
            {
                ddl.SelectedIndex = 0; // Optionally, set to a default if the value is missing
            }
        }

        protected void gvExamSchedule_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            // Get the AssignmentID of the row being deleted
            int ExamScheduleID = Convert.ToInt32(gvExamSchedule.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM examschedule WHERE ExamScheduleID = @ExamScheduleID", conn))
                {
                    cmd.Parameters.AddWithValue("@ExamScheduleID", ExamScheduleID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Rebind the GridView to refresh the data
            BindExamScheduleGrid(schoolID);
        }


        protected void gvExamSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the delete button in the row
                Button deleteButton = e.Row.Cells[5].Controls.OfType<Button>().FirstOrDefault(btn => btn.CommandName == "Delete");

                if (deleteButton != null)
                {
                    // Add a JavaScript confirmation dialog to the delete button
                    deleteButton.OnClientClick = "return confirm('Are you sure you want to delete this assignment?');";
                }
            }
        }
    }
}