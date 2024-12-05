using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace Schoolnest.Student
{
    public partial class FeePaymentHistory : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private string AcademicYear;
        private int UserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetAcademicYear();
            StudentID = GetStudentIDFromSession(UserID, SchoolID);

            if (!IsPostBack)
            {
                LoadStudentDetails(StudentID, SchoolID);
                BindFeePaymentHistory(StudentID, SchoolID, AcademicYear);
            }
        }

        private void GetAcademicYear()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT AcademicYear FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AcademicYear = reader["AcademicYear"]?.ToString();
                        }
                    }
                }
            }
        }

        private void LoadStudentDetails(string studentID, string schoolID)
        {
            string studentDetailsQuery = @"
                SELECT 
                    SM.[Student_Standard] AS [StandardID],
                    SD.StandardDesc AS [StandardName],
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

                            gvFeePaymentHistory.DataSource = dt;
                            gvFeePaymentHistory.DataBind();
                        }
                        
                    }
                }
            }
        }
    }
}
