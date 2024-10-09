using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Schoolnest.Admin
{
    public partial class Dashboard : System.Web.UI.Page
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
                GetDefaulters(schoolId);

                totalStudentsLabel.Text = totalStudents.ToString();
                totalTeachersLabel.Text = totalTeachers.ToString();
                activeClassesLabel.Text = activeClasses.ToString();
                pendingFeesLabel.Text = pendingFees.ToString("C"); // Formats as currency

                LoadUpcomingEvents("2024-2025", schoolId);
                
            }
        }

        private void GetDefaulters(string schoolId)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
                    
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spG_tDefaulters", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        RepeaterDefaulters.DataSource = dt;
                        RepeaterDefaulters.DataBind();
                    }
                }
            }
                }

        [WebMethod]
        public static string GetAttendanceData()
        {
            // Retrieve SchoolID from session
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            string jsonData = string.Empty;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetWeeklyAttendanceSummary", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    jsonData = JsonConvert.SerializeObject(dt);
                }
            }
            return jsonData;
        }

        [WebMethod]
        public static string GetBudgetData()
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            List<BudgetCategory> budgetDataList = new List<BudgetCategory>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetBudgetDataBySchoolID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    BudgetCategory budgetData = new BudgetCategory
                    {
                        CategoryName = reader["CategoryName"].ToString(),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        ActualExpenditure = Convert.ToDecimal(reader["ActualExpenditure"])
                    };
                    budgetDataList.Add(budgetData);
                }
            }

            return JsonConvert.SerializeObject(budgetDataList);
        }

        private void LoadUpcomingEvents(string academicYear,string schoolId)
        {
            string connString = ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("GetUpcomingEventsForCurrentYear", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        UpcomingEventsRepeater.DataSource = reader;
                        UpcomingEventsRepeater.DataBind();
                    }
                    else
                    {
                        NoEventsLabel.Text = "No upcoming events for the current academic year.";
                    }
                }
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
                        schoolName = reader["SchoolName"].ToString();
                    }
                }
            }

            return schoolName;
        }

        private void GetDashboardData(string schoolId, out int totalStudents, out int totalTeachers, out int activeClasses, out decimal pendingFees)
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