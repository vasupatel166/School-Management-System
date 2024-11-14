using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class ViewTimetable : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string Username;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            Username = Session["Username"]?.ToString();

            if (!IsPostBack)
            {
                LoadTeacherDropdown();
                if (ddlTeacher.SelectedValue != "0")
                {
                    LoadTeacherTimetable();
                }
            }
        }

        protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlTeacher.SelectedValue != "0")
                {
                    LoadTeacherTimetable();
                }
                else
                {
                    TeacherTimetable.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ddlTeacher_SelectedIndexChanged: {ex.Message}");
            }
        }

        private void LoadTeacherDropdown()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT 
                        TM.TeacherID,
                        TM.TeacherName,
                        UM.UserID,
                        ISNULL((SELECT TM.TeacherID
                                FROM TeacherMaster TM
                                INNER JOIN UserMaster UM ON TM.UserMaster_UserID = UM.UserID
                                WHERE UM.Username = @Username 
                                  AND UM.RoleMaster_RoleID = 'T' 
                                  AND UM.SchoolMaster_SchoolID = @SchoolID), 0) AS LoggedInTeacherID
                    FROM TeacherMaster TM
                    INNER JOIN UserMaster UM ON TM.UserMaster_UserID = UM.UserID
                    WHERE TM.SchoolMaster_SchoolID = @SchoolID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            string loggedInTeacherID = "0";
                            ddlTeacher.Items.Clear();
                            ddlTeacher.Items.Add(new ListItem("Select Teacher", "0"));

                            while (reader.Read())
                            {
                                string teacherID = reader["TeacherID"].ToString();
                                string teacherName = reader["TeacherName"].ToString();
                                ddlTeacher.Items.Add(new ListItem(teacherName, teacherID));

                                if (loggedInTeacherID == "0")
                                {
                                    loggedInTeacherID = reader["LoggedInTeacherID"].ToString();
                                }
                            }

                            if (ddlTeacher.Items.FindByValue(loggedInTeacherID) != null)
                            {
                                ddlTeacher.SelectedValue = loggedInTeacherID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error appropriately
                System.Diagnostics.Debug.WriteLine($"Error in LoadTeacherDropdown: {ex.Message}");
                throw;
            }
        }


        private string FormatPeriodTime(string periodTime)
        {
            try
            {
                // Split the period time range and take only the start time
                string startTime = periodTime.Split('-')[0].Trim();
                // Convert to DateTime to ensure proper formatting
                DateTime time = DateTime.Parse(startTime);
                return time.ToString("h:mm tt"); // Will format as "8:00 AM"
            }
            catch
            {
                return periodTime; // Return original if parsing fails
            }
        }

        private void LoadTeacherTimetable()
        {
            try
            {
                if (string.IsNullOrEmpty(ddlTeacher.SelectedValue) || ddlTeacher.SelectedValue == "0")
                    return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    TeacherTimetable.Rows.Clear();
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

                                string periodTime = periodReader["PeriodTime"].ToString();
                                periodCell.Text = FormatPeriodTime(periodTime);
                                headerRow.Cells.Add(periodCell);
                            }
                        }
                    }

                    TeacherTimetable.Rows.Add(headerRow);

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
                            ISNULL(SD.SubjectMaster_SubjectID, 0) as SubjectID,
                            SM.SubjectName,
                            STD.StandardName,
                            DM.DivisionName,
                            TTM.TimeTableID
                        FROM DaysOfWeek DOW
                        CROSS JOIN PeriodTimeMaster PTM
                        LEFT JOIN TimeTableMaster TTM ON TTM.DayOfWeek = DOW.DayOfWeek 
                            AND TTM.PeriodTimeID = PTM.PeriodTimeID
                            AND TTM.SchoolMaster_SchoolID = @SchoolID
                        LEFT JOIN SubjectDetail SD ON TTM.SubjectDetail_SubjectDetailID = SD.SubjectDetailID
                            AND SD.Teachers_TeacherID = @TeacherID
                        LEFT JOIN SubjectMaster SM ON SD.SubjectMaster_SubjectID = SM.SubjectID
                        LEFT JOIN Standards STD ON SD.Standards_StandardID = STD.StandardID
                        LEFT JOIN Divisions DM ON SD.Divisions_DivisionID = DM.DivisionID
                        WHERE PTM.SchoolMaster_SchoolID = @SchoolID
                        ORDER BY DOW.DayOrder, PTM.PeriodTimeID";

                    using (SqlCommand timetableCmd = new SqlCommand(timetableQuery, conn))
                    {
                        timetableCmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        timetableCmd.Parameters.AddWithValue("@TeacherID", ddlTeacher.SelectedValue);

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
                                        TeacherTimetable.Rows.Add(currentRow);
                                    }

                                    currentRow = new TableRow();
                                    TableHeaderCell dayCell = new TableHeaderCell();
                                    dayCell.Text = dayOfWeek;
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
                                    string standardName = timetableReader["StandardName"].ToString();
                                    string divisionName = timetableReader["DivisionName"].ToString();

                                    if (!string.IsNullOrEmpty(subjectName))
                                    {
                                        periodCell.Text = $"{subjectName}";
                                        // Set the tooltip with proper HTML encoding and line break
                                        periodCell.Attributes["title"] = $"Standard: {standardName} | Division: {divisionName}";
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
                                TeacherTimetable.Rows.Add(currentRow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadTeacherTimetable: {ex.Message}");
                throw;
            }
        }
    }
}