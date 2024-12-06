using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Student

{
    public partial class ExamSchedule : System.Web.UI.Page
    {

        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private int UserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            StudentID = GetStudentIDFromSession(UserID, SchoolID);

            if (!IsPostBack)
            {
                lblstandardID.Visible = false;
                lblDivisionID.Visible = false;
                LoadStudentDetails(StudentID, SchoolID);
            }
        }

        private void LoadStudentDetails(string studentID, string schoolID)
        {
            string studentDetailsQuery = @"SELECT 
	                    SM.[Student_Standard] AS [StandardID],
	                    SD.StandardName AS [StandardName],
	                    SM.[Student_Division] AS [DivisionID],
	                    DV.DivisionName AS [DivisionName]
                   from StudentMaster SM 
                   LEFT JOIN Standards SD ON SD.StandardID=SM.Student_Standard
                   LEFT JOIN Divisions DV ON DV.DivisionID=SM.Student_Division
                   where StudentID=@studentID and SM.SchoolMaster_SchoolID=@schoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(studentDetailsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set student details
                            lblstandardID.Text = reader["StandardID"].ToString();
                            lblDivisionID.Text = reader["DivisionID"].ToString();
                            lblStandard.Text = reader["StandardName"].ToString();
                            lblDivision.Text = reader["DivisionName"].ToString();

                            string standardID = lblstandardID.Text.ToString();
                            string DivisionID = lblDivisionID.Text.ToString();

                            // Load Subjects for filtering
                            LoadSubjectsForFilter(standardID, DivisionID);
                            GetExamSchedule(standardID, DivisionID, schoolID);
                        }

                    }
                }
            }

        }

        private void LoadSubjectsForFilter(string standardID, string divisionID)
        {
            string subjectQuery = @"
        SELECT SM.SubjectName, SM.SubjectID
        FROM SubjectMaster SM
        INNER JOIN ExamSchedule ES ON ES.SubjectID = SM.SubjectID
        WHERE ES.StandardID = @StandardID AND ES.DivisionID = @DivisionID
        GROUP BY SM.SubjectName, SM.SubjectID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(subjectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", standardID);
                    cmd.Parameters.AddWithValue("@DivisionID", divisionID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlSubjectFilter.Items.Clear();
                        ddlSubjectFilter.Items.Add(new ListItem("-- Select Subject --", ""));

                        while (reader.Read())
                        {
                            ddlSubjectFilter.Items.Add(new ListItem(reader["SubjectName"].ToString(), reader["SubjectID"].ToString()));
                        }
                    }
                }
            }
        }

        private void GetExamSchedule(string standardID, string divisionID, string schoolID)
        {
            string subjectFilter = ddlSubjectFilter.SelectedValue;

            string examScheduleQuery = @"
        SELECT ES.ExamDate AS ExamDate,
               EX.ExamName AS ExamName,
               EX.TotalMarks AS Marks,
               SM.SubjectName AS SubjectName
        FROM ExamSchedule ES
        LEFT JOIN Standards SD ON SD.StandardID=ES.StandardID
        LEFT JOIN Divisions DV ON DV.DivisionID=ES.DivisionID
        LEFT JOIN SubjectMaster SM ON SM.SubjectID=ES.SubjectID
        LEFT JOIN Exam EX ON EX.ExamID=ES.ExamID
        WHERE ES.StandardID = @StandardID 
          AND ES.DivisionID = @DivisionID 
          AND ES.SchoolID = @SchoolID";

            if (!string.IsNullOrEmpty(subjectFilter))
            {
                examScheduleQuery += " AND SM.SubjectID = @SubjectID";
            }

            examScheduleQuery += " ORDER BY ExamDate";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(examScheduleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    cmd.Parameters.AddWithValue("@StandardID", standardID);
                    cmd.Parameters.AddWithValue("@DivisionID", divisionID);

                    if (!string.IsNullOrEmpty(subjectFilter))
                    {
                        cmd.Parameters.AddWithValue("@SubjectID", subjectFilter);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            gvExamSchedule.DataSource = dt;
                            gvExamSchedule.DataBind();
                        }
                        else
                        {
                            gvExamSchedule.DataSource = null;
                            gvExamSchedule.DataBind();
                        }
                    }
                }
            }
        }

        private string GetStudentIDFromSession(int userID, string schoolID)
        {
            string studentId = string.Empty;

            string query = @"
                SELECT StudentID 
                FROM StudentMaster 
                WHERE UserMaster_UserID = @UserID 
                AND SchoolMaster_SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                conn.Open();
                studentId = cmd.ExecuteScalar()?.ToString();
            }

            return studentId;
        }

        protected void ddlSubjectFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string standardID = lblstandardID.Text;
            string divisionID = lblDivisionID.Text;
            string schoolID = Session["SchoolID"].ToString();

            GetExamSchedule(standardID, divisionID, schoolID);  // Rebind the GridView with the selected subject filter
        }
    }
}