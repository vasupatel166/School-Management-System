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
        private int ClassTeacher;
        private int UserID;
        private string AcademicYear;

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

        private void GETStudentID()
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

        private void GetClassTeacher()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM AssignTeacher WHERE TeacherID = @TeacherID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", StudentID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    conn.Open();

                    // Use ExecuteScalar to get the count directly
                    int count = (int)cmd.ExecuteScalar();

                    // If the count is greater than 0, the teacher is assigned
                    ClassTeacher = count > 0 ? 1 : 0;
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

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Fetch dashboard summary counts
                        if (reader.Read())
                        {
                            ToadyClasses.InnerText = reader["TodaysClasses"]?.ToString() ?? "0";
                            TotalClasses.InnerText = reader["TotalWeeklyClasses"]?.ToString() ?? "0";
                            DaysPresent.InnerText = reader["DaysPresent"]?.ToString() ?? "0";
                            DaysAbsent.InnerText = reader["DaysAbsent"]?.ToString() ?? "0";
                        }

                        // Fetch timetable
                        if (reader.NextResult())
                        {
                            TodaysTimetableRepeater.DataSource = reader;
                            TodaysTimetableRepeater.DataBind();
                        }

                        // Fetch upcoming events
                        if (reader.NextResult())
                        {
                            var eventsHtml = new System.Text.StringBuilder();
                            while (reader.Read())
                            {
                                // Get EventTime as TimeSpan and format it to AM/PM
                                var eventTime = reader["EventTime"] as TimeSpan?;
                                string formattedTime = eventTime.HasValue
                                    ? DateTime.Today.Add(eventTime.Value).ToString("hh:mm tt") // Convert to DateTime and format
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



                        // Fetch upcoming holidays
                        if (reader.NextResult())
                        {
                            var holidaysHtml = new System.Text.StringBuilder();
                            while (reader.Read())
                            {
                                holidaysHtml.Append($@"
                            <li class='list-group-item p-3 d-flex justify-content-between align-items-center'>
                                <div class='w-100 d-flex justify-content-between align-items-center'>
                                    <strong>{reader["HolidayName"]}</strong><br>
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
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable attendanceData = new DataTable();
                        attendanceData.Load(reader);

                        if (attendanceData.Rows.Count > 0)
                        {
                            // Convert attendance data to JSON for FullCalendar
                            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(
                                attendanceData.AsEnumerable().Select(row => new
                                {
                                    title = row["Status"].ToString(),
                                    start = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                                    backgroundColor = row["Status"].ToString() == "Present" ? "#28a745" : "#dc3545",
                                    borderColor = row["Status"].ToString() == "Present" ? "#28a745" : "#dc3545"
                                })
                            );

                            // Pass JSON data to the hidden field
                            AttendanceDataHiddenField.Value = jsonData;
                        }
                        else
                        {
                            AttendanceDataHiddenField.Value = "[]"; // No data
                        }
                    }
                }
            }
        }



    }
}