using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Student
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private int UserID;
        private string AcademicYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetStudentID();

            if (!IsPostBack)
            {
                
                GetSchoolName();
                GetAcademicYear();
                LoadStudentDashboardData();
                LoadAttendanceData(DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek), DateTime.Now);
            }
        }

        private void GetSchoolName()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSchoolName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SchoolNameHeader.InnerHtml = reader["SchoolName"].ToString();
                        }
                    }
                }
            }
        }

        private void GetStudentID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT StudentID FROM StudentMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            StudentID = reader["StudentID"]?.ToString();
                        }
                    }
                }
            }
        }

        private void GetAcademicYear()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT AcademicYear FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AcademicYear = reader["AcademicYear"]?.ToString();
                        }
                    }
                }
            }
        }

        private void LoadStudentDashboardData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetStudentDashboardData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear);
                }
            }
        }

        protected void ddlAttendanceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string attendanceType = ddlAttendanceType.SelectedValue;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            // Determine date range based on attendance type
            switch (attendanceType)
            {
                case "Weekly":
                    startDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek); // Start of the week
                    endDate = startDate.AddDays(6); // End of the week
                    break;
                case "Monthly":
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Start of the month
                    endDate = startDate.AddMonths(1).AddDays(-1); // End of the month
                    break;
                case "Yearly":
                    startDate = new DateTime(DateTime.Now.Year, 1, 1); // Start of the year
                    endDate = new DateTime(DateTime.Now.Year, 12, 31); // End of the year
                    break;
            }

            LoadAttendanceData(startDate, endDate);
        }

        private void LoadAttendanceData(DateTime startDate, DateTime endDate)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetStudentAttendance", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    //cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    //cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    //cmd.Parameters.AddWithValue("@StartDate", startDate);
                    //cmd.Parameters.AddWithValue("@EndDate", endDate);
                }
            }
        }

    }
}