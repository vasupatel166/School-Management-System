using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class ViewMyAttendance : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string TeacherID;
        private int UserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetTeacherID();

            if (!IsPostBack)
            {
                LoadAttendanceData("weekly");
            }
        }

        private void GetTeacherID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TeacherID FROM TeacherMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
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

        protected void ddlDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDuration = ddlDuration.SelectedValue;

            customDateDiv.Visible = selectedDuration == "custom";
            customDateToDiv.Visible = selectedDuration == "custom";
            searchDiv.Visible = selectedDuration == "custom";

            if (selectedDuration != "custom")
            {
                LoadAttendanceData(selectedDuration);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAttendanceData("custom");
        }

        private void LoadAttendanceData(string durationType)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            switch (durationType)
            {
                case "weekly":
                    startDate = DateTime.Now.AddDays(-7);
                    break;
                case "monthly":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "yearly":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
                case "custom":
                    if (DateTime.TryParse(txtFromDate.Text, out startDate) &&
                        DateTime.TryParse(txtToDate.Text, out endDate))
                    {
                        // Dates are valid
                    }
                    else
                    {
                        // Handle invalid dates
                        return;
                    }
                    break;
            }

            if (durationType != "custom")
            {
                endDate = DateTime.Now;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                        SELECT 
                            ROW_NUMBER() OVER(ORDER BY ta.Date DESC) as SrNo,
                            tm.TeacherName,
                            ta.Date,
                            ta.Status
                        FROM TeacherAttendance ta
                        INNER JOIN TeacherMaster tm ON ta.TeacherID = tm.TeacherID
                        WHERE ta.TeacherID = @TeacherID 
                        AND ta.SchoolMaster_SchoolID = @SchoolID
                        AND ta.Date BETWEEN @StartDate AND @EndDate
                        ORDER BY ta.Date DESC";

                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    DataTable dt = new DataTable();
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());

                    gvAttendance.DataSource = dt;
                    gvAttendance.DataBind();
                }
            }
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttendance.PageIndex = e.NewPageIndex;
            LoadAttendanceData(ddlDuration.SelectedValue);
        }
    }
}