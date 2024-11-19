using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                schoolId = Session["SchoolId"].ToString();
                Username = Session["Username"].ToString();
                LoadExamSchedule(schoolId,Username);
            }

        }

        private void LoadExamSchedule(string schoolId, string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT es.ExamDate, e.ExamName, std.StandardName, div.DivisionName, subj.SubjectName " +
                                                       "FROM ExamSchedule es JOIN Exam e ON es.ExamID = e.ExamID " +
                                                       "JOIN SubjectMaster subj ON es.SubjectID = subj.SubjectID JOIN Standards std ON es.StandardID = std.StandardID JOIN Divisions div ON es.DivisionID = div.DivisionID JOIN SubjectDetail tcs ON es.SubjectID = tcs.SubjectMaster_SubjectID " + 
                                                       "AND es.StandardID = tcs.Standards_StandardID AND es.DivisionID = tcs.Divisions_DivisionID " +
                                                       " WHERE tcs.Teachers_TeacherID = @Username and es.SchoolID=@SchoolID; ", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@Username", username);
                    conn.Open();
                    gvExamSchedule.DataSource = cmd.ExecuteReader();
                    gvExamSchedule.DataBind();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Teacher/Dashboard.aspx");
        }
    }
}