using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Schoolnest.Student
{
    public partial class FeePaymentHistory : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                lblStandardID.Visible = false;
                lblDivisionID.Visible = false;
                LoadStudentDetails(StudentID, SchoolID);
                BindFeePaymentHistory(StudentID, SchoolID, "2024-2025");  // Example Academic Year
            }
        }

        private void LoadStudentDetails(string studentID, string schoolID)
        {
            string studentDetailsQuery = @"
                SELECT 
                    SM.[Student_Standard] AS [StandardID],
                    SD.StandardName AS [StandardName],
                    SM.[Student_Division] AS [DivisionID],
                    DV.DivisionName AS [DivisionName]
                FROM StudentMaster SM 
                LEFT JOIN Standards SD ON SD.StandardID = SM.Student_Standard
                LEFT JOIN Divisions DV ON DV.DivisionID = SM.Student_Division
                WHERE StudentID = @studentID AND SM.SchoolMaster_SchoolID = @schoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(studentDetailsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    cmd.Parameters.AddWithValue("@StudentID", studentID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set student details for Standard and Division
                            lblStandardID.Text = reader["StandardID"].ToString();
                            lblDivisionID.Text = reader["DivisionID"].ToString();
                            lblStandard.Text = reader["StandardName"].ToString();
                            lblDivision.Text = reader["DivisionName"].ToString();
                        }
                    }
                }
            }
        }

        private string GetStudentIDFromSession(int userID, string schoolID)
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
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                conn.Open();
                studentId = cmd.ExecuteScalar()?.ToString();
            }

            return studentId;
        }

        // Method to bind Fee Payment History data to GridView
        private void BindFeePaymentHistory(string studentID, string schoolID, string academicYear)
        {
            string feePaymentQuery = @"
                SELECT DISTINCT FR.FeeRecordID, 
                       FR.FeeDetail_FeeDetailID, 
                       FR.ReceiptNumber, 
                       FR.TermType, 
                       FR.PaidAmount, 
                       FR.AmountInWords, 
                       FR.PaymentDate, 
                       FR.SchoolMaster_SchoolID
                FROM FeeRecords AS FR 
                LEFT JOIN FeeDetail AS FD 
                    ON FD.FeeDetailID = FR.FeeDetail_FeeDetailID
                WHERE FD.StudentMaster_StudentID = @StudentID 
                    AND FD.PaymentStatus = 1 
                    AND FD.AcademicYear = @AcademicYear 
                    AND FD.SchoolMaster_SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(feePaymentQuery, conn))
                {
                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Load the data into a DataTable
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            // Bind the data to the GridView (assuming GridView ID is gvFeePaymentHistory)
                            gvFeePaymentHistory.DataSource = dt;
                            gvFeePaymentHistory.DataBind();
                        }
                        else
                        {
                            // If no rows are returned, you can optionally show a message
                            // Example: lblNoDataMessage.Visible = true;
                        }
                    }
                }
            }
        }
    }
}
