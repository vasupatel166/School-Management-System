using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class MarkAttendance : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        string Username = string.Empty;
        int standard, division;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                Username = Session["Username"].ToString();
                LoadTeacherInfo(schoolId, Username);

                string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
                txtDate.Text = todayDate;
                hfSelectedDate.Value = todayDate;

                LoadAttendanceData(todayDate);
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            string selectedDate = txtDate.Text;
            hfSelectedDate.Value = selectedDate;

            // Load attendance data for the selected date
            LoadAttendanceData(selectedDate);
        }

        private void LoadTeacherInfo(string schoolId, string Username)
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
                         WHERE AT.TeacherID = @TeacherID AND AT.SchoolID = @SchoolID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", Username);
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

                        standard = Convert.ToInt32(hfStandardID.Value);
                        division = Convert.ToInt32(hfDivisionID.Value);
                    }
                }
                reader.Close();
            }
        }

        private void LoadAttendanceData(string attendanceDate)
        {
            int standardID = Convert.ToInt32(hfStandardID.Value);
            int divisionID = Convert.ToInt32(hfDivisionID.Value);
            string schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT SM.Student_FullName, 
                                        COALESCE(SA.Status, 'Absent') AS Status 
                                 FROM StudentMaster SM
                                 LEFT JOIN StudentAttendance SA ON SA.StudentName = SM.Student_FullName
                                 AND SA.StandardID = @StandardID AND SA.DivisionID = @DivisionID
                                 AND SA.Date = @Date
                                 WHERE SM.Student_Standard = @StandardID 
                                 AND SM.Student_Division = @DivisionID
                                 AND SM.SchoolMaster_SchoolID = @SchoolID
                                 ORDER BY SM.Student_LastName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StandardID", standardID);
                cmd.Parameters.AddWithValue("@DivisionID", divisionID);
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                cmd.Parameters.AddWithValue("@Date", attendanceDate);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvStudents.DataSource = dt;
                gvStudents.DataBind();

                // Set the selected value for each student's attendance status
                foreach (GridViewRow row in gvStudents.Rows)
                {
                    DropDownList ddlAttendance = (DropDownList)row.FindControl("ddlAttendance");
                    DataRow dataRow = dt.Rows[row.RowIndex];
                    ddlAttendance.SelectedValue = dataRow["Status"].ToString();
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
            string schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (GridViewRow row in gvStudents.Rows)
                {
                    string studentName = row.Cells[0].Text;
                    DropDownList ddlAttendance = (DropDownList)row.FindControl("ddlAttendance");
                    string attendanceStatus = ddlAttendance.SelectedValue; // Get the selected value of the dropdown

                    string checkQuery = @"SELECT COUNT(1) FROM StudentAttendance 
                                          WHERE StudentName = @StudentName 
                                          AND StandardID = @StandardID 
                                          AND DivisionID = @DivisionID 
                                          AND Date = @Date";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@StudentName", studentName);
                    checkCmd.Parameters.AddWithValue("@StandardID", standardID);
                    checkCmd.Parameters.AddWithValue("@DivisionID", divisionID);
                    checkCmd.Parameters.AddWithValue("@Date", attendanceDate);

                    int recordExists = (int)checkCmd.ExecuteScalar();

                    if (recordExists > 0)
                    {
                        string updateQuery = @"UPDATE StudentAttendance 
                                               SET Status = @Status 
                                               WHERE StudentName = @StudentName 
                                               AND StandardID = @StandardID 
                                               AND DivisionID = @DivisionID 
                                               AND Date = @Date";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@Status", attendanceStatus);
                        updateCmd.Parameters.AddWithValue("@StudentName", studentName);
                        updateCmd.Parameters.AddWithValue("@StandardID", standardID);
                        updateCmd.Parameters.AddWithValue("@DivisionID", divisionID);
                        updateCmd.Parameters.AddWithValue("@Date", attendanceDate);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertQuery = @"INSERT INTO StudentAttendance 
                                               (SchoolMaster_SchoolID, StudentName, StandardID, DivisionID, Date, Status)
                                               VALUES (@SchoolID, @StudentName, @StandardID, @DivisionID, @Date, @Status)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@SchoolID", schoolID);
                        insertCmd.Parameters.AddWithValue("@StudentName", studentName);
                        insertCmd.Parameters.AddWithValue("@StandardID", standardID);
                        insertCmd.Parameters.AddWithValue("@DivisionID", divisionID);
                        insertCmd.Parameters.AddWithValue("@Date", attendanceDate);
                        insertCmd.Parameters.AddWithValue("@Status", attendanceStatus);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
