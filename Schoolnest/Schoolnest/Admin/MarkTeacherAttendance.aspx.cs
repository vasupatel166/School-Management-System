using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class MarkTeacherAttendance : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["SchoolId"] != null)
                {
                    schoolId = Session["SchoolId"].ToString();
                }
                else
                {
                    Response.Write("<script>alert('Session expired. Please log in again.');</script>");
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            string selectedDate = txtDate.Text;
            string schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();
            if (!string.IsNullOrEmpty(selectedDate))
            {
                // Parse the selected date as it is (no time zone shifts)
                DateTime parsedDate;
                if (DateTime.TryParse(selectedDate, out parsedDate))
                {
                    LoadAttendanceData(parsedDate.ToString("yyyy-MM-dd"), schoolID); // Format as YYYY-MM-DD for consistency
                }
                else
                {
                    Response.Write("<script>alert('Invalid date format.');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a date to view attendance.');</script>");
            }
        }

        protected void LoadAttendanceData(string date, string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT t.TeacherID, t.TeacherName, 
                                ISNULL(a.Status, '') AS Status 
                         FROM TeacherMaster t
                         LEFT JOIN TeacherAttendance a 
                         ON t.TeacherID = a.TeacherID 
                         AND CAST(a.Date AS DATE) = @Date 
                         AND a.SchoolMaster_SchoolID = @SchoolID
                         WHERE t.SchoolMaster_SchoolID = @SchoolID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                cmd.Parameters.AddWithValue("@Date", date);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvTeacher.DataSource = dt;
                gvTeacher.DataBind();

                // Loop through each row to set the attendance status
                foreach (GridViewRow row in gvTeacher.Rows)
                {
                    DropDownList ddlAttendance = (DropDownList)row.FindControl("ddlAttendance");
                    ddlAttendance.SelectedValue = dt.Rows[row.RowIndex]["Status"].ToString();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string selectedDate = txtDate.Text;
            string schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (GridViewRow row in gvTeacher.Rows)
                {
                    HiddenField hfTeacherID = (HiddenField)row.FindControl("hfTeacherID");
                    string teacherID = hfTeacherID.Value;

                    // Get the selected attendance status
                    DropDownList ddlAttendance = (DropDownList)row.FindControl("ddlAttendance");
                    string status = ddlAttendance.SelectedValue;

                    // Check if the record exists for this teacher
                    string checkQuery = @"SELECT COUNT(*) 
                                  FROM TeacherAttendance 
                                  WHERE SchoolMaster_SchoolID = @SchoolID 
                                  AND TeacherID = @TeacherID 
                                  AND Date = @Date";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    checkCmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(selectedDate));
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Update the existing attendance record
                        string updateQuery = @"UPDATE TeacherAttendance
                                       SET Status = @Status
                                       WHERE SchoolMaster_SchoolID = @SchoolID
                                       AND TeacherID = @TeacherID
                                       AND Date = @Date";

                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@Status", status);
                        updateCmd.Parameters.AddWithValue("@SchoolID", schoolID);
                        updateCmd.Parameters.AddWithValue("@TeacherID", teacherID);
                        updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(selectedDate));
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Insert a new attendance record
                        string insertQuery = @"INSERT INTO TeacherAttendance (SchoolMaster_SchoolID, TeacherID, Date, Status)
                                       VALUES (@SchoolID, @TeacherID, @Date, @Status)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@SchoolID", schoolID);
                        insertCmd.Parameters.AddWithValue("@TeacherID", teacherID);
                        insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(selectedDate));
                        insertCmd.Parameters.AddWithValue("@Status", status);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            Response.Write("<script>alert('Attendance saved successfully!');</script>");
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Teacher/Dashboard.aspx");
        }
    }

   
}
