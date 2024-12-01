using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Student
{
    public partial class AttendanceReport : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private int UserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve SchoolID and UserID from session
            SchoolID = Session["SchoolID"]?.ToString();
            UserID = Convert.ToInt32(Session["UserID"]);

            // Retrieve StudentID based on UserID and SchoolID
            StudentID = GetStudentIDFromSession(UserID, SchoolID);

            if (string.IsNullOrEmpty(StudentID))
            {
                Response.Redirect("~/Student/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Initially load attendance data
                LoadAttendanceData();

                lblNoRecordsFound.Visible = false;
            }
        }

        protected void ddlFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show or hide filters based on selection
            string filter = ddlFilterBy.SelectedValue;
            monthlyField.Visible = filter == "Monthly";
            customField.Visible = filter == "Custom";
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            // Load attendance data based on selected filters
            LoadAttendanceData();

           
        }

        private void LoadAttendanceData()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            string filter = ddlFilterBy.SelectedValue;

            switch (filter)
            {
                case "Monthly":
                    int month = ddlMonth.SelectedIndex + 1; // Get the month
                    startDate = new DateTime(DateTime.Now.Year, month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1); // Get last day of month
                    break;

                case "Custom":
                    if (!DateTime.TryParse(txtFromDate.Text, out startDate) || !DateTime.TryParse(txtToDate.Text, out endDate))
                    {
                        // Handle invalid date input
                        return;
                    }
                    break;
            }

            string query = @"
                SELECT 
                    Date,
                    Status
                FROM 
                    StudentAttendance
                WHERE 
                    StudentID = @StudentID 
                    AND SchoolMaster_SchoolID = @SchoolID 
                    AND Date BETWEEN @StartDate AND @EndDate
                ORDER BY 
                    Date";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", StudentID);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                DataTable dt = new DataTable();
                conn.Open();
                dt.Load(cmd.ExecuteReader());

                // Check if any data is returned
                if (dt.Rows.Count == 0)
                {
                    // No records found, hide the GridView and show the message
                    gvAttendanceReport.Visible = false;
                    lblNoRecordsFound.Visible = true;
                    lblNoRecordsFound.Text = "No attendance records found.";
                }
                else
                {
                    // Data found, bind it to the GridView
                    gvAttendanceReport.DataSource = dt;
                    gvAttendanceReport.DataBind();
                    gvAttendanceReport.Visible = true;
                    lblNoRecordsFound.Visible = false;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlFilterBy.SelectedIndex = 0;
            monthlyField.Visible = false;
            customField.Visible = false;
            ddlMonth.Visible = false;
            customField.Visible = false;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            gvAttendanceReport.Visible = false; // Hide the grid until data is loaded
            lblNoRecordsFound.Visible = false; // Hide the "No records found" message
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Student/Dashboard.aspx");
        }

        private string GetStudentIDFromSession(int userId, string schoolId)
        {
            string studentId = string.Empty;

            string query = @"
                SELECT StudentID 
                FROM StudentMaster 
                WHERE UserMaster_UserID = @UserID 
                AND SchoolMaster_SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                conn.Open();
                studentId = cmd.ExecuteScalar()?.ToString();
            }

            return studentId;
        }

        // RowDataBound event to format the date
        protected void gvAttendanceReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check if the row is a data row (not a header or footer row)
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the date value from the DataBinder
                DateTime dateValue;

                // Ensure that the date is parsed correctly and is valid
                if (DateTime.TryParse(DataBinder.Eval(e.Row.DataItem, "Date").ToString(), out dateValue))
                {
                    // Format the date as DD/MM/yyyy and set it to the cell
                    e.Row.Cells[0].Text = dateValue.ToString("dd/MM/yyyy");
                }
                else
                {
                    // In case the date is null or invalid, you can set a default value or leave it blank
                    e.Row.Cells[0].Text = "N/A";
                }

                string status = e.Row.Cells[1].Text.Trim().ToLower();

                if (status == "absent")
                {
                    e.Row.BackColor = System.Drawing.Color.Red;
                }
                else if (status == "present")
                {
                    e.Row.BackColor = System.Drawing.Color.Green;
                }

            }
        }
    }
}