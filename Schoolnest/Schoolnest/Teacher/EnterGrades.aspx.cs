using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class EnterGrades : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        string Username = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                Username = Session["Username"].ToString();
                BindDropdownLists(schoolId, Username);
            }
        }

        private void BindDropdownLists(string schoolId, string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDropdownDataEnterGrades", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolId));
                    cmd.Parameters.Add(new SqlParameter("@username", username));
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

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            schoolId = Session["SchoolId"].ToString();
            Username = Session["Username"].ToString();
            BindGridview(schoolId);
        }

        private void BindGridview(string schoolId)
        {
            int selectedStandard = int.Parse(ddlStandard.SelectedValue);
            int selectedDivision = int.Parse(ddlDivision.SelectedValue);
            int selectedExam = int.Parse(ddlExam.SelectedValue);
            int selectedSubject = int.Parse(ddlSubject.SelectedValue);

            DataTable dtStudents = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query to get students and their existing grades if available
                string query = @"
            SELECT 
                SM.Student_FullName, 
                E.TotalMarks,
                G.Grade 
            FROM 
                StudentMaster SM
                INNER JOIN Exam E ON E.SchoolID = SM.SchoolMaster_SchoolID AND E.ExamID = @ExamID
                LEFT JOIN Grades G ON G.StudentName = SM.Student_FullName 
                                    AND G.ExamMaster_ExamID = @ExamID 
                                    AND G.StandardID = @StandardID 
                                    AND G.DivisionID = @DivisionID 
                                    AND G.SubjectID = @SubjectID
            WHERE 
                SM.SchoolMaster_SchoolID = @SchoolID 
                AND SM.Student_Standard = @StandardID 
                AND SM.Student_Division = @DivisionID
            ORDER BY 
                SM.Student_LastName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                cmd.Parameters.AddWithValue("@ExamID", selectedExam);
                cmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                cmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                cmd.Parameters.AddWithValue("@SubjectID", selectedSubject);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtStudents);
            }
                        
            if (dtStudents.Rows.Count == 0)
            {
                
                gvStudents.DataSource = dtStudents;
                gvStudents.DataBind();
            }
            else
            {
               
                gvStudents.DataSource = dtStudents;
                gvStudents.DataBind();
            }
        }



        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void gvStudents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the TextBox control within the row
                TextBox txtGrade = (TextBox)e.Row.FindControl("txtGrade");

                // Check if the "Grade" field has a value in the data source and set it in the TextBox
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                if (rowView["Grade"] != DBNull.Value)
                {
                    txtGrade.Text = rowView["Grade"].ToString();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            schoolId = Session["SchoolId"].ToString();
            int selectedExam = int.Parse(ddlExam.SelectedValue);
            int selectedStandard = int.Parse(ddlStandard.SelectedValue);
            int selectedDivision = int.Parse(ddlDivision.SelectedValue);
            int selectedSubject = int.Parse(ddlSubject.SelectedValue);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (GridViewRow row in gvStudents.Rows)
                {
                    string studentName = row.Cells[0].Text;
                    TextBox txtGrade = (TextBox)row.FindControl("txtGrade");
                    int grade = int.Parse(txtGrade.Text);
                    int totalMarks = int.Parse(row.Cells[1].Text);

                    // Validate that grade does not exceed total marks
                    if (grade > totalMarks)
                    {
                        Response.Write("<script>alert('Grade for " + studentName + " exceeds total marks (" + totalMarks + "). Please correct it.');</script>");
                        return;
                    }

                    // Check if grade already exists for this student, exam, standard, division, and subject
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Grades WHERE SchoolMaster_SchoolID = @SchoolID AND ExamMaster_ExamID = @ExamID AND StudentName = @StudentName AND StandardID = @StandardID AND DivisionID = @DivisionID AND SubjectID = @SubjectID", conn);
                    checkCmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    checkCmd.Parameters.AddWithValue("@ExamID", selectedExam);
                    checkCmd.Parameters.AddWithValue("@StudentName", studentName);
                    checkCmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                    checkCmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                    checkCmd.Parameters.AddWithValue("@SubjectID", selectedSubject);

                    int recordCount = (int)checkCmd.ExecuteScalar();

                    if (recordCount > 0)
                    {
                        // If the record exists, update the grade
                        SqlCommand updateCmd = new SqlCommand("UPDATE Grades SET Grade = @Grade WHERE SchoolMaster_SchoolID = @SchoolID AND ExamMaster_ExamID = @ExamID AND StudentName = @StudentName AND StandardID = @StandardID AND DivisionID = @DivisionID AND SubjectID = @SubjectID", conn);
                        updateCmd.Parameters.AddWithValue("@Grade", grade);
                        updateCmd.Parameters.AddWithValue("@SchoolID", schoolId);
                        updateCmd.Parameters.AddWithValue("@ExamID", selectedExam);
                        updateCmd.Parameters.AddWithValue("@StudentName", studentName);
                        updateCmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                        updateCmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                        updateCmd.Parameters.AddWithValue("@SubjectID", selectedSubject);

                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // If the record does not exist, insert a new grade
                        SqlCommand insertCmd = new SqlCommand("INSERT INTO Grades (SchoolMaster_SchoolID, ExamMaster_ExamID, StudentName, StandardID, DivisionID, SubjectID, Grade) VALUES (@SchoolID, @ExamID, @StudentName, @StandardID, @DivisionID, @SubjectID, @Grade)", conn);
                        insertCmd.Parameters.AddWithValue("@SchoolID", schoolId);
                        insertCmd.Parameters.AddWithValue("@ExamID", selectedExam);
                        insertCmd.Parameters.AddWithValue("@StudentName", studentName);
                        insertCmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                        insertCmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                        insertCmd.Parameters.AddWithValue("@SubjectID", selectedSubject);
                        insertCmd.Parameters.AddWithValue("@Grade", grade);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            Response.Write("<script>alert('Grades saved or updated successfully');</script>");
        }


        private void ClearForm()
        {
            ddlStandard.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
            ddlSubject.SelectedIndex = 0;
            ddlExam.SelectedIndex = 0;
            gvStudents.Visible = false;
        }
    }
}