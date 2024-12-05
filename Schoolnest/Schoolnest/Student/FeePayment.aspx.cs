using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace Schoolnest.Student
{
    public partial class FeePayment : System.Web.UI.Page
    {
        private string SchoolName;
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private string StudentName;
        private int StandardID;
        private int DivisionID;
        private string AcademicYear;
        private string FirstTermFeeDueDate;
        private string SecondTermFeeDueDate;
        private int LateFeeCharges;
        private int UserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"];
            SchoolID = Session["SchoolID"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetStudentDetails();
            GetSchoolName();
            GetSchoolSettings();


            if (!IsPostBack)
            {
                BindTermOptions();
                string paymentStatus = Request.QueryString["status"];
                string sessionId = Request.QueryString["session_id"];

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    if (paymentStatus == "success")
                    {

                        string paymentReference = ""; ;
                        string paymentMethod = "";


                        if (!string.IsNullOrEmpty(sessionId))
                        {
                            (string paymentIntentId, string paymentMethodType) = RetrieveSessionDetails(sessionId);

                            paymentReference = paymentIntentId;
                            paymentMethod = paymentMethodType;
                        }

                        string paymentDataJson = Session["PaymentDataJson"] as string;

                        if (!string.IsNullOrEmpty(paymentDataJson))
                        {
                            var paymentData = JsonConvert.DeserializeObject<PaymentDetailModel>(paymentDataJson);

                            InsertIntoFeeDetail(paymentReference, paymentMethod, paymentData);

                            string paymentAmount = Request.QueryString["amount"];

                            decimal amount = 0;
                            if (decimal.TryParse(paymentAmount, out amount))
                            {
                                paymentAmount = string.Format("₹ {0:N0}", amount);
                            }

                            string redirectUrl = "/Student/FeePayment.aspx";

                            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", $"Swal.fire('Success!', 'Payment of {paymentAmount} processed successfully.', 'success').then(function() {{ window.location.href = '{redirectUrl}'; }});", true);
                        }
                    }
                    else if (paymentStatus == "cancel")
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", "Swal.fire('Payment Cancelled', 'Your payment was cancelled.', 'warning');", true);
                    }
                }
            }
        }

        public (string paymentIntentId, string paymentMethodType) RetrieveSessionDetails(string sessionId)
        {
            try
            {
                // Set your Stripe API key
                StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"];

                // Retrieve the session details from Stripe
                var sessionService = new SessionService();
                Session session = sessionService.Get(sessionId);

                // Extract the payment_intent and payment_method_type from the session
                string paymentIntentId = session.PaymentIntentId;
                string paymentMethodType = session.PaymentMethodTypes != null && session.PaymentMethodTypes.Count > 0
                    ? session.PaymentMethodTypes[0]
                    : string.Empty;

                // Serialize the entire session object to JSON for debugging purposes
                string sessionDetailsJson = JsonConvert.SerializeObject(session, Formatting.Indented);

                // Log the session details (you can log this to a file, console, or debug output)
                Debug.WriteLine("Full Session Details:\n" + sessionDetailsJson);

                // Return a tuple containing both values
                return (paymentIntentId, paymentMethodType);
            }
            catch (StripeException stripeEx)
            {
                lblError.Text = $"Error retrieving session details: {stripeEx.Message}";
                return (null, null); // or handle error differently
            }
            catch (Exception ex)
            {
                lblError.Text = $"An unexpected error occurred: {ex.Message}";
                return (null, null); // or handle error differently
            }
        }

        public class PaymentDetailModel
        {
            public string FeeDetailID { get; set; }
            public string StudentID { get; set; }
            public string StandardID { get; set; }
            public string DivisionID { get; set; }
            public string SchoolID { get; set; }
            public string AcademicYear { get; set; }
            public string TermType { get; set; }
            public List<FeeItemModel> Fees { get; set; }
        }

        public class FeeItemModel
        {
            public int? FeeMasterID { get; set; }
            public string FeeName { get; set; }
            public decimal FeeAmount { get; set; }
            public decimal TotalFeeAmount { get; set; }
            public decimal PayAmount { get; set; }
            public decimal BalanceAmount { get; set; }
            public bool LateFee { get; set; }
        }

        private string GetPaymentDataJson()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999); // Generates a 5-digit random number

            var feeDetailData = new
            {
                FeeDetailID = $"FEE{StudentID.Substring(0, 6).ToUpper()}{StandardID}{DivisionID}{randomNumber}",
                StudentID,
                StandardID,
                DivisionID,
                SchoolID,
                AcademicYear,
                TermType = ddlTerm.SelectedValue,
                Fees = new List<object>()
            };

            foreach (GridViewRow row in gvFeeMaster.Rows)
            {
                // Skip the last row (Total row)
                if (row.Cells[1].Text == "Total")
                    continue;

                HiddenField hfFeeMasterID = (HiddenField)row.FindControl("hfFeeMasterID");
                int feeMasterID = 0;
                if (hfFeeMasterID != null && !string.IsNullOrEmpty(hfFeeMasterID.Value) && !int.TryParse(hfFeeMasterID.Value, out feeMasterID))
                {
                    Debug.WriteLine($"Invalid FeeMasterID: {hfFeeMasterID.Value}"); 
                }

                string feeName = row.Cells[1].Text;
                decimal feeAmount = 0;
                if (!decimal.TryParse(row.Cells[2].Text.Replace("₹", "").Replace(",", "").Trim(), out feeAmount))
                {
                    Debug.WriteLine($"Invalid FeeAmount: {row.Cells[2].Text}"); 
                }

                // Skip the Late Fee row when creating fee entries
                if (feeName == "Late Fee")
                    continue;

                int totalFeeAmount = GetFeeAmount(feeMasterID);
                feeDetailData.Fees.Add(new
                {
                    FeeMasterID = feeMasterID == 0 ? (object)DBNull.Value : (object)feeMasterID, 
                    FeeName = feeName,
                    FeeAmount = feeAmount,
                    TotalFeeAmount = totalFeeAmount,
                    PayAmount = feeAmount,
                    BalanceAmount = totalFeeAmount - feeAmount,
                    LateFee = 0,
                    PaymentMethod = "",
                    PaymentReference = "",
                    PaymentStatus = 1
                });
            }

            GridViewRow lateFeeRow = gvFeeMaster.Rows.Cast<GridViewRow>()
                .FirstOrDefault(r => r.Cells[1].Text == "Late Fee");

            if (lateFeeRow != null)
            {
                decimal lateFeeAmount = 0;
                decimal.TryParse(lateFeeRow.Cells[2].Text.Replace("₹", "").Replace(",", "").Trim(), out lateFeeAmount);

                feeDetailData.Fees.Add(new
                {
                    FeeMasterID = (object)DBNull.Value,
                    FeeName = "Late Fee",
                    FeeAmount = lateFeeAmount,
                    TotalFeeAmount = (int)lateFeeAmount,
                    PayAmount = lateFeeAmount,
                    BalanceAmount = 0,
                    LateFee = 1,
                    PaymentMethod = "",
                    PaymentReference = "",
                    PaymentStatus = 1
                });
            }

            // Serialize the data to JSON string
            string jsonData = JsonConvert.SerializeObject(feeDetailData);
            return jsonData;
        }

        private int GetFeeAmount(int FeeMasterID)
        {
            try
            { 
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT FeeAmount FROM FeeMaster WHERE FeeMasterID = @FeeMasterID AND SchoolMaster_SchoolID = @SchoolID", conn))
                    {
                        cmd.Parameters.AddWithValue("@FeeMasterID", FeeMasterID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return int.Parse(reader["FeeAmount"].ToString());
                            }
                            else
                            {
                                return 0;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }

        private void InsertIntoFeeDetail(string paymentReference, string paymentMethod, PaymentDetailModel paymentData)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Validate payment data
                        if (paymentData == null)
                        {
                            throw new ArgumentNullException(nameof(paymentData), "Payment data is null");
                        }

                        if (paymentData.Fees == null || paymentData.Fees.Count == 0)
                        {
                            throw new InvalidOperationException("No fee items found in payment data");
                        }

                        int TotalPaidAmount = 0;
                        foreach (var fee in paymentData.Fees)
                        {
                            SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO FeeDetail (
                                FeeDetailID, FeeMasterID, StudentMaster_StudentID, Standards_StandardID, Divisions_DivisionID,
                                TotalFeeAmount, PayAmount, BalanceAmount, LateFee, PaymentDate, ChqDate, PaymentMethod, PaymentReference,
                                PaymentStatus, AcademicYear, SchoolMaster_SchoolID
                            ) VALUES (
                                @FeeDetailID, @FeeMasterID, @StudentID, @StandardID, @DivisionID,
                                @TotalFeeAmount, @PayAmount, @BalanceAmount, @LateFee, @PaymentDate, @ChqDate, @PaymentMethod, @PaymentReference,
                                @PaymentStatus, @AcademicYear, @SchoolID
                            )", conn, transaction);

                            cmd.Parameters.AddWithValue("@FeeDetailID", paymentData.FeeDetailID);
                            cmd.Parameters.AddWithValue("@StudentID", paymentData.StudentID);
                            cmd.Parameters.AddWithValue("@StandardID", paymentData.StandardID);
                            cmd.Parameters.AddWithValue("@DivisionID", paymentData.DivisionID);
                            cmd.Parameters.AddWithValue("@TotalFeeAmount", fee.TotalFeeAmount);
                            cmd.Parameters.AddWithValue("@PayAmount", fee.PayAmount);
                            cmd.Parameters.AddWithValue("@BalanceAmount", fee.BalanceAmount);
                            cmd.Parameters.AddWithValue("@LateFee", fee.LateFee);
                            cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@ChqDate", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                            cmd.Parameters.AddWithValue("@PaymentReference", paymentReference);
                            cmd.Parameters.AddWithValue("@PaymentStatus", 1);
                            cmd.Parameters.AddWithValue("@AcademicYear", paymentData.AcademicYear);
                            cmd.Parameters.AddWithValue("@SchoolID", paymentData.SchoolID);

                            if (fee.FeeMasterID == 0 || fee.FeeMasterID == null)
                            {
                                cmd.Parameters.AddWithValue("@FeeMasterID", DBNull.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@FeeMasterID", fee.FeeMasterID);
                            }

                            cmd.ExecuteNonQuery();

                            TotalPaidAmount += Convert.ToInt32(fee.PayAmount);
                        }

                        InsertIntoFeeRecords(paymentData.FeeDetailID, paymentData.TermType, TotalPaidAmount);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }

        public static class NumberToWordsConverter
        {
            private static readonly string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            private static readonly string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            private static readonly string[] thousands = { "", "Thousand", "Million", "Billion" };

            public static string ToWords(int number)
            {
                if (number == 0) return "Zero";

                string words = "";

                int thousandCounter = 0;

                while (number > 0)
                {
                    if (number % 1000 != 0)
                    {
                        words = ConvertHundreds(number % 1000) + thousands[thousandCounter] + " " + words;
                    }
                    number /= 1000;
                    thousandCounter++;
                }

                return words.Trim();
            }

            private static string ConvertHundreds(int number)
            {
                string result = "";

                if (number > 99)
                {
                    result += ones[number / 100] + " Hundred ";
                    number %= 100;
                }

                if (number > 19)
                {
                    result += tens[number / 10] + " ";
                    number %= 10;
                }

                if (number > 0)
                {
                    result += ones[number] + " ";
                }

                return result;
            }
        }

        private void InsertIntoFeeRecords(string feeDetailID, string termType, int paidAmount)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string receiptNumber = $"REC{StudentID.Substring(0, 6).ToUpper()}{StandardID}{DivisionID}";

                        SqlCommand cmd = new SqlCommand(@"INSERT INTO FeeRecords VALUES (@FeeDetailID, @ReceiptNumber, @TermType, @PaidAmount, @AmountInWords, @PaymentDate, @SchoolMaster_SchoolID)", conn,transaction);

                        cmd.Parameters.AddWithValue("@FeeDetailID", feeDetailID);
                        cmd.Parameters.AddWithValue("@ReceiptNumber", receiptNumber);
                        cmd.Parameters.AddWithValue("@TermType", termType);
                        cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                        cmd.Parameters.AddWithValue("@AmountInWords", NumberToWordsConverter.ToWords(paidAmount));
                        cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }

        private void GetStudentDetails()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT StudentID, Student_FullName, Student_Standard, Student_Division FROM StudentMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            StudentID = reader["StudentID"]?.ToString();
                            StandardID = int.Parse(reader["Student_Standard"].ToString());
                            DivisionID = int.Parse(reader["Student_Division"].ToString());
                            StudentName = reader["Student_FullName"].ToString();
                        }
                    }
                }
            }
        }

        private void GetSchoolSettings()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AcademicYear = reader["AcademicYear"]?.ToString();
                            FirstTermFeeDueDate = reader["FirstTermFeeDueDate"].ToString();
                            SecondTermFeeDueDate = reader["SecondTermFeeDueDate"].ToString();
                            LateFeeCharges = int.Parse(reader["LateFeeCharges"].ToString());
                        }
                    }
                }
            }

            GetFeeDetailsForTermsWithTotalDue("First");
            GetFeeDetailsForTermsWithTotalDue("Second");
        }

        private void GetFeeDetailsForTermsWithTotalDue(string termType)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetFeeDetailsForTermsWithTotalDue", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StandardID", StandardID);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@DivisionID", DivisionID);
                    cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear);

                    DateTime FirstDueDate;
                    if (DateTime.TryParse(FirstTermFeeDueDate, out FirstDueDate))
                    {
                        cmd.Parameters.AddWithValue("@FirstTermDueDate", FirstDueDate.Date);
                    }

                    DateTime SecondDueDate;
                    if (DateTime.TryParse(SecondTermFeeDueDate, out SecondDueDate))
                    {
                        cmd.Parameters.AddWithValue("@SecondTermDueDate", SecondDueDate.Date);
                    }
                    cmd.Parameters.AddWithValue("@LateFeeCharges", LateFeeCharges);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            int IsLateFee = 0;
                            if (reader["IsLateFee"] != DBNull.Value)
                            {
                                IsLateFee = int.Parse(reader["IsLateFee"].ToString());
                            }

                            if (IsLateFee == 1)
                            {
                                LateFeeNote.Visible = true;
                                LateFeeNote.InnerHtml = "Note: Late Fee Charges (₹10/day) applied on your total due.";
                            }

                            string TotalDue = reader["TotalFeesDue"].ToString();
                            decimal totalDueAmount;

                            if (decimal.TryParse(TotalDue, out totalDueAmount))
                            {
                                txtTotalDue.Text = string.Format("₹ {0:N0}", totalDueAmount);
                            }
                            else
                            {
                                txtTotalDue.Text = "₹ 0";
                            }

                            // Get the total fee amounts for both terms
                            decimal firstTermAmount = reader.GetDecimal(reader.GetOrdinal("FirstTermAmount"));
                            decimal secondTermAmount = reader.GetDecimal(reader.GetOrdinal("SecondTermAmount"));

                            if (termType == "First")
                            {
                                FirstTermAmount.InnerHtml = string.Format("₹ {0:N0}", firstTermAmount);
                            }
                            else if (termType == "Second")
                            {
                                SecondTermAmount.InnerHtml = string.Format("₹ {0:N0}", secondTermAmount);
                            }

                            string firstTermStatus = reader["FirstTermStatus"].ToString();
                            string secondTermStatus = reader["SecondTermStatus"].ToString();

                            if (firstTermStatus == "Pending")
                            {
                                FirstTermFeeStatus.Attributes["class"] = "bg-danger text-white";
                            }
                            else
                            {
                                FirstTermFeeStatus.Attributes["class"] = "bg-success text-white";
                            }

                            if (secondTermStatus == "Pending")
                            {
                                SecondTermFeeStatus.Attributes["class"] = "bg-danger text-white";
                            }
                            else
                            {
                                SecondTermFeeStatus.Attributes["class"] = "bg-success text-white";
                            }

                            // Set payment status for each term
                            FirstTermFeeStatus.InnerHtml = firstTermStatus;
                            SecondTermFeeStatus.InnerHtml = secondTermStatus;

                            if (DateTime.TryParse(FirstTermFeeDueDate, out DateTime firstTermDueDate))
                            {
                                FirstTermDate.InnerHtml = firstTermDueDate.ToString("dd/MM/yyyy");
                            }

                            if (DateTime.TryParse(SecondTermFeeDueDate, out DateTime secondTermDueDate))
                            {
                                SecondTermDate.InnerHtml = secondTermDueDate.ToString("dd/MM/yyyy");
                            }
                        }
                    }
                }
            }
        }

        private void BindTermOptions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if there are records in FeeDetail for the selected student
                SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM FeeDetail WHERE 
                StudentMaster_StudentID = @StudentID AND 
                Standards_StandardID = @StandardID AND 
                Divisions_DivisionID = @DivisionID AND 
                PaymentStatus = 1 AND 
                AcademicYear = @AcademicYear AND 
                SchoolMaster_SchoolID = @SchoolID", conn);

                cmd.Parameters.AddWithValue("@StudentID", StudentID);
                cmd.Parameters.AddWithValue("@StandardID", StandardID);
                cmd.Parameters.AddWithValue("@DivisionID", DivisionID);
                cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                int feeDetailCount = (int)cmd.ExecuteScalar();

                if (feeDetailCount == 0) // Scenario 1: No record in FeeDetail, show "Term 1" and "Both Term"
                {
                    ddlTerm.Items.Clear();
                    ddlTerm.Items.Add(new ListItem("Select", ""));
                    ddlTerm.Items.Add(new ListItem("Term 1", "Term1"));
                    ddlTerm.Items.Add(new ListItem("Both Term", "Both Term"));
                }
                else
                {
                    // Scenario 2: Record exists, check PayAmount and FeeAmount
                    SqlCommand cmdCheckAmount = new SqlCommand(@"
                    SELECT SUM(PayAmount) 
                    FROM FeeDetail 
                    WHERE StudentMaster_StudentID = @StudentID
                    AND Standards_StandardID = @StandardID
                    AND Divisions_DivisionID = @DivisionID
                    AND PaymentStatus = 1
                    AND LateFee = 0
                    AND AcademicYear = @AcademicYear
                    AND SchoolMaster_SchoolID = @SchoolID", conn);

                    cmdCheckAmount.Parameters.AddWithValue("@StudentID", StudentID);
                    cmdCheckAmount.Parameters.AddWithValue("@StandardID", StandardID);
                    cmdCheckAmount.Parameters.AddWithValue("@DivisionID", DivisionID);
                    cmdCheckAmount.Parameters.AddWithValue("@AcademicYear", AcademicYear);
                    cmdCheckAmount.Parameters.AddWithValue("@SchoolID", SchoolID);

                    int paidAmount = (int)cmdCheckAmount.ExecuteScalar();

                    SqlCommand cmdFeeMaster = new SqlCommand(@"
                    SELECT SUM(FeeAmount) 
                    FROM FeeMaster 
                    WHERE Standards_StandardID = @StandardID
                    AND SchoolMaster_SchoolID = @SchoolID", conn);

                    cmdFeeMaster.Parameters.AddWithValue("@StandardID", StandardID);
                    cmdFeeMaster.Parameters.AddWithValue("@SchoolID", SchoolID);

                    int totalFeeAmount = (int)cmdFeeMaster.ExecuteScalar();

                    if (paidAmount >= totalFeeAmount) // Scenario 3: Full fees paid, no dropdown, show message
                    {
                        lblError.Text = "Your school fees are fully paid. Currently No Outstanding Fees.";
                        lblFeeTerm.Visible = false;
                        ddlTerm.Visible = false;
                        gvFeeMaster.Visible = false;
                        lblPayAmount.Visible = false;
                        amountTextBox.Visible = false;
                        payButton.Visible = false;
                    }
                    else // Scenario 4: Some fee remaining, check for Term 2
                    {
                        SqlCommand cmdFirstTermAmount = new SqlCommand(@"
                        SELECT SUM(CASE 
                            WHEN FeeTermType = 'First' THEN FeeAmount
                            WHEN FeeTermType = 'Both' THEN FeeAmount / 2
                            ELSE 0
                        END) AS TotalFeeAmount
                        FROM FeeMaster
                        WHERE Standards_StandardID = @StandardID
                        AND SchoolMaster_SchoolID = @SchoolID", conn);

                        cmdFirstTermAmount.Parameters.AddWithValue("@StandardID", StandardID);
                        cmdFirstTermAmount.Parameters.AddWithValue("@SchoolID", SchoolID);

                        int firstTermFeeAmount = (int)cmdFirstTermAmount.ExecuteScalar();

                        if (paidAmount >= firstTermFeeAmount) // Scenario 5: Paid amount equals Term 1 fee, show "Term 2" option
                        {
                            ddlTerm.Items.Clear();
                            ddlTerm.Items.Add(new ListItem("Select", ""));
                            ddlTerm.Items.Add(new ListItem("Term 2", "Term2"));
                        }
                        else // Scenario 6: Not enough paid for Term 1, show both Term options
                        {
                            ddlTerm.Items.Clear();
                            ddlTerm.Items.Add(new ListItem("Select", ""));
                            ddlTerm.Items.Add(new ListItem("Term 1", "Term1"));
                            ddlTerm.Items.Add(new ListItem("Both Term", "Both Term"));
                        }
                    }
                }
            }
        }

        private void GetSchoolName()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSchoolName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SchoolName = reader["SchoolName"].ToString();
                        }
                    }
                }
            }
        }

        protected void ddlTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTerm = ddlTerm.SelectedValue;

            BindGridView(selectedTerm);
        }

        private void BindGridView(string selectedTerm)
        {
            gvFeeMaster.Visible = true;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd;

                DateTime dueDate = DateTime.MinValue;
                int lateFeePerDay = LateFeeCharges;
                decimal totalLateFee = 0;

                if (selectedTerm == "Term1")
                {
                    // Fetch records for Term 1 and half of Both Term
                    cmd = new SqlCommand(@"
                    SELECT FeeMasterID, FeeName, 
                           CASE 
                               WHEN FeeTermType = 'First' THEN FeeAmount 
                               WHEN FeeTermType = 'Both' THEN FeeAmount / 2
                               ELSE 0
                           END AS FeeAmount
                    FROM FeeMaster
                    WHERE Standards_StandardID = @StandardID 
                    AND SchoolMaster_SchoolID = @SchoolID 
                    AND (FeeTermType = 'First' OR FeeTermType = 'Both')", conn);

                    // Parse FirstTermFeeDueDate to check for late fee calculation
                    if (DateTime.TryParse(FirstTermFeeDueDate, out dueDate))
                    {
                        int daysLate = (DateTime.Now - dueDate).Days;
                        if (daysLate > 0) totalLateFee = daysLate * lateFeePerDay;
                    }
                }
                else if (selectedTerm == "Both Term")
                {
                    // Fetch records for Both Term (Term 1 + Term 2 + half of Both)
                    cmd = new SqlCommand(@"
                    SELECT FeeMasterID, FeeName, FeeAmount
                    FROM FeeMaster
                    WHERE Standards_StandardID = @StandardID 
                    AND SchoolMaster_SchoolID = @SchoolID", conn);

                    // Check both First and Second Term Due Dates for Late Fees
                    decimal term1LateFee = 0, term2LateFee = 0;

                    if (DateTime.TryParse(FirstTermFeeDueDate, out dueDate))
                    {
                        int daysLateTerm1 = (DateTime.Now - dueDate).Days;
                        if (daysLateTerm1 > 0) term1LateFee = daysLateTerm1 * lateFeePerDay;
                    }

                    if (DateTime.TryParse(SecondTermFeeDueDate, out dueDate))
                    {
                        int daysLateTerm2 = (DateTime.Now - dueDate).Days;
                        if (daysLateTerm2 > 0) term2LateFee = daysLateTerm2 * lateFeePerDay;
                    }

                    totalLateFee = term1LateFee + term2LateFee;
                }
                else if (selectedTerm == "Term2")
                {
                    // Fetch records for Term 2 and half of Both Term
                    cmd = new SqlCommand(@"
                    SELECT FeeMasterID, FeeName, 
                           CASE 
                               WHEN FeeTermType = 'Second' THEN FeeAmount 
                               WHEN FeeTermType = 'Both' THEN FeeAmount / 2
                               ELSE 0
                           END AS FeeAmount
                    FROM FeeMaster
                    WHERE Standards_StandardID = @StandardID 
                    AND SchoolMaster_SchoolID = @SchoolID 
                    AND (FeeTermType = 'Second' OR FeeTermType = 'Both')", conn);

                    // Parse SecondTermFeeDueDate to check for late fee calculation
                    if (DateTime.TryParse(SecondTermFeeDueDate, out dueDate))
                    {
                        int daysLate = (DateTime.Now - dueDate).Days;
                        if (daysLate > 0) totalLateFee = daysLate * lateFeePerDay;
                    }
                }
                else
                {
                    // If no valid term is selected, exit the function
                    amountTextBox.Text = "";
                    gvFeeMaster.Visible = false;
                    return;
                }

                cmd.Parameters.AddWithValue("@StandardID", StandardID);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("SRNo", typeof(int));

                int srNo = 1;
                decimal totalAmount = 0;

                // Format the amount and calculate total
                foreach (DataRow row in dt.Rows)
                {
                    row["SRNo"] = srNo++;
                    decimal feeAmount = Convert.ToDecimal(row["FeeAmount"]);
                    row["FeeAmount"] = feeAmount;

                    // Sum the total amount
                    totalAmount += feeAmount;
                }

                // Add Late Fee to the total amount
                totalAmount += totalLateFee;

                // Add Late Fee row
                DataRow lateFeeRow = dt.NewRow();
                lateFeeRow["SRNo"] = srNo++;
                lateFeeRow["FeeName"] = "Late Fee";
                lateFeeRow["FeeAmount"] = totalLateFee;
                dt.Rows.Add(lateFeeRow);

                DataRow totalRow = dt.NewRow();
                totalRow["FeeName"] = "Total";
                totalRow["FeeAmount"] = totalAmount;
                dt.Rows.Add(totalRow);

                // If no records are found
                if (dt.Rows.Count == 0)
                {
                    lblError.Text = "No fee records found for the selected term.";
                }
                else
                {
                    amountTextBox.Text = string.Format("₹ {0:N0}", totalAmount);
                    gvFeeMaster.DataSource = dt;
                    gvFeeMaster.DataBind();
                }
            }
        }

        protected void gvFeeMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.Style["padding"] = "5px 0px 5px 15px !important";
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                decimal feeAmount = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "FeeAmount"));
                e.Row.Cells[2].Text = string.Format("₹ {0:N0}", feeAmount);

                //HiddenField hfFeeMasterID = (HiddenField)e.Row.FindControl("hfFeeMasterID");
                //if (hfFeeMasterID != null)
                //{
                //    string feeMasterID = hfFeeMasterID.Value;
                //}

                // If the row is the total row, format it to appear bold and styled
                if (e.Row.Cells.Count > 1 && e.Row.Cells[1].Text == "Total")
                {
                    e.Row.Cells[1].Font.Bold = true;
                    e.Row.Cells[1].Font.Size = 12;

                    e.Row.Cells[2].Font.Bold = true;
                    e.Row.Cells[2].Font.Size = 12;
                }
            }
        }

        protected void PayButton_Click(object sender, EventArgs e)
        {
            string TermName = ddlTerm.SelectedItem.Text;

            if(TermName == "Both Term")
            {
                TermName = "Term 1 & Term 2";
            }

            string cleanedAmount = amountTextBox.Text.Replace("₹", "").Replace(",", "").Trim();

            if (!decimal.TryParse(cleanedAmount, out decimal amount) || amount <= 0)
            {
                lblError.Text = "Please enter a valid amount.";
                return;
            }

            try
            {
                string paymentDataJson = GetPaymentDataJson();
                Session["PaymentDataJson"] = paymentDataJson;

                // Create Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "inr",
                                UnitAmount = (long)(amount * 100),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = SchoolName,
                                    Description = $"{StudentName}'s Fee Payment - {TermName} ({AcademicYear})"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/Student/FeePayment.aspx?status=success&amount={amount}&session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/Student/FeePayment.aspx?status=cancel"
                };

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Redirect(session.Url, false);
            }
            catch (StripeException stripeEx)
            {
                string stripeErrorMessage = $"Stripe Error: {stripeEx.Message}\n{stripeEx.StackTrace}";
                lblError.Text = $"Payment Error: {stripeEx.Message}";

                Debug.WriteLine(stripeErrorMessage);

            }
            catch (Exception ex)
            {
                string errorMessage = $"General Error: {ex.Message}\n{ex.StackTrace}";
                lblError.Text = $"An unexpected error occurred: {ex.Message}";

                Debug.WriteLine(errorMessage);
            }
        }

    }
}