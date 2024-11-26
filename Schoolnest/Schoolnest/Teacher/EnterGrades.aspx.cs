using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web.UI;

namespace Schoolnest.Teacher
{
    public partial class EnterGrades : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        string TeacherID = string.Empty;
        private int UserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            schoolId = Session["SchoolId"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetTeacherID();

            if (!IsPostBack)
            {
                GetStandards();
            }
        }

        private void GetTeacherID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TeacherID FROM TeacherMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TeacherID = reader["TeacherID"]?.ToString();
                        }
                    }
                }
            }
        }

        private void GetStandards()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("" +
                    "Select DISTINCT sd.Standards_StandardID, s.StandardName from SubjectDetail as sd " +
                    "Inner Join Standards as s ON s.StandardID = sd.Standards_StandardID " +
                    "where sd.Teachers_TeacherID = @TeacherID AND sd.SchoolMaster_SchoolID = @SchoolID;", conn))
                {

                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolId));
                    cmd.Parameters.Add(new SqlParameter("@TeacherID", TeacherID));
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            ddlStandard.DataSource = reader;
                            ddlStandard.DataTextField = "StandardName";
                            ddlStandard.DataValueField = "Standards_StandardID";
                            ddlStandard.DataBind();
                            ddlStandard.Items.Insert(0, new ListItem("--Select Standard--", "0"));
                        }
                        else
                        {
                            ddlDivision.Enabled = false;
                        }
                    }
                }
            }
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.Parse(ddlStandard.SelectedValue) != 0)
            {
                ddlDivision.Enabled = true;
                GetDivisions();
                ddlDivision.SelectedIndex = 0;
                ddlSubject.Enabled = false;
                ddlExam.Enabled = false;
                gvStudents.Visible = false;
            }
            else
            {
               ClearForm();
            }
        }

        private void GetDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("" +
                    "Select DISTINCT sd.Divisions_DivisionID, d.DivisionName from SubjectDetail as sd " +
                    "Inner Join Divisions as d ON d.DivisionID = sd.Divisions_DivisionID " +
                    "where sd.Standards_StandardID = @StandardID AND sd.Teachers_TeacherID = @TeacherID AND sd.SchoolMaster_SchoolID = @SchoolID;", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@StandardID", int.Parse(ddlStandard.SelectedValue)));
                    cmd.Parameters.Add(new SqlParameter("@TeacherID", TeacherID));
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolId));
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            ddlDivision.DataSource = reader;
                            ddlDivision.DataTextField = "DivisionName";
                            ddlDivision.DataValueField = "Divisions_DivisionID";
                            ddlDivision.DataBind();
                            ddlDivision.Items.Insert(0, new ListItem("--Select Division--", "0"));
                        }
                        else
                        {
                            ddlSubject.Enabled = false;
                        }
                    }
                }
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.Parse(ddlDivision.SelectedValue) != 0)
            {
                ddlSubject.Enabled = true;
                GetSubjects();
                ddlSubject.SelectedIndex = 0;
                ddlExam.Enabled = false;
                gvStudents.Visible = false;
            }
            else
            {
                ddlSubject.Enabled = false;
                ddlExam.Enabled = false;
                gvStudents.Visible = false;
            }
        }

        private void GetSubjects()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("" +
                    "Select sd.SubjectMaster_SubjectID, s.SubjectName from SubjectDetail as sd " +
                    "Inner Join SubjectMaster as s ON s.SubjectID = sd.SubjectMaster_SubjectID " +
                    "where sd.Standards_StandardID = @StandardID AND sd.Divisions_DivisionID = @DivisionID AND sd.Teachers_TeacherID = @TeacherID " +
                    "AND sd.SchoolMaster_SchoolID = @SchoolID;", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@StandardID", int.Parse(ddlStandard.SelectedValue)));
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", int.Parse(ddlDivision.SelectedValue)));
                    cmd.Parameters.Add(new SqlParameter("@TeacherID", TeacherID));
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolId));
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            ddlSubject.DataSource = reader;
                            ddlSubject.DataTextField = "SubjectName";
                            ddlSubject.DataValueField = "SubjectMaster_SubjectID";
                            ddlSubject.DataBind();
                            ddlSubject.Items.Insert(0, new ListItem("--Select Subject--", "0"));
                        }
                        else
                        {
                            ddlExam.Enabled = false;
                        }
                    }
                }
            }
        }

        protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.Parse(ddlSubject.SelectedValue) != 0)
            {
                // Clear existing items
                ddlExam.Items.Clear();

                GetExams();
                ddlExam.Enabled = true;
                gvStudents.Visible = false;
            }
            else
            {
                ddlExam.Items.Clear();
                ddlExam.Items.Add(new ListItem("--Select Exam--", "0"));
                ddlExam.Enabled = false;
                gvStudents.Visible = false;
            }
        }

        private void GetExams()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT DISTINCT es.ExamID, e.ExamName, e.TotalMarks " +
                    "FROM ExamSchedule AS es " +
                    "INNER JOIN Exam AS e ON e.ExamID = es.ExamID " +
                    "INNER JOIN SubjectDetail AS sd ON sd.SubjectMaster_SubjectID = es.SubjectID " +
                    "WHERE es.StandardID = @StandardID AND es.DivisionID = @DivisionID AND es.SubjectID = @SubjectID AND sd.Teachers_TeacherID = @TeacherID AND es.SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", int.Parse(ddlStandard.SelectedValue));
                    cmd.Parameters.AddWithValue("@DivisionID", int.Parse(ddlDivision.SelectedValue));
                    cmd.Parameters.AddWithValue("@SubjectID", int.Parse(ddlSubject.SelectedValue));
                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Clear existing items
                            ddlExam.Items.Clear();

                            // Add default item
                            ddlExam.Items.Add(new ListItem("--Select Exam--", "0"));

                            // Populate dropdown with combined text
                            while (reader.Read())
                            {
                                string combinedText = reader["ExamName"] + " Exam - " + reader["TotalMarks"] + " Marks";
                                string examID = reader["ExamID"].ToString();

                                ddlExam.Items.Add(new ListItem(combinedText, examID));
                            }

                            ddlExam.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        protected void ddlExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.Parse(ddlExam.SelectedValue) != 0)
            {
                gvStudents.Visible = true;
                BindGridview();
            }
            else
            {
                gvStudents.Visible = false;
            }
        }


        private void BindGridview()
        {
            DataTable dtStudents = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT SM.StudentID, SM.Student_FullName, E.TotalMarks, G.Grade 
                FROM StudentMaster SM 
                INNER JOIN Exam E ON E.SchoolID = SM.SchoolMaster_SchoolID AND E.ExamID = @ExamID
                LEFT JOIN Grades G ON G.StudentID = SM.StudentID AND G.ExamMaster_ExamID = @ExamID 
                    AND G.StandardID = @StandardID AND G.DivisionID = @DivisionID AND G.SubjectID = @SubjectID
                WHERE SM.SchoolMaster_SchoolID = @SchoolID AND SM.Student_Standard = @StandardID 
                    AND SM.Student_Division = @DivisionID 
                ORDER BY SM.Student_LastName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                cmd.Parameters.AddWithValue("@ExamID", int.Parse(ddlExam.SelectedValue));
                cmd.Parameters.AddWithValue("@StandardID", int.Parse(ddlStandard.SelectedValue));
                cmd.Parameters.AddWithValue("@DivisionID", int.Parse(ddlDivision.SelectedValue));
                cmd.Parameters.AddWithValue("@SubjectID", int.Parse(ddlSubject.SelectedValue));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtStudents);
            }

            if (dtStudents.Rows.Count > 0)
            {
                gvStudents.DataSource = dtStudents;
                gvStudents.DataBind();
            }
            else
            {
                gvStudents.DataSource = null;
                gvStudents.DataBind();
            }
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
            int selectedExam = int.Parse(ddlExam.SelectedValue);
            int selectedStandard = int.Parse(ddlStandard.SelectedValue);
            int selectedDivision = int.Parse(ddlDivision.SelectedValue);
            int selectedSubject = int.Parse(ddlSubject.SelectedValue);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (GridViewRow row in gvStudents.Rows)
                {
                    // Get StudentID from hidden field
                    HiddenField hfStudentID = (HiddenField)row.FindControl("hfStudentID");
                    string studentID = hfStudentID.Value;

                    // Get Student Name from the label
                    Label lblStudentName = (Label)row.FindControl("lblStudentName");
                    string studentName = lblStudentName.Text;

                    // Get entered grade
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
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Grades WHERE ExamMaster_ExamID = @ExamID AND StudentID = @StudentID AND StandardID = @StandardID AND DivisionID = @DivisionID AND SubjectID = @SubjectID AND SchoolMaster_SchoolID = @SchoolID", conn);
                    checkCmd.Parameters.AddWithValue("@ExamID", selectedExam);
                    checkCmd.Parameters.AddWithValue("@StudentID", studentID);
                    checkCmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                    checkCmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                    checkCmd.Parameters.AddWithValue("@SubjectID", selectedSubject);
                    checkCmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    int recordCount = (int)checkCmd.ExecuteScalar();

                    if (recordCount > 0)
                    {
                        // If the record exists, update the grade
                        SqlCommand updateCmd = new SqlCommand("UPDATE Grades SET Grade = @Grade WHERE ExamMaster_ExamID = @ExamID AND StudentID = @StudentID AND StandardID = @StandardID AND DivisionID = @DivisionID AND SubjectID = @SubjectID AND SchoolMaster_SchoolID = @SchoolID", conn);
                        updateCmd.Parameters.AddWithValue("@Grade", grade);
                        updateCmd.Parameters.AddWithValue("@ExamID", selectedExam);
                        updateCmd.Parameters.AddWithValue("@StudentID", studentID);
                        updateCmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                        updateCmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                        updateCmd.Parameters.AddWithValue("@SubjectID", selectedSubject);
                        updateCmd.Parameters.AddWithValue("@SchoolID", schoolId);

                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // If the record does not exist, insert a new grade
                        SqlCommand insertCmd = new SqlCommand("INSERT INTO Grades (ExamMaster_ExamID, StudentID, StandardID, DivisionID, SubjectID, Grade, SchoolMaster_SchoolID) VALUES (@ExamID, @StudentID, @StandardID, @DivisionID, @SubjectID, @Grade, @SchoolID)", conn);
                        insertCmd.Parameters.AddWithValue("@ExamID", selectedExam);
                        insertCmd.Parameters.AddWithValue("@StudentID", studentID);
                        insertCmd.Parameters.AddWithValue("@StandardID", selectedStandard);
                        insertCmd.Parameters.AddWithValue("@DivisionID", selectedDivision);
                        insertCmd.Parameters.AddWithValue("@SubjectID", selectedSubject);
                        insertCmd.Parameters.AddWithValue("@Grade", grade);
                        insertCmd.Parameters.AddWithValue("@SchoolID", schoolId);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            Response.Write("<script>alert('Grades updated successfully');</script>");
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ddlDivision.Enabled = false;
            ddlSubject.Enabled = false;
            ddlExam.Enabled = false;
            gvStudents.Visible = false;
        }
     
    }
}