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
        string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            
            if(!IsPostBack)
            {
                BindDropdowns();
                BindExamScheduleGrid();
            }
        }

        private void BindExamScheduleGrid()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ES.ExamScheduleID AS ExamScheduleID,E.ExamName AS ExamName,ES.ExamDate AS ExamDate,SM.SubjectName AS [Subject],SD.StandardName AS [Standard]," +
                " DV.DivisionName AS [Division],SC.SectionName AS [Section] " +
                " from examschedule ES " +
                " Left Join Exam E ON E.ExamID=ES.ExamID " +
                " Left Join Standards SD ON SD.StandardID=ES.StandardID " +
                " Left Join Divisions DV ON DV.DivisionID=ES.DivisionID " +
                " Left Join SubjectMaster SM ON SM.SubjectID=ES.SubjectID " +
                " Left Join Sections SC ON SC.SectionID=ES.SectionID " +
                " Where ES.SchoolID=@SchoolID ", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();
                    gvExamSchedule.DataSource = cmd.ExecuteReader();
                    gvExamSchedule.DataBind();
                }
            }
        }

        private void BindDropdowns()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDropdownDataExamSchedule", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", SchoolID));
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtExamScheduleID.Text = string.Empty;
            ddlSubject.SelectedIndex = 0;
            ddlStandard.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
            ddlSection.SelectedIndex = 0;
            ddlExam.SelectedIndex = 0;
            txtDateOfExam.Text = string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            saveSchedule();
            gvExamSchedule.EditIndex = -1;
            BindExamScheduleGrid();
        }

        private void saveSchedule()
        {

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
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", SchoolID));

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

        protected void gvExamSchedule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int examScheduleID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditExamSchedule")
            {
                // Populate the form for editing
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM ExamSchedule WHERE ExamScheduleID = @ExamScheduleID AND SchoolID = @SchoolID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ExamScheduleID", examScheduleID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            txtExamScheduleID.Text = reader["ExamScheduleID"].ToString();
                            ddlExam.SelectedValue = reader["ExamID"].ToString();
                            ddlSubject.SelectedValue = reader["SubjectID"].ToString();
                            ddlStandard.SelectedValue = reader["StandardID"].ToString();
                            ddlDivision.SelectedValue = reader["DivisionID"].ToString();
                            ddlSection.SelectedValue = reader["SectionID"].ToString();
                            txtDateOfExam.Text = Convert.ToDateTime(reader["ExamDate"]).ToString("yyyy-MM-dd");
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteExamSchedule")
            {
                // Delete the record
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM ExamSchedule WHERE ExamScheduleID = @ExamScheduleID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ExamScheduleID", examScheduleID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Refresh the grid
                BindExamScheduleGrid();
            }
        }

        protected void gvExamSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton deleteButton = e.Row.FindControl("lnkDelete") as LinkButton;
                if (deleteButton != null)
                {
                    deleteButton.OnClientClick = "return confirm('Are you sure you want to delete this exam schedule?');";
                }
            }
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
                ddl.SelectedIndex = 0;
            }
        }        

    }
}