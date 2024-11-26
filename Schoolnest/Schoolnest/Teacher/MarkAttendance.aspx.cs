using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class MarkAttendance : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        private int UserID;
        private string TeacherID;

        protected void Page_Load(object sender, EventArgs e)
        {
            schoolId = Session["SchoolId"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);

            if (!IsPostBack)
            {
                GetTeacherID();
                LoadTeacherInfo();

                // Only set today's date if it's the first load
                string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
                txtDate.Text = todayDate;
                hfSelectedDate.Value = todayDate;

                LoadAttendanceData(todayDate);
            }
            else
            {
                // On postback, use the date from the textbox
                string selectedDate = txtDate.Text;
                hfSelectedDate.Value = selectedDate;
            }
        }

        private void GetTeacherID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TeacherID FROM TeacherMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TeacherID = reader["TeacherID"]?.ToString();
                        }
                    }
                }
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            string selectedDate = txtDate.Text;
            hfSelectedDate.Value = selectedDate;

            // Load attendance data for the selected date
            LoadAttendanceData(selectedDate);
        }

        private void LoadTeacherInfo()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TM.TeacherName AS TeacherName,
                                SD.StandardName AS [Standard],
                                DV.DivisionName AS [Division],
                                AT.StandardID,
                                AT.DivisionID
                         FROM AssignTeacher AT 
                         LEFT JOIN TeacherMaster TM ON TM.TeacherID = AT.TeacherID
                         LEFT JOIN Standards SD ON SD.StandardID = AT.StandardID
                         LEFT JOIN Divisions DV ON DV.DivisionID = AT.DivisionID
                         WHERE AT.TeacherID = @TeacherID AND AT.SchoolMaster_SchoolID = @SchoolID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        lblTeacher.Text = reader["TeacherName"].ToString();
                        lblStd.Text = reader["Standard"].ToString();
                        lblDivision.Text = reader["Division"].ToString();
                        hfStandardID.Value = reader["StandardID"].ToString();
                        hfDivisionID.Value = reader["DivisionID"].ToString();
                    }
                }
                reader.Close();
            }
        }

        private void LoadAttendanceData(string attendanceDate)
        {
            int standardID = Convert.ToInt32(hfStandardID.Value);
            int divisionID = Convert.ToInt32(hfDivisionID.Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    SM.StudentID, 
                    SM.Student_FullName,
                    ISNULL(SA.Status, 'Present') AS Status 
                FROM StudentMaster SM
                LEFT JOIN (
                    SELECT StudentID, Status 
                    FROM StudentAttendance 
                    WHERE StandardID = @StandardID 
                    AND DivisionID = @DivisionID 
                    AND Date = @Date
                    AND SchoolMaster_SchoolID = @SchoolID
                ) SA ON SM.StudentID = SA.StudentID
                WHERE SM.Student_Standard = @StandardID 
                AND SM.Student_Division = @DivisionID
                AND SM.SchoolMaster_SchoolID = @SchoolID
                ORDER BY SM.Student_FullName";

                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@StandardID", standardID);
                    cmd.Parameters.AddWithValue("@DivisionID", divisionID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@Date", attendanceDate);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Store the DataTable in ViewState for later use
                    ViewState["AttendanceData"] = dt;

                    gvStudents.DataSource = dt;
                    gvStudents.DataBind();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading attendance data: {ex.Message}");
                    throw;
                }
            }
        }

        protected void gvStudents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the DataTable from ViewState
                DataTable dt = ViewState["AttendanceData"] as DataTable;
                if (dt != null)
                {
                    // Find the dropdown and set its value
                    DropDownList ddlAttendance = (DropDownList)e.Row.FindControl("ddlAttendance");
                    if (ddlAttendance != null)
                    {
                        string status = dt.Rows[e.Row.RowIndex]["Status"].ToString();
                        ddlAttendance.SelectedValue = status;
                    }

                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Teacher/Dashboard.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string attendanceDate = hfSelectedDate.Value;
            int standardID = Convert.ToInt32(hfStandardID.Value);
            int divisionID = Convert.ToInt32(hfDivisionID.Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (GridViewRow row in gvStudents.Rows)
                {
                    try
                    {
                        HiddenField hfStudentID = (HiddenField)row.FindControl("hfStudentID");
                        DropDownList ddlAttendance = (DropDownList)row.FindControl("ddlAttendance");

                        if (hfStudentID != null && ddlAttendance != null)
                        {
                            string studentID = hfStudentID.Value;
                            string attendanceStatus = ddlAttendance.SelectedValue;

                            // Debug line
                            System.Diagnostics.Debug.WriteLine($"Processing StudentID: {studentID}, Status: {attendanceStatus}");

                            string checkQuery = @"SELECT COUNT(1) FROM StudentAttendance 
                                          WHERE StudentID = @StudentID 
                                          AND StandardID = @StandardID 
                                          AND DivisionID = @DivisionID 
                                          AND Date = @Date
                                          AND SchoolMaster_SchoolID = @SchoolID";

                            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                            checkCmd.Parameters.AddWithValue("@StudentID", studentID);
                            checkCmd.Parameters.AddWithValue("@StandardID", standardID);
                            checkCmd.Parameters.AddWithValue("@DivisionID", divisionID);
                            checkCmd.Parameters.AddWithValue("@Date", attendanceDate);
                            checkCmd.Parameters.AddWithValue("@SchoolID", schoolId);

                            int recordExists = (int)checkCmd.ExecuteScalar();

                            if (recordExists > 0)
                            {
                                string updateQuery = @"UPDATE StudentAttendance 
                                               SET Status = @Status 
                                               WHERE StudentID = @StudentID 
                                               AND StandardID = @StandardID 
                                               AND DivisionID = @DivisionID 
                                               AND Date = @Date
                                               AND SchoolMaster_SchoolID = @SchoolID";
                                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                                updateCmd.Parameters.AddWithValue("@StudentID", studentID);
                                updateCmd.Parameters.AddWithValue("@StandardID", standardID);
                                updateCmd.Parameters.AddWithValue("@DivisionID", divisionID);
                                updateCmd.Parameters.AddWithValue("@Date", attendanceDate);
                                updateCmd.Parameters.AddWithValue("@Status", attendanceStatus);
                                updateCmd.Parameters.AddWithValue("@SchoolID", schoolId);
                                updateCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                string insertQuery = @"INSERT INTO StudentAttendance 
                                               (StudentID, StandardID, DivisionID, Date, Status, SchoolMaster_SchoolID)
                                               VALUES (@StudentID, @StandardID, @DivisionID, @Date, @Status, @SchoolID)";
                                SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                                insertCmd.Parameters.AddWithValue("@StudentID", studentID);
                                insertCmd.Parameters.AddWithValue("@StandardID", standardID);
                                insertCmd.Parameters.AddWithValue("@DivisionID", divisionID);
                                insertCmd.Parameters.AddWithValue("@Date", attendanceDate);
                                insertCmd.Parameters.AddWithValue("@Status", attendanceStatus);
                                insertCmd.Parameters.AddWithValue("@SchoolID", schoolId);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Row {row.RowIndex}: StudentID HiddenField or DropDownList not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing row {row.RowIndex}: {ex.Message}");
                        throw;
                    }
                }
            }
        }
    }
}
