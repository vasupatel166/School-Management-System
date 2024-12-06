using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class AttendanceReports : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                BindDropdowns(schoolId);
                ToggleFilters();
            }
        }
        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide the GridView when the filter is changed
            gvAttendance.Visible = false;

            // Toggle filters for Standard and Division
            ToggleFilters();
        }

        private void ToggleFilters()
        {
            if (ddlFilter.SelectedValue == "Student")
            {
                studentFilters.Visible = true;
            }
            else
            {
                studentFilters.Visible = false;
            }
        }

        private void BindDropdowns(string schoolId)
        {
            // Bind Standards Dropdown
            DataTable standards = GetStandards(schoolId);
            ddlStandard.DataSource = standards;
            ddlStandard.DataTextField = "StandardName";
            ddlStandard.DataValueField = "StandardID";
            ddlStandard.DataBind();
            ddlStandard.Items.Insert(0, new ListItem("--Select Standard--", ""));

            DataTable divisions = GetDivisions(schoolId);
            ddlDivision.DataSource = divisions;
            ddlDivision.DataTextField = "DivisionName";
            ddlDivision.DataValueField = "DivisionID";
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0, new ListItem("--Select Division--", ""));
        }

        private DataTable GetDivisions(string schoolId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DivisionID, DivisionName FROM Divisions where SchoolMaster_SchoolID=@SchoolID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private DataTable GetStandards(string schoolId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT StandardID, StandardName FROM Standards where SchoolMaster_SchoolID=@SchoolID ";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            string filter = ddlFilter.SelectedValue;
            string schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();
            DateTime fromDate;
            DateTime toDate;

            if (!DateTime.TryParse(txtFromDate.Text, out fromDate) || !DateTime.TryParse(txtToDate.Text, out toDate))
            {
                // Handle invalid date inputs
                return;
            }

            DataTable dt = new DataTable();
            if (filter == "Student")
            {
                string standard = ddlStandard.SelectedValue;
                string division = ddlDivision.SelectedValue;

                dt = GetStudentAttendance(standard, division, fromDate, toDate, schoolID);
            }
            else if (filter == "Teacher")
            {
                dt = GetTeacherAttendance(fromDate, toDate, schoolID);
            }

            gvAttendance.DataSource = dt;
            gvAttendance.DataBind();

            gvAttendance.Visible = true;
        }

        private DataTable GetStudentAttendance(string standard, string division, DateTime fromDate, DateTime toDate, string schoolID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        SM.Student_FullName AS Name,
                        CONVERT(VARCHAR(10), SA.Date, 120) AS AttendanceDate, 
                        SA.Status AS AttendanceStatus
                    FROM 
                        StudentAttendance SA
                    JOIN 
                        StudentMaster SM ON SA.StudentID = SM.StudentID
                    WHERE 
                        SA.StandardID = @StandardID
                        AND SA.DivisionID = @DivisionID
                        AND SA.Date BETWEEN @FromDate AND @ToDate
                        AND SA.SchoolMaster_SchoolID = @SchoolID
                    ORDER BY 
                        SA.Date";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", standard);
                    cmd.Parameters.AddWithValue("@DivisionID", division);
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@SchoolID",schoolID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private DataTable GetTeacherAttendance(DateTime fromDate, DateTime toDate,string schoolID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        TM.TeacherName AS Name,
                        CONVERT(VARCHAR(10), TA.Date, 120) AS AttendanceDate, 
                        TA.Status AS AttendanceStatus
                    FROM 
                        TeacherAttendance TA
                    JOIN 
                        TeacherMaster TM ON TA.TeacherID = TM.TeacherID
                    WHERE 
                        TA.Date BETWEEN @FromDate AND @ToDate
                        AND TA.SchoolMaster_SchoolID = @SchoolID
                    ORDER BY 
                        TM.TeacherName, TA.Date";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

    }
}