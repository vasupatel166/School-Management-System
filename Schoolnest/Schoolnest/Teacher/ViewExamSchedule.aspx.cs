using System;
using System.Data.SqlClient;
using System.Data;

namespace Schoolnest.Teacher
{
    public partial class ViewExamSchedule : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        string Username = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"]?.ToString();
                Username = Session["Username"]?.ToString();

                if (!string.IsNullOrEmpty(schoolId) && !string.IsNullOrEmpty(Username))
                {
                    LoadExamSchedule(schoolId, Username);
                }
                else
                {
                    // Handle session expiration
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        private void LoadExamSchedule(string schoolId, string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT es.ExamDate, e.ExamName, std.StandardName, div.DivisionName, subj.SubjectName 
                                 FROM ExamSchedule es 
                                 JOIN Exam e ON es.ExamID = e.ExamID 
                                 JOIN SubjectMaster subj ON es.SubjectID = subj.SubjectID 
                                 JOIN Standards std ON es.StandardID = std.StandardID 
                                 JOIN Divisions div ON es.DivisionID = div.DivisionID 
                                 JOIN SubjectDetail tcs ON es.SubjectID = tcs.SubjectMaster_SubjectID 
                                 AND es.StandardID = tcs.Standards_StandardID 
                                 AND es.DivisionID = tcs.Divisions_DivisionID 
                                 WHERE tcs.Teachers_TeacherID = @Username 
                                 AND es.SchoolID = @SchoolID 
                                 ORDER BY es.ExamDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@Username", username);

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
                            gvExamSchedule.DataBind(); // This will trigger EmptyDataTemplate
                        }
                    }
                }
            }
        }
    }
}
