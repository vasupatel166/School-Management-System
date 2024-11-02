using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class ViewAttendenceReports : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedMonth = int.Parse(ddlMonth.SelectedValue);
                int selectedYear = int.Parse(ddlYear.SelectedValue);

                // Logic to fetch attendance data based on selected month and year
                DataTable attendanceData = GetAttendanceData(selectedMonth, selectedYear);

                // Bind data to GridView
                gvAttendanceReport.DataSource = attendanceData;
                gvAttendanceReport.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private DataTable GetAttendanceData(int month, int year)
        {
            // Simulate fetching data from a database or other source
            DataTable dt = new DataTable();
            dt.Columns.Add("TeacherName");
            dt.Columns.Add("Date");
            dt.Columns.Add("Status");

            // Example data (replace with your data retrieval logic)
            dt.Rows.Add("Rajesh Sharma", new DateTime(year, 10, 25).ToShortDateString(), "Present");
            dt.Rows.Add("Ashish Chauhan", new DateTime(year, 10, 25).ToShortDateString(), "Absent");

            return dt;
        }

        //private void InitializeYears()
        //{
        //    for (int year = DateTime.Now.Year; year <= DateTime.Now.Year + 5; year++)
        //    {
        //        ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
        //    }
        //}
    }
}
