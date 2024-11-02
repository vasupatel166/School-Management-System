using System;
using System.Data;
using System.Web.UI;

namespace Schoolnest.Admin
{
    public partial class MarkAttendance : Page
    {
        private DataTable attendanceData; // Store attendance data in a DataTable

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the DataTable to store attendance records
                attendanceData = new DataTable();
                attendanceData.Columns.Add("TeacherName");
                attendanceData.Columns.Add("Date");
                attendanceData.Columns.Add("Status");

                // Store DataTable in ViewState for persistence across postbacks
                ViewState["AttendanceData"] = attendanceData;
            }
            else
            {
                // Retrieve DataTable from ViewState
                attendanceData = (DataTable)ViewState["AttendanceData"];
            }
        }

        protected void btnPresent_Click(object sender, EventArgs e)
        {
            RecordAttendance("Present");
        }

        protected void btnAbsent_Click(object sender, EventArgs e)
        {
            RecordAttendance("Absent");
        }


        private void RecordAttendance(string status)
        {
            // Get the selected teacher and date
            string selectedTeacher = ddlTeacher.SelectedValue;
            string selectedDate = txtDate.Text;

            if (!string.IsNullOrEmpty(selectedTeacher) && !string.IsNullOrEmpty(selectedDate))
            {
                // Add the attendance record to the DataTable
                attendanceData.Rows.Add(selectedTeacher, selectedDate, status);

                // Update the ViewState with the new DataTable
                ViewState["AttendanceData"] = attendanceData;

                // Bind the GridView to display updated attendance records
                gvAttendanceReport.DataSource = attendanceData;
                gvAttendanceReport.DataBind();

                // Optionally clear the selections
                ddlTeacher.SelectedIndex = 0;
                txtDate.Text = string.Empty;
            }
            else
            {
                // Optionally show a message if inputs are not valid
                // lblError.Text = "Please select a teacher and a date.";
                // lblError.Visible = true;
            }
        }
    }
}
