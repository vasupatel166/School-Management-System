using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Student
{
    public partial class Timetable : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string Username;
        private int UserID;
        private string StudentID;
        private int standardID;
        private int divisionID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            UserID = Convert.ToInt32(Session["UserID"]);

            GetStudentID();

            if (!IsPostBack)
            {
                LoadStudentTimetable();
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

        private string FormatPeriodTime(string periodTime)
        {
            try
            {
                string startTime = periodTime.Split('-')[0].Trim();
                DateTime time = DateTime.Parse(startTime);
                return time.ToString("h:mm tt"); // Format as "8:00 AM"
            }
            catch
            {
                return periodTime; // Return original if parsing fails
            }
        }

        private void LoadStudentTimetable()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string standardName = "", divisionName = "";
                    string query = @"
                    SELECT SM.Student_Standard, SM.Student_Division, ST.StandardName, D.DivisionName
                    FROM StudentMaster SM
                    INNER JOIN Standards ST ON SM.Student_Standard = ST.StandardID
                    INNER JOIN Divisions D ON SM.Student_Division = D.DivisionID
                    WHERE SM.StudentID = @StudentID AND SM.SchoolMaster_SchoolID = @SchoolID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", StudentID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                standardID = int.Parse(reader["Student_Standard"].ToString());
                                divisionID = int.Parse(reader["Student_Division"].ToString());
                                standardName = reader["StandardName"].ToString();
                                divisionName = reader["DivisionName"].ToString();
                            }
                        }
                    }

                    // Display Standard and Division
                    lblStandard.Text = $"Standard : {standardName} - {divisionName}";

                    // Build timetable header
                    StudentTimetable.Rows.Clear();
                    TableHeaderRow headerRow = new TableHeaderRow();
                    headerRow.Cells.Add(new TableHeaderCell { Text = "Day" });

                    string periodQuery = @"
                    SELECT PeriodTimeID, PeriodTime, IsBreakTime
                    FROM PeriodTimeMaster 
                    WHERE SchoolMaster_SchoolID = @SchoolID 
                    ORDER BY PeriodTimeID";

                    using (SqlCommand periodCmd = new SqlCommand(periodQuery, conn))
                    {
                        periodCmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        using (SqlDataReader periodReader = periodCmd.ExecuteReader())
                        {
                            while (periodReader.Read())
                            {
                                TableHeaderCell periodCell = new TableHeaderCell();
                                periodCell.Text = FormatPeriodTime(periodReader["PeriodTime"].ToString());
                                headerRow.Cells.Add(periodCell);
                            }
                        }
                    }

                    StudentTimetable.Rows.Add(headerRow);

                    // Timetable Query
                    string timetableQuery = @"
                WITH DaysOfWeek AS (
                    SELECT 'Monday' as DayOfWeek, 1 as DayOrder UNION
                    SELECT 'Tuesday', 2 UNION
                    SELECT 'Wednesday', 3 UNION
                    SELECT 'Thursday', 4 UNION
                    SELECT 'Friday', 5 UNION
                    SELECT 'Saturday', 6
                )
                SELECT 
                    DOW.DayOfWeek,
                    PTM.PeriodTimeID,
                    PTM.PeriodTime,
                    PTM.IsBreakTime,
                    SM.SubjectName,
                    TM.TeacherName
                FROM DaysOfWeek DOW
                CROSS JOIN PeriodTimeMaster PTM
                LEFT JOIN TimeTableMaster TTM ON TTM.DayOfWeek = DOW.DayOfWeek 
                    AND TTM.PeriodTimeID = PTM.PeriodTimeID
                    AND TTM.SchoolMaster_SchoolID = @SchoolID
                LEFT JOIN SubjectDetail SD ON TTM.SubjectDetail_SubjectDetailID = SD.SubjectDetailID
                    AND SD.Standards_StandardID = @StandardID
                    AND SD.Divisions_DivisionID = @DivisionID
                LEFT JOIN SubjectMaster SM ON SD.SubjectMaster_SubjectID = SM.SubjectID
                LEFT JOIN TeacherMaster TM ON SD.Teachers_TeacherID = TM.TeacherID
                WHERE PTM.SchoolMaster_SchoolID = @SchoolID
                ORDER BY DOW.DayOrder, PTM.PeriodTimeID";

                    using (SqlCommand timetableCmd = new SqlCommand(timetableQuery, conn))
                    {
                        timetableCmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        timetableCmd.Parameters.AddWithValue("@StandardID", standardID);
                        timetableCmd.Parameters.AddWithValue("@DivisionID", divisionID);

                        using (SqlDataReader timetableReader = timetableCmd.ExecuteReader())
                        {
                            string currentDay = "";
                            TableRow currentRow = null;

                            while (timetableReader.Read())
                            {
                                string dayOfWeek = timetableReader["DayOfWeek"].ToString();
                                bool isBreakTime = Convert.ToBoolean(timetableReader["IsBreakTime"]);

                                if (dayOfWeek != currentDay)
                                {
                                    if (currentRow != null)
                                    {
                                        StudentTimetable.Rows.Add(currentRow);
                                    }

                                    currentRow = new TableRow();
                                    TableHeaderCell dayCell = new TableHeaderCell { Text = dayOfWeek };
                                    currentRow.Cells.Add(dayCell);
                                    currentDay = dayOfWeek;
                                }

                                TableCell periodCell = new TableCell();

                                if (isBreakTime)
                                {
                                    periodCell.Text = "Recess";
                                    periodCell.CssClass = "break-period";
                                }
                                else
                                {
                                    string subjectName = timetableReader["SubjectName"].ToString();
                                    string teacherName = timetableReader["TeacherName"].ToString();

                                    if (!string.IsNullOrEmpty(subjectName))
                                    {
                                        periodCell.Text = subjectName;
                                        periodCell.Attributes["title"] = $"Teacher: {teacherName}";
                                        periodCell.CssClass = "assigned-period";
                                    }
                                    else
                                    {
                                        periodCell.Text = "Not Assigned";
                                        periodCell.CssClass = "not-assigned-period";
                                    }
                                }

                                currentRow.Cells.Add(periodCell);
                            }

                            if (currentRow != null)
                            {
                                StudentTimetable.Rows.Add(currentRow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadStudentTimetable: {ex.Message}");
                throw;
            }
        }

    }
}
