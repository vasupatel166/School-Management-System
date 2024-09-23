using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Schoolnest.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Session["SchoolId"] != null)
                //{
                //string schoolId = Session["SchoolId"].ToString();
                string schoolId = "Guru0012024".ToString();
                string schoolName = GetSchoolName(schoolId);
                    // Assuming you want to set the school name in a label or directly in the HTML
                    schoolNameLabel.Text = schoolName; // Update a label on the page
                                                       //}

                GetDashboardData(schoolId, out int totalStudents, out int totalTeachers, out int activeClasses, out decimal pendingFees);

                totalStudentsLabel.Text = totalStudents.ToString();
                totalTeachersLabel.Text = totalTeachers.ToString();
                activeClassesLabel.Text = activeClasses.ToString();
                pendingFeesLabel.Text = pendingFees.ToString("C"); // Formats as currency
            }
        }

        private string GetSchoolName(string schoolId)
        {
            string schoolName = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("GetSchoolName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolId", schoolId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        schoolName = reader["School_Name"].ToString();
                    }
                }
            }

            return schoolName;
        }

        private void GetDashboardData(string schoolId,out int totalStudents, out int totalTeachers, out int activeClasses, out decimal pendingFees)
        {
            totalStudents = totalTeachers = activeClasses = 0;
            pendingFees = 0;

            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("GetDashboardData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SchoolId", schoolId);
                    cmd.Parameters.Add(new SqlParameter("@TotalStudents", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@TotalTeachers", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@ActiveClasses", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@PendingFees", SqlDbType.Decimal) { Direction = ParameterDirection.Output });

                    cmd.ExecuteNonQuery();

                    totalStudents = (int)cmd.Parameters["@TotalStudents"].Value;
                    totalTeachers = (int)cmd.Parameters["@TotalTeachers"].Value;
                    activeClasses = (int)cmd.Parameters["@ActiveClasses"].Value;
                    object pendingFeesValue = cmd.Parameters["@PendingFees"].Value;
                    pendingFees = pendingFeesValue != DBNull.Value ? (decimal)pendingFeesValue : 0; // Default to 0 if NULL
                }
            }
        }

    }
}