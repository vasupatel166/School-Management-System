using System;
using System.Data.SqlClient;
using System.Data;

namespace Schoolnest.Teacher
{
    public partial class ViewExamSchedule : System.Web.UI.Page
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
                LoadExamSchedule();
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

        private void LoadExamSchedule()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT es.ExamDate, e.ExamName, e.TotalMarks, std.StandardName, div.DivisionName, subj.SubjectName 
                                 FROM ExamSchedule es 
                                 JOIN Exam e ON es.ExamID = e.ExamID 
                                 JOIN SubjectMaster subj ON es.SubjectID = subj.SubjectID 
                                 JOIN Standards std ON es.StandardID = std.StandardID 
                                 JOIN Divisions div ON es.DivisionID = div.DivisionID 
                                 JOIN SubjectDetail tcs ON es.SubjectID = tcs.SubjectMaster_SubjectID 
                                 AND es.StandardID = tcs.Standards_StandardID 
                                 AND es.DivisionID = tcs.Divisions_DivisionID 
                                 WHERE tcs.Teachers_TeacherID = @TeacherID 
                                 AND es.SchoolID = @SchoolID 
                                 ORDER BY es.ExamDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
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
    }
}
