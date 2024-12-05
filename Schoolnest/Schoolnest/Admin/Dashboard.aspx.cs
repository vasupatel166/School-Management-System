using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Schoolnest.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private static string connectionString = Global.ConnectionString;
        public static string SchoolID = "";
        private string AcademicYear;
        private string FirstTermFeeDueDate;
        private string SecondTermFeeDueDate;
        private int LateFeeCharges;
        private int TotalSchoolFeesPending = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();
            GetSchoolSettings();
            GetTotalSchoolFeesPending();

            GetSchoolName();
            GetAdminDashboardData();

            FeesRemaining.InnerHtml = "₹ " + TotalSchoolFeesPending.ToString("#,##0");

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            startDate = DateTime.Today.AddDays(-7);
            endDate = DateTime.Today;

            LoadAttendanceData(startDate, endDate, SchoolID);
        }

        private void GetSchoolSettings()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AcademicYear = reader["AcademicYear"]?.ToString();
                            FirstTermFeeDueDate = reader["FirstTermFeeDueDate"].ToString();
                            SecondTermFeeDueDate = reader["SecondTermFeeDueDate"].ToString();
                            LateFeeCharges = int.Parse(reader["LateFeeCharges"].ToString());
                        }
                    }
                }
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

        private void GetTotalSchoolFeesPending()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(@"SELECT StudentID, Student_Standard, Student_Division FROM StudentMaster 
                Where SchoolMaster_SchoolID = @SchoolID AND IsActive = 1", conn))
                {

                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {

                            string StudentID = reader["StudentID"].ToString();
                            int StandardID = int.Parse(reader["Student_Standard"].ToString());
                            int DivisionID = int.Parse(reader["Student_Division"].ToString());

                            GetFeeDetailsForTermsWithTotalDue(StudentID, StandardID, DivisionID);
                        }
                    }
                }
            }
        }

        private void GetFeeDetailsForTermsWithTotalDue(string StudentID, int StandardID, int DivisionID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetFeeDetailsForTermsWithTotalDue", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StandardID", StandardID);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@DivisionID", DivisionID);
                    cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear);

                    DateTime FirstDueDate;
                    if (DateTime.TryParse(FirstTermFeeDueDate, out FirstDueDate))
                    {
                        cmd.Parameters.AddWithValue("@FirstTermDueDate", FirstDueDate.Date);
                    }

                    DateTime SecondDueDate;
                    if (DateTime.TryParse(SecondTermFeeDueDate, out SecondDueDate))
                    {
                        cmd.Parameters.AddWithValue("@SecondTermDueDate", SecondDueDate.Date);
                    }
                    cmd.Parameters.AddWithValue("@LateFeeCharges", LateFeeCharges);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int TotalDue = int.Parse(reader["TotalFeesDue"].ToString());

                            TotalSchoolFeesPending += TotalDue; 
                        }
                    }
                }
            }
        }

        private void GetAdminDashboardData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAdminDashboardData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TotalClasses.InnerHtml = reader["ActiveClasses"].ToString();
                            TotalStudents.InnerHtml = reader["TotalStudents"].ToString();
                            TotalTeachers.InnerHtml = reader["TotalTeachers"].ToString();
                            TotalSubjects.InnerHtml = reader["TotalSubjects"].ToString();
                            TotalBuses.InnerHtml = reader["TotalBuses"].ToString();

                            decimal totalBudgetRemaining = Convert.ToDecimal(reader["TotalBudgetRemaining"]);
                            TotalBudgetRemaining.InnerHtml = "₹ " + totalBudgetRemaining.ToString("#,##0");

                        }

                        if (reader.NextResult())
                        {
                            var eventsHtml = new StringBuilder();
                            while (reader.Read())
                            {
                                var eventTime = reader["EventTime"] as TimeSpan?;
                                string formattedTime = eventTime.HasValue
                                    ? DateTime.Today.Add(eventTime.Value).ToString("hh:mm tt")
                                    : "N/A";

                                eventsHtml.Append($@"
                                <li class='list-group-item p-1 d-flex justify-content-between align-items-center'>
                                    <div>
                                        <strong>{reader["EventTitle"]}</strong><br>
                                        <small>{Convert.ToDateTime(reader["EventDate"]).ToString("MMM dd, yyyy")}</small>
                                    </div>
                                    <span class='badge bg-primary'>{formattedTime}</span>
                                </li>");
                            }

                            // If no events are found, display a placeholder
                            if (eventsHtml.Length == 0)
                            {
                                eventsHtml.Append("<li class='list-group-item'>No upcoming events</li>");
                            }
                            UpcomingEvents.InnerHtml = eventsHtml.ToString();
                        }

                        if (reader.NextResult())
                        {
                            var holidaysHtml = new StringBuilder();
                            while (reader.Read())
                            {
                                holidaysHtml.Append($@"
                                <li class='list-group-item p-3 d-flex justify-content-between align-items-center'>
                                    <div class='w-100 d-flex justify-content-between align-items-center'>
                                        <strong>{reader["HolidayName"]}</strong>
                                        <small>{Convert.ToDateTime(reader["HolidayDate"]).ToString("MMM dd, yyyy")}</small>
                                    </div>
                                </li>");
                            }

                            // If no holidays are found, display a placeholder
                            if (holidaysHtml.Length == 0)
                            {
                                holidaysHtml.Append("<li class='list-group-item'>No upcoming holidays</li>");
                            }
                            UpcomingHolidays.InnerHtml = holidaysHtml.ToString();
                        }
                    }
                }
            }
        }

        protected void ddlAttendanceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string attendanceType = ddlAttendanceType.SelectedValue;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            switch (attendanceType)
            {
                case "Weekly":
                    startDate = DateTime.Today.AddDays(-7);
                    endDate = DateTime.Today;
                    break;
                case "Monthly":
                    startDate = DateTime.Today.AddDays(-30);
                    endDate = DateTime.Today;
                    break;
                case "Yearly":
                    startDate = new DateTime(DateTime.Today.Year, 1, 1);
                    endDate = DateTime.Today;
                    break;
            }

            LoadAttendanceData(startDate, endDate, SchoolID);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "UpdateChart", "updateChart();", true);
        }

        private void LoadAttendanceData(DateTime startDate, DateTime endDate, string schoolID)
        {
            int StudentDaysPresent = 0;
            int StudentDaysAbsent = 0;

            int TeacherDaysPresent = 0;
            int TeacherDaysAbsent = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
                    SELECT Status FROM StudentAttendance 
                    WHERE SchoolMaster_SchoolID = @SchoolID AND Date BETWEEN @StartDate AND @EndDate;

                    SELECT Status FROM TeacherAttendance 
                    WHERE SchoolMaster_SchoolID = @SchoolID AND Date BETWEEN @StartDate AND @EndDate;", conn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Process Student Attendance
                            while (reader.Read())
                            {
                                string status = reader["Status"].ToString();

                                if (status == "Present")
                                {
                                    StudentDaysPresent++;
                                }
                                else if (status == "Absent")
                                {
                                    StudentDaysAbsent++;
                                }
                            }

                            // Move to the next result set (Teacher Attendance)
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    string status = reader["Status"].ToString();

                                    if (status == "Present")
                                    {
                                        TeacherDaysPresent++;
                                    }
                                    else if (status == "Absent")
                                    {
                                        TeacherDaysAbsent++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading attendance data: " + ex.Message);
                }
            }

            var StudentAttendanceData = new { DaysPresent = StudentDaysPresent, DaysAbsent = StudentDaysAbsent };
            StudentAttendanceDataHiddenField.Value = JsonConvert.SerializeObject(StudentAttendanceData);

            var TeacherAttendanceData = new { DaysPresent = TeacherDaysPresent, DaysAbsent = TeacherDaysAbsent };
            TeacherAttendanceDataHiddenField.Value = JsonConvert.SerializeObject(TeacherAttendanceData);
        }

    }
}
