using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using ListItem = System.Web.UI.WebControls.ListItem;
using System.Threading;

namespace Schoolnest.Admin
{
    public partial class TimetableMaster : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadAssignedStandard();
            }

        }

        private void LoadAssignedStandard()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Standards_StandardID, s.StandardName FROM SubjectDetail AS sd INNER JOIN Standards AS s ON sd.Standards_StandardID = s.StandardID
                    WHERE sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStandard.Items.Clear();
                    ddlStandard.Items.Add(new ListItem("Select Standard", "0"));
                    while (reader.Read())
                    {
                        ddlStandard.Items.Add(new ListItem(reader["StandardName"].ToString(), reader["Standards_StandardID"].ToString()));
                    }
                }
            }
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStandard.SelectedValue != "0")
            {
                LoadAssignedDivisions();
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.SelectedIndex = 0;
                ddlDivision.Enabled = false;
            }

            ClearSubjectsGrid();
        }

        private void LoadAssignedDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Divisions_DivisionID, d.DivisionName FROM SubjectDetail AS sd INNER JOIN Divisions AS d ON sd.Divisions_DivisionID = d.DivisionID
                    WHERE sd.Standards_StandardID = @StandardID AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlDivision.Items.Clear();
                    ddlDivision.Items.Add(new ListItem("Select Division", "0"));
                    while (reader.Read())
                    {
                        ddlDivision.Items.Add(new ListItem(reader["DivisionName"].ToString(), reader["Divisions_DivisionID"].ToString()));
                    }
                }
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDivision.SelectedValue != "0")
            {
                LoadSubjectsGrid();
                LoadClassTimetable();
            }
            else
            {
                ClearSubjectsGrid();
                ClearSubjectsGrid();
                ClassTimetable.Rows.Clear();
            }
        }

        private string FormatPeriodTime(string periodTime)
        {
            try
            {
                string startTime = periodTime.Split('-')[0].Trim();
                DateTime time = DateTime.Parse(startTime);
                return time.ToString("h:mm tt");
            }
            catch
            {
                return periodTime;
            }
        }

        private void LoadSubjectsGrid()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT sd.SubjectDetailID, sm.SubjectName, ISNULL(t.TeacherName, 'Not Assigned') AS TeacherName FROM SubjectDetail sd
                INNER JOIN SubjectMaster sm ON sd.SubjectMaster_SubjectID = sm.SubjectID
                LEFT JOIN TeacherMaster t ON sd.Teachers_TeacherID = t.TeacherID
                WHERE sd.Standards_StandardID = @StandardID AND sd.Divisions_DivisionID = @DivisionID AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    gvSubjects.DataSource = reader;
                    gvSubjects.DataBind();
                }
            }
        }

        protected void gvSubjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string teacherName = DataBinder.Eval(e.Row.DataItem, "TeacherName").ToString();
                if (teacherName == "Not Assigned")
                {
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SetTimetable")
            {
                // Retrieve SubjectDetailID from the CommandArgument
                ViewState["SubjectDetailID"] = e.CommandArgument.ToString();

                LoadTimetableModal();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "$(document).ready(function() { $('#timetableModal').modal('show'); });", true);
            }
        }

        private void LoadTimetableModal()
        {
            LoadPeriodTimes(ddlMonday, "Monday");
            LoadPeriodTimes(ddlTuesday, "Tuesday");
            LoadPeriodTimes(ddlWednesday, "Wednesday");
            LoadPeriodTimes(ddlThursday, "Thursday");
            LoadPeriodTimes(ddlFriday, "Friday");
            LoadPeriodTimes(ddlSaturday, "Saturday");
        }

        private void LoadPeriodTimes(DropDownList ddlDay, string DayOfWeek)
        {
            string SubjectDetailID = ViewState["SubjectDetailID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
        SELECT 
            pt.PeriodTimeID, 
            pt.PeriodTime, 
            CASE 
                WHEN tm.PeriodTimeID IS NOT NULL AND tm.SubjectDetail_SubjectDetailID = @SubjectDetailID THEN 1 
                WHEN tm.PeriodTimeID IS NOT NULL AND tm.SubjectDetail_SubjectDetailID != @SubjectDetailID THEN 2
                ELSE 0 
            END AS AssignmentStatus
        FROM PeriodTimeMaster pt
        LEFT JOIN TimeTableMaster tm 
            ON pt.PeriodTimeID = tm.PeriodTimeID 
            AND tm.DayOfWeek = @DayOfWeek
        WHERE pt.SchoolMaster_SchoolID = @SchoolID 
        AND pt.IsBreakTime = 0", conn))
                {
                    cmd.Parameters.AddWithValue("@DayOfWeek", DayOfWeek);
                    cmd.Parameters.AddWithValue("@SubjectDetailID", SubjectDetailID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlDay.Items.Clear();
                    ddlDay.Items.Add(new ListItem("Select Period", "0"));
                    while (reader.Read())
                    {
                        string periodText = reader["PeriodTime"].ToString();
                        string periodValue = reader["PeriodTimeID"].ToString();
                        int assignmentStatus = Convert.ToInt32(reader["AssignmentStatus"]);

                        if (assignmentStatus == 1)
                        {
                            periodText += " (Assigned)";
                            ListItem listItem = new ListItem(periodText, periodValue)
                            {
                                Selected = true
                            };
                            ddlDay.Items.Add(listItem);
                        }
                        else if (assignmentStatus == 0)
                        {
                            // Period is available
                            ddlDay.Items.Add(new ListItem(periodText, periodValue));
                        }
                        // Skip adding if status is 2 (Assigned to another SubjectDetailID)
                    }
                }
            }
        }

        protected void btnSaveTimetable_Click(object sender, EventArgs e)
        {

            string SubjectDetailID = ViewState["SubjectDetailID"]?.ToString();

            List<(string dayOfWeek, string periodTimeID)> timetableEntries = new List<(string, string)>
            {
                ("Monday", ddlMonday.SelectedValue),
                ("Tuesday", ddlTuesday.SelectedValue),
                ("Wednesday", ddlWednesday.SelectedValue),
                ("Thursday", ddlThursday.SelectedValue),
                ("Friday", ddlFriday.SelectedValue),
                ("Saturday", ddlSaturday.SelectedValue)
            };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (var entry in timetableEntries)
                {
                    if (entry.periodTimeID != "0") // Ensure a valid period time is selected
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
                        IF EXISTS (SELECT 1 FROM TimeTableMaster 
                                   WHERE SubjectDetail_SubjectDetailID = @SubjectDetailID 
                                   AND DayOfWeek = @DayOfWeek)
                        BEGIN
                            UPDATE TimeTableMaster
                            SET PeriodTimeID = @PeriodTimeID
                            WHERE SubjectDetail_SubjectDetailID = @SubjectDetailID AND DayOfWeek = @DayOfWeek
                        END
                        ELSE
                        BEGIN
                            INSERT INTO TimeTableMaster (DayOfWeek, PeriodTimeID, SubjectDetail_SubjectDetailID, SchoolMaster_SchoolID)
                            VALUES (@DayOfWeek, @PeriodTimeID, @SubjectDetailID, @SchoolID)
                        END", conn))
                        {
                            cmd.Parameters.AddWithValue("@DayOfWeek", entry.dayOfWeek);
                            cmd.Parameters.AddWithValue("@PeriodTimeID", entry.periodTimeID);
                            cmd.Parameters.AddWithValue("@SubjectDetailID", SubjectDetailID);
                            cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            LoadClassTimetable();

            // Close the modal using JavaScript
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModal", "closeModal();", true);
        }

        private void LoadClassTimetable()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    ClassTimetable.Rows.Clear();
                    TableHeaderRow headerRow = new TableHeaderRow();
                    headerRow.Cells.Add(new TableHeaderCell { Text = "Day" });

                    // Get period times
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

                    ClassTimetable.Rows.Add(headerRow);

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
                        timetableCmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                        timetableCmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);

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
                                        ClassTimetable.Rows.Add(currentRow);
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
                                    string teacherName = timetableReader["TeacherName"].ToString();

                                    if (!string.IsNullOrEmpty(subjectName))
                                    {
                                        periodCell.Text = subjectName;
                                        periodCell.Attributes["title"] = $"Teacher: {teacherName}";
                                        periodCell.CssClass = "assigned-period";
                                    }
                                    else
                                    {
                                        periodCell.Text = "-";
                                        periodCell.CssClass = "not-assigned-period";
                                    }
                                }

                                currentRow.Cells.Add(periodCell);
                            }

                            if (currentRow != null)
                            {
                                ClassTimetable.Rows.Add(currentRow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadClassTimetable: {ex.Message}");
                throw;
            }
        }

        private void ClearSubjectsGrid()
        {
            gvSubjects.DataSource = null;
            gvSubjects.DataBind();
        }
    }
}