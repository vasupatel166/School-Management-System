using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                LoadStandards();
                InitializeFeeRepeater();
                btnAddRow.Enabled = false;
            }
        }

        protected void ValidateStandard(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(ddlStandard.SelectedValue) && ddlStandard.SelectedValue != "0";
            if (!args.IsValid)
            {
                ((CustomValidator)source).ErrorMessage = "Please select a valid standard";
            }
        }

        protected void ValidateFeeName(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrWhiteSpace(args.Value);
            if (!args.IsValid)
            {
                ((CustomValidator)source).ErrorMessage = "Fee name cannot be empty";
            }
        }

        protected void ValidateFeeAmount(object source, ServerValidateEventArgs args)
        {
            if (!decimal.TryParse(args.Value, out decimal amount))
            {
                args.IsValid = false;
                ((CustomValidator)source).ErrorMessage = "Please enter a valid amount";
                return;
            }

            args.IsValid = amount > 0;
            if (!args.IsValid)
            {
                ((CustomValidator)source).ErrorMessage = "Amount must be greater than 0";
            }
        }

        private void LoadStandards()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT StandardID, StandardName FROM Standards WHERE SchoolMaster_SchoolID = @SchoolID", conn);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                ddlStandard.DataSource = cmd.ExecuteReader();
                ddlStandard.DataTextField = "StandardName";
                ddlStandard.DataValueField = "StandardID";
                ddlStandard.DataBind();
                ddlStandard.Items.Insert(0, new ListItem("Select Standard", "0"));
            }
        }

        private void InitializeFeeRepeater()
        {
            List<FeeDetail> fees = new List<FeeDetail>();
            ViewState["Fees"] = fees;
            BindRepeater();
        }

        private void BindRepeater()
        {
            List<FeeDetail> fees = ViewState["Fees"] as List<FeeDetail> ?? new List<FeeDetail>();
            rptFee.DataSource = fees;
            rptFee.DataBind();

            // Set the values for each row after binding
            foreach (RepeaterItem item in rptFee.Items)
            {
                FeeDetail fee = fees[item.ItemIndex];
                TextBox txtFeeName = (TextBox)item.FindControl("txtFeeName");
                TextBox txtFeeAmount = (TextBox)item.FindControl("txtFeeAmount");
                DropDownList ddlTermType = (DropDownList)item.FindControl("ddlTermType");

                txtFeeName.Text = fee.FeeName;
                txtFeeAmount.Text = fee.Amount.ToString();
                ddlTermType.SelectedValue = fee.TermType;
            }
        }

        private void LoadFeesForStandard(string standardId)
        {
            List<FeeDetail> fees = new List<FeeDetail>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT FeeMasterID, FeeName, FeeAmount, FeeTermType FROM FeeMaster WHERE Standards_StandardID = @StandardID AND SchoolMaster_SchoolID = @SchoolID", conn);
                cmd.Parameters.AddWithValue("@StandardID", standardId);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    fees.Add(new FeeDetail
                    {
                        FeeMasterID = Convert.ToInt32(reader["FeeMasterID"]),
                        FeeName = reader["FeeName"].ToString(),
                        Amount = Convert.ToDecimal(reader["FeeAmount"]),
                        TermType = reader["FeeTermType"].ToString()
                    });
                }
            }

            ViewState["Fees"] = fees;
            BindRepeater();
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            CustomValidator standardValidator = new CustomValidator();
            standardValidator.ServerValidate += new ServerValidateEventHandler(ValidateStandard);
            standardValidator.Validate();

            if (!standardValidator.IsValid)
            {
                ShowErrorMessage(standardValidator.ErrorMessage);
                return;
            }

            SaveCurrentState();

            List<FeeDetail> fees = ViewState["Fees"] as List<FeeDetail> ?? new List<FeeDetail>();
            fees.Add(new FeeDetail());
            ViewState["Fees"] = fees;
            BindRepeater();
        }

        private void SaveCurrentState()
        {
            List<FeeDetail> fees = ViewState["Fees"] as List<FeeDetail> ?? new List<FeeDetail>();

            for (int i = 0; i < rptFee.Items.Count; i++)
            {
                RepeaterItem item = rptFee.Items[i];
                TextBox txtFeeName = (TextBox)item.FindControl("txtFeeName");
                TextBox txtFeeAmount = (TextBox)item.FindControl("txtFeeAmount");
                DropDownList ddlTermType = (DropDownList)item.FindControl("ddlTermType");

                if (i < fees.Count)
                {
                    fees[i].FeeName = txtFeeName.Text;
                    fees[i].Amount = !string.IsNullOrEmpty(txtFeeAmount.Text) ? Convert.ToDecimal(txtFeeAmount.Text) : 0;
                    fees[i].TermType = ddlTermType.SelectedValue;
                }
            }

            ViewState["Fees"] = fees;
        }

        protected void rptFee_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                SaveCurrentState(); // Save current state before removing
                List<FeeDetail> fees = ViewState["Fees"] as List<FeeDetail>;
                int index = e.Item.ItemIndex;

                if (index < fees.Count)
                {
                    int feeMasterIdToRemove = fees[index].FeeMasterID;
                    if (feeMasterIdToRemove != 0)
                    {
                        List<int> removedFeeIds = ViewState["RemovedFees"] as List<int> ?? new List<int>();
                        removedFeeIds.Add(feeMasterIdToRemove);
                        ViewState["RemovedFees"] = removedFeeIds;
                    }

                    fees.RemoveAt(index);
                    ViewState["Fees"] = fees;
                    BindRepeater();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateAllFees())
                return;

            SaveCurrentState();
            List<FeeDetail> fees = ViewState["Fees"] as List<FeeDetail>;
            List<int> removedFeeIds = ViewState["RemovedFees"] as List<int> ?? new List<int>();

            CustomValidator standardValidator = new CustomValidator();
            standardValidator.ServerValidate += new ServerValidateEventHandler(ValidateStandard);
            standardValidator.Validate();

            if (!standardValidator.IsValid)
            {
                ShowErrorMessage(standardValidator.ErrorMessage);
                return;
            }

            if (!fees.Any())
            {
                ShowErrorMessage("Please add at least one fee");
                return;
            }

            CustomValidator feeNameValidator = new CustomValidator();
            feeNameValidator.ServerValidate += new ServerValidateEventHandler(ValidateFeeName);
            feeNameValidator.Validate();

            if (!feeNameValidator.IsValid)
            {
                ShowErrorMessage(feeNameValidator.ErrorMessage);
                return;
            }

            CustomValidator feeAmountValidator = new CustomValidator();
            feeAmountValidator.ServerValidate += new ServerValidateEventHandler(ValidateFeeAmount);
            feeAmountValidator.Validate();

            if (!feeAmountValidator.IsValid)
            {
                ShowErrorMessage(feeAmountValidator.ErrorMessage);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete removed fees
                        foreach (int feeMasterId in removedFeeIds)
                        {
                            using (SqlCommand cmd = new SqlCommand("DELETE FROM FeeMaster WHERE FeeMasterID = @FeeMasterID", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FeeMasterID", feeMasterId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Insert or update fees
                        foreach (FeeDetail fee in fees)
                        {
                            using (SqlCommand cmd = new SqlCommand("InsertUpdateFeeMaster", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@FeeMasterID", fee.FeeMasterID == 0 ? (object)DBNull.Value : fee.FeeMasterID);
                                cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                                cmd.Parameters.AddWithValue("@FeeName", fee.FeeName);
                                cmd.Parameters.AddWithValue("@Amount", fee.Amount);
                                cmd.Parameters.AddWithValue("@TermType", fee.TermType);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, GetType(), "SuccessMessage", "alert('Fee saved successfully');", true);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage", $"alert('Error saving fees: {ex.Message}');", true);
                    }
                }
            }
        }

        private bool ValidateAllFees()
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            foreach (RepeaterItem item in rptFee.Items)
            {
                TextBox txtFeeName = (TextBox)item.FindControl("txtFeeName");
                TextBox txtFeeAmount = (TextBox)item.FindControl("txtFeeAmount");
                CustomValidator cvFeeName = (CustomValidator)item.FindControl("cvFeeName");
                CustomValidator cvFeeAmount = (CustomValidator)item.FindControl("cvFeeAmount");

                // Validate Fee Name
                cvFeeName.Validate();
                if (!cvFeeName.IsValid)
                {
                    errorMessage = cvFeeName.ErrorMessage;
                    isValid = false;
                    break;
                }

                // Validate Fee Amount
                cvFeeAmount.Validate();
                if (!cvFeeAmount.IsValid)
                {
                    errorMessage = cvFeeAmount.ErrorMessage;
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                ShowErrorMessage(errorMessage);
            }

            return isValid;
        }

        private void ShowErrorMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage",$"alert('{message}');", true);
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStandardId = ddlStandard.SelectedValue;
            CustomValidator standardValidator = new CustomValidator();
            standardValidator.ServerValidate += new ServerValidateEventHandler(ValidateStandard);
            standardValidator.Validate();

            btnAddRow.Enabled = standardValidator.IsValid;

            if (btnAddRow.Enabled)
            {
                LoadFeesForStandard(selectedStandardId);
            }
            else
            {
                InitializeFeeRepeater();
                ShowErrorMessage("Please select a valid standard");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }
    }

    [Serializable]
    public class FeeDetail
    {
        public int FeeMasterID { get; set; }
        public string FeeName { get; set; }
        public decimal Amount { get; set; }
        public string TermType { get; set; }
    }
}