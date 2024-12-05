using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Student
{
    public partial class UpcomingHolidays : System.Web.UI.Page
    {

        string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();

            if (!IsPostBack)
            {
                BindAcademicYearDropdown();
                LoadHolidays();
            }
        }

        private void BindAcademicYearDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT DISTINCT AcademicYear 
                         FROM HolidayMaster 
                         WHERE SchoolMaster_SchoolID = @SchoolID
                         ORDER BY AcademicYear DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlAcademicYear.Items.Clear();
                        ddlAcademicYear.Items.Add(new ListItem("All", ""));
                        while (reader.Read())
                        {
                            ddlAcademicYear.Items.Add(new ListItem(reader["AcademicYear"].ToString(), reader["AcademicYear"].ToString()));
                        }
                    }
                }
            }
        }


        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHolidays(ddlAcademicYear.SelectedValue);
        }

        private void LoadHolidays(string academicYear = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                StringBuilder queryBuilder = new StringBuilder(@"SELECT HolidayName, HolidayDate 
                             FROM HolidayMaster 
                             WHERE SchoolMaster_SchoolID = @SchoolID");

                if (!string.IsNullOrEmpty(academicYear) && academicYear != "")
                {
                    queryBuilder.Append(" AND AcademicYear = @AcademicYear");
                }

                queryBuilder.Append(" ORDER BY HolidayDate ASC");

                using (SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    if (!string.IsNullOrEmpty(academicYear) && academicYear != "")
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                    }

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var holidaysHtml = new StringBuilder();
                        int index = 1;
                        while (reader.Read())
                        {
                            var holidayDate = Convert.ToDateTime(reader["HolidayDate"]).ToString("MMM dd, yyyy");

                            holidaysHtml.Append($@"
                                <tr>
                                    <td>{index++}</td>
                                    <td>{reader["HolidayName"]}</td>
                                    <td>{holidayDate}</td>
                                </tr>");
                        }

                        if (holidaysHtml.Length == 0)
                        {
                            holidaysHtml.Append("<tr><td colspan='3' class='text-center'>No holidays available.</td></tr>");
                        }

                        HolidaysTableBody.InnerHtml = holidaysHtml.ToString();
                    }
                }
            }
        }
    }
}