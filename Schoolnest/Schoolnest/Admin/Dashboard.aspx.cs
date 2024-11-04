using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web;

namespace Schoolnest.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private static string connectionString = Global.ConnectionString;
        public static string SchoolID = "";

        // Properties to hold the data to pass to the front-end
        public string Labels { get; set; }
        public string StudentData { get; set; }
        public string TeacherData { get; set; }
        public string StudentTooltips { get; set; }
        public string TeacherTooltips { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SchoolID = Session["SchoolID"].ToString();
                GetAdminDashboardData(SchoolID);
            }
        }

        private void GetAdminDashboardData(string schoolId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAdminDashboardData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@AcademicYear", "2024-2025");

                    // OUTPUT parameters
                    SqlParameter totalStudentsParam = new SqlParameter("@TotalStudents", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    SqlParameter totalTeachersParam = new SqlParameter("@TotalTeachers", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    SqlParameter activeClassesParam = new SqlParameter("@ActiveClasses", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    SqlParameter pendingFeesParam = new SqlParameter("@PendingFees", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(totalStudentsParam);
                    cmd.Parameters.Add(totalTeachersParam);
                    cmd.Parameters.Add(activeClassesParam);
                    cmd.Parameters.Add(pendingFeesParam);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Handle Upcoming Events
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string eventTitle = reader["EventTitle"].ToString();
                            DateTime eventDate = (DateTime)reader["EventDate"];

                            // Retrieve EventTime as TimeSpan if it's of TIME type in the database
                            TimeSpan? eventTime = reader["EventTime"] != DBNull.Value ? (TimeSpan?)reader["EventTime"] : null;

                            if (eventTime.HasValue)
                            {
                                // Convert TimeSpan to DateTime for display purposes
                                DateTime eventDateTime = DateTime.Today.Add(eventTime.Value);
                                AddUpcomingEvent(eventTitle, eventDate, eventDateTime);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Warning: EventTime is null for event " + eventTitle);
                            }
                        }
                    }

                    // Move to the next result set (attendance records)
                    //if (reader.NextResult())
                    //{
                    //    List<string> labels = new List<string>(); // This will store Mon, Tue, etc.
                    //    List<double> studentAttendance = new List<double>();
                    //    List<double> teacherAttendance = new List<double>();
                    //    List<string> studentTooltips = new List<string>(); // To store actual values for tooltips
                    //    List<string> teacherTooltips = new List<string>();

                    //    while (reader.Read())
                    //    {
                    //        DateTime attendanceDate = (DateTime)reader["AttendanceDate"];
                    //        labels.Add(attendanceDate.ToString("ddd"));  // Example: "Mon", "Tue"

                    //        // Calculate student and teacher attendance percentages
                    //        int totalStudentsPresent = Convert.ToInt32(reader["TotalStudentsPresent"]);
                    //        int totalTeachersPresent = Convert.ToInt32(reader["TotalTeachersPresent"]);

                    //        double studentPercentage = (int)totalStudentsParam.Value > 0 ? ((double)totalStudentsPresent / (int)totalStudentsParam.Value) * 100 : 0;
                    //        double teacherPercentage = (int)totalTeachersParam.Value > 0 ? ((double)totalTeachersPresent / (int)totalTeachersParam.Value) * 100 : 0;

                    //        studentAttendance.Add(studentPercentage);
                    //        teacherAttendance.Add(teacherPercentage);

                    //        // Tooltips to show the actual values
                    //        studentTooltips.Add($"{totalStudentsPresent} / {(int)totalStudentsParam.Value}");
                    //        teacherTooltips.Add($"{totalTeachersPresent} / {(int)totalTeachersParam.Value}");
                    //    }

                    //    // Convert data into JSON format for use in the front-end
                    //    Labels = "'" + string.Join("','", labels) + "'";
                    //    StudentData = string.Join(",", studentAttendance);
                    //    TeacherData = string.Join(",", teacherAttendance);
                    //    StudentTooltips = "'" + string.Join("','", studentTooltips) + "'";
                    //    TeacherTooltips = "'" + string.Join("','", teacherTooltips) + "'";
                    //}

                    reader.Close(); // Close the reader to access output parameters.

                    // Now access the OUTPUT parameters after closing the reader
                    TotalStudentsCard.Text = totalStudentsParam.Value != DBNull.Value ? totalStudentsParam.Value.ToString() : "0";
                    TotalTeachersCard.Text = totalTeachersParam.Value != DBNull.Value ? totalTeachersParam.Value.ToString() : "0";
                    TotalActiveClassesCard.Text = activeClassesParam.Value != DBNull.Value ? activeClassesParam.Value.ToString() : "0";
                    TotalPendingFeesCard.Text = pendingFeesParam.Value != DBNull.Value ? pendingFeesParam.Value.ToString() : "0";
                }
            }
        }

        private void AddUpcomingEvent(string title, DateTime date, DateTime time)
        {

            string formattedTime = time.ToString("hh:mm tt");

            // Create a div for each event
            string eventHtml = $@"
                <div class='item-list'>
                    <div class='info-user ms-3'>
                        <div class='username'>{title}</div>
                        <div class='status'>{formattedTime}</div>
                    </div>
                    <p>{date.ToString("dd MMM, yyyy")}</p>
                </div>";

            // Registering the event list as a control on the server-side
            UpcomingEvents.InnerHtml += eventHtml;
        }
    }
}
