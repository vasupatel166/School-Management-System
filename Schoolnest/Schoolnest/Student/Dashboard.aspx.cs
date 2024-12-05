using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace Schoolnest.Student
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private int StandardID;
        private int DivisionID;
        private int UserID;
        private string AcademicYear;
        private string FirstTermFeeDueDate;
        private string SecondTermFeeDueDate;
        private int LateFeeCharges;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetStudentDetails();
            GetSchoolSettings();

            if (!IsPostBack)
            {
                GetSchoolName();
                GetFeeDetailsForTermsWithTotalDue();
                LoadStudentDashboardData();

                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                startDate = DateTime.Today.AddDays(-7);
                endDate = DateTime.Today;

                LoadAttendanceData(startDate,endDate, SchoolID, StudentID);
            }
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

        private void GetStudentDetails()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT StudentID, Student_Standard, Student_Division FROM StudentMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            StudentID = reader["StudentID"]?.ToString();
                            StandardID = int.Parse(reader["Student_Standard"].ToString());
                            DivisionID = int.Parse(reader["Student_Division"].ToString());
                        }
                    }
                }
            }
        }

        private void GetFeeDetailsForTermsWithTotalDue()
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

                            string TotalDue = reader["TotalFeesDue"].ToString();
                            decimal totalDueAmount;

                            if (decimal.TryParse(TotalDue, out totalDueAmount))
                            {
                                FeesRemaining.InnerHtml = string.Format("₹ {0:N0}", totalDueAmount);
                            }
                            else
                            {
                                FeesRemaining.InnerHtml = "₹ 0";
                            }
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

                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Read the summary data first
                        if (reader.Read())
                        {
                            string standardName = reader["StandardName"].ToString();
                            string divisionName = reader["DivisionName"].ToString();
                            int daysPresent = Convert.ToInt32(reader["DaysPresent"]);
                            int daysAbsent = Convert.ToInt32(reader["DaysAbsent"]);

                            // Set values to labels on the page
                            StudentStandardName.InnerText = $"{standardName} - {divisionName}";
                            DaysPresent.InnerText = daysPresent.ToString();
                            DaysAbsent.InnerText = daysAbsent.ToString();
                        }

                        // Read Today's Timetable
                        if (reader.NextResult() && reader.HasRows)
                        {
                            TodaysTimetableRepeater.DataSource = reader;
                            TodaysTimetableRepeater.DataBind();
                        }

                        // Read Upcoming Events
                        if (reader.NextResult())
                        {
                            var eventsHtml = new StringBuilder();
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

                        // Read Upcoming Holidays
                        if (reader.NextResult())
                        {
                            var holidaysHtml = new StringBuilder();
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

                        // Read Upcoming Exams
                        if (reader.NextResult())
                        {
                            var examsHtml = new StringBuilder();
                            examsHtml.Append(@"
                            <table class='table table-bordered'>
                                <thead>
                                    <tr>
                                        <th>Exam Name</th>
                                        <th>Subject</th>
                                        <th>Exam Date</th>
                                        <th>Total Marks</th>
                                    </tr>
                                </thead>
                                <tbody>");
                                    while (reader.Read())
                                    {
                                        string examDate = Convert.ToDateTime(reader["ExamDate"]).ToString("MMM dd, yyyy");
                                        examsHtml.Append($@"
                                <tr>
                                    <td>{reader["ExamName"]}</td>
                                    <td>{reader["ExamSubject"]}</td>
                                    <td>{examDate}</td>
                                    <td>{reader["TotalMarks"]}</td>
                                </tr>");
                            }

                            // If no exams are found, display a placeholder
                            if (examsHtml.Length == 0)
                            {
                                examsHtml.Append("<tr><td colspan='4'>No upcoming exams</td></tr>");
                            }

                            examsHtml.Append("</tbody></table>");
                            UpcomingExams.InnerHtml = examsHtml.ToString();
                        }

                        // Read Last Exam Grades
                        if (reader.NextResult())
                        {
                            var gradesHtml = new StringBuilder();
                            gradesHtml.Append(@"
                            <table class='table table-bordered'>
                                <thead>
                                    <tr>
                                        <th>Exam Name</th>
                                        <th>Subject</th>
                                        <th>Grade</th>
                                        <th>Total Marks</th>
                                    </tr>
                                </thead>
                                <tbody>");
                                    while (reader.Read())
                                    {
                                        gradesHtml.Append($@"
                                <tr>
                                    <td>{reader["ExamName"]}</td>
                                    <td>{reader["ExamSubject"]}</td>
                                    <td>{reader["Grade"]}</td>
                                    <td>{reader["TotalMarks"]}</td>
                                </tr>");
                            }

                            // If no grades are found, display a placeholder
                            if (gradesHtml.Length == 0)
                            {
                                gradesHtml.Append("<tr><td colspan='5'>No exam grades available</td></tr>");
                            }

                            gradesHtml.Append("</tbody></table>");
                            LastExamGrades.InnerHtml = gradesHtml.ToString();
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

            LoadAttendanceData(startDate, endDate, SchoolID, StudentID);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "UpdateChart", "updateChart();", true);
        }

        private void LoadAttendanceData(DateTime startDate, DateTime endDate, string schoolID, string studentID)
        {
            int daysPresent = 0;
            int daysAbsent = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("GetStudentAttendance", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string status = reader["Status"].ToString();

                                if (status == "Present")
                                {
                                    daysPresent++;
                                }
                                else if (status == "Absent")
                                {
                                    daysAbsent++;
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

            var attendanceData = new { DaysPresent = daysPresent, DaysAbsent = daysAbsent };
            AttendanceDataHiddenField.Value = JsonConvert.SerializeObject(attendanceData);
        }


    }
}