using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class FeeMaster : System.Web.UI.Page
    {

        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {

            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadDropDowns();
            }
        }

        private void LoadDropDowns()
        {
            // Load Schools
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(@"SELECT SchoolID, SchoolName FROM SchoolMaster WHERE SchoolID = @SchoolID", conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    ddlSchool.DataSource = cmd.ExecuteReader();
                    ddlSchool.DataTextField = "SchoolName";
                    ddlSchool.DataValueField = "SchoolID";
                    ddlSchool.DataBind();
                    conn.Close();
                }

                // Load Standards
                using (SqlCommand cmd = new SqlCommand(@"SELECT StandardID, StandardName FROM Standards WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    ddlStandard.DataSource = cmd.ExecuteReader();
                    ddlStandard.DataTextField = "StandardName";
                    ddlStandard.DataValueField = "StandardID";
                    ddlStandard.DataBind();
                    conn.Close();
                }
            }
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFeeData();
        }

        private void LoadFeeData()
        {
            if (string.IsNullOrEmpty(txtAcademicYear.Text) || ddlStandard.SelectedValue == "")
                return;

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetFeeMasterByAcademicYearAndStandard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text);
                    cmd.Parameters.AddWithValue("@StandardID", Convert.ToInt32(ddlStandard.SelectedValue));

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate form fields
                            ddlSchool.SelectedValue = reader["SchoolMaster_SchoolID"].ToString();
                            txtFeeCode.Text = reader["FeeCode"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);

                            // Core Fees
                            txtAdmissionFee.Text = Convert.ToDecimal(reader["AdmissionFee"]).ToString("F2");
                            txtRegistrationFee.Text = Convert.ToDecimal(reader["RegistrationFee"]).ToString("F2");
                            txtSecurityDeposit.Text = Convert.ToDecimal(reader["SecurityDeposit"]).ToString("F2");
                            txtAnnualCharges.Text = Convert.ToDecimal(reader["AnnualCharges"]).ToString("F2");

                            // Regular Fees
                            txtTuitionFee.Text = Convert.ToDecimal(reader["TuitionFee"]).ToString("F2");
                            txtDevelopmentFee.Text = Convert.ToDecimal(reader["DevelopmentFee"]).ToString("F2");
                            txtLibraryFee.Text = Convert.ToDecimal(reader["LibraryFee"]).ToString("F2");
                            txtComputerFee.Text = Convert.ToDecimal(reader["ComputerFee"]).ToString("F2");
                            txtSportsFee.Text = Convert.ToDecimal(reader["SportsFee"]).ToString("F2");
                            txtTransportFee.Text = Convert.ToDecimal(reader["TransportFee"]).ToString("F2");
                            txtLabFee.Text = Convert.ToDecimal(reader["LabFee"]).ToString("F2");
                            txtMiscFee.Text = Convert.ToDecimal(reader["MiscFee"]).ToString("F2");

                            // Additional Charges
                            txtLateFee.Text = Convert.ToDecimal(reader["LateFee"]).ToString("F2");
                            txtTotalFee.Text = Convert.ToDecimal(reader["TotalFee"]).ToString("F2");

                            // Payment Schedule
                            ddlPaymentScheduleType.SelectedValue = reader["PaymentScheduleType"].ToString();

                            // Due Dates based on schedule type
                            switch (ddlPaymentScheduleType.SelectedValue)
                            {
                                case "Quarterly":
                                    txtFirstQuarter.Text = Convert.ToDateTime(reader["FirstQuarterDueDate"]).ToString("yyyy-MM-dd");
                                    txtSecondQuarter.Text = Convert.ToDateTime(reader["SecondQuarterDueDate"]).ToString("yyyy-MM-dd");
                                    txtThirdQuarter.Text = Convert.ToDateTime(reader["ThirdQuarterDueDate"]).ToString("yyyy-MM-dd");
                                    txtFourthQuarter.Text = Convert.ToDateTime(reader["FourthQuarterDueDate"]).ToString("yyyy-MM-dd");
                                    break;
                                case "HalfYearly":
                                    txtFirstHalf.Text = Convert.ToDateTime(reader["FirstHalfDueDate"]).ToString("yyyy-MM-dd");
                                    txtSecondHalf.Text = Convert.ToDateTime(reader["SecondHalfDueDate"]).ToString("yyyy-MM-dd");
                                    break;
                                case "Annual":
                                    txtAnnualDueDate.Text = Convert.ToDateTime(reader["AnnualDueDate"]).ToString("yyyy-MM-dd");
                                    break;
                            }

                            // Show appropriate schedule div
                            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSchedule",
                                "togglePaymentSchedule();", true);
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateFeeMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text);
                    cmd.Parameters.AddWithValue("@Standards_StandardID", Convert.ToInt32(ddlStandard.SelectedValue));
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", ddlSchool.SelectedValue);
                    cmd.Parameters.AddWithValue("@FeeCode", txtFeeCode.Text);
                    cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);

                    // Core Fees
                    cmd.Parameters.AddWithValue("@AdmissionFee", Convert.ToDecimal(txtAdmissionFee.Text));
                    cmd.Parameters.AddWithValue("@RegistrationFee", Convert.ToDecimal(txtRegistrationFee.Text));
                    cmd.Parameters.AddWithValue("@SecurityDeposit", Convert.ToDecimal(txtSecurityDeposit.Text));
                    cmd.Parameters.AddWithValue("@AnnualCharges", Convert.ToDecimal(txtAnnualCharges.Text));

                    // Regular Fees
                    cmd.Parameters.AddWithValue("@TuitionFee", Convert.ToDecimal(txtTuitionFee.Text));
                    cmd.Parameters.AddWithValue("@DevelopmentFee", Convert.ToDecimal(txtDevelopmentFee.Text));
                    cmd.Parameters.AddWithValue("@LibraryFee", Convert.ToDecimal(txtLibraryFee.Text));
                    cmd.Parameters.AddWithValue("@ComputerFee", Convert.ToDecimal(txtComputerFee.Text));
                    cmd.Parameters.AddWithValue("@SportsFee", Convert.ToDecimal(txtSportsFee.Text));
                    cmd.Parameters.AddWithValue("@TransportFee", Convert.ToDecimal(txtTransportFee.Text));
                    cmd.Parameters.AddWithValue("@LabFee", Convert.ToDecimal(txtLabFee.Text));
                    cmd.Parameters.AddWithValue("@MiscFee", Convert.ToDecimal(txtMiscFee.Text));

                    // Additional Charges
                    cmd.Parameters.AddWithValue("@LateFee", Convert.ToDecimal(txtLateFee.Text));

                    // Payment Schedule
                    cmd.Parameters.AddWithValue("@PaymentScheduleType", ddlPaymentScheduleType.SelectedValue);

                    // Due Dates based on schedule type
                    if (!string.IsNullOrEmpty(txtFirstQuarter.Text))
                        cmd.Parameters.AddWithValue("@FirstQuarterDueDate", Convert.ToDateTime(txtFirstQuarter.Text));
                    if (!string.IsNullOrEmpty(txtSecondQuarter.Text))
                        cmd.Parameters.AddWithValue("@SecondQuarterDueDate", Convert.ToDateTime(txtSecondQuarter.Text));
                    if (!string.IsNullOrEmpty(txtThirdQuarter.Text))
                        cmd.Parameters.AddWithValue("@ThirdQuarterDueDate", Convert.ToDateTime(txtThirdQuarter.Text));
                    if (!string.IsNullOrEmpty(txtFourthQuarter.Text))
                        cmd.Parameters.AddWithValue("@FourthQuarterDueDate", Convert.ToDateTime(txtFourthQuarter.Text));
                    if (!string.IsNullOrEmpty(txtFirstHalf.Text))
                        cmd.Parameters.AddWithValue("@FirstHalfDueDate", Convert.ToDateTime(txtFirstHalf.Text));
                    if (!string.IsNullOrEmpty(txtSecondHalf.Text))
                        cmd.Parameters.AddWithValue("@SecondHalfDueDate", Convert.ToDateTime(txtSecondHalf.Text));
                    if (!string.IsNullOrEmpty(txtAnnualDueDate.Text))
                        cmd.Parameters.AddWithValue("@AnnualDueDate", Convert.ToDateTime(txtAnnualDueDate.Text));

                    //// Check if this is an insert or update based on the FeeID
                    //if (string.IsNullOrEmpty(hdnFeeID.Value))
                    //{
                    //    // Insert operation
                    //    cmd.Parameters.AddWithValue("@FeeID", DBNull.Value);
                    //}
                    //else
                    //{
                    //    // Update operation
                    //    cmd.Parameters.AddWithValue("@FeeID", Convert.ToInt32(hdnFeeID.Value));
                    //}

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // After saving, reload the form to reflect changes
            LoadFeeData();
            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessMessage", "alert('Fee structure saved successfully');", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void ResetForm()
        {
            Response.Redirect("~/Admin/FeeMaster.aspx");
        }
    }
}
