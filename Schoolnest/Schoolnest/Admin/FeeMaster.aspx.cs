using System;
using System.Collections.Generic;
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
                LoadStandards();
                InitializeFeeRepeater();
                btnAddRow.Enabled = false;
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
            ViewState["Fees"] = new List<FeeDetail>();
            ViewState["RemovedFees"] = new List<int>();
            BindRepeater();
        }

        private void BindRepeater()
        {
            var fees = ViewState["Fees"] as List<FeeDetail>;
            if (fees != null && fees.Count > 0)
            {
                rptFee.DataSource = fees;
                rptFee.DataBind();
            }
            else
            {
                // Ensure at least one empty row is shown when no fees exist
                fees = new List<FeeDetail> { new FeeDetail() };
                rptFee.DataSource = fees;
                rptFee.DataBind();
            }
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStandard.SelectedValue == "0")
            {
                btnAddRow.Enabled = false;
                InitializeFeeRepeater();
                ShowErrorMessage("Please select a valid standard.");
            }
            else
            {
                btnAddRow.Enabled = true;
                LoadFeesForStandard(ddlStandard.SelectedValue);
            }
        }

        private void LoadFeesForStandard(string standardId)
        {
            var fees = new List<FeeDetail>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT FeeMasterID, FeeName, FeeAmount, FeeTermType FROM FeeMaster WHERE Standards_StandardID = @StandardID AND SchoolMaster_SchoolID = @SchoolID", conn);
                cmd.Parameters.AddWithValue("@StandardID", standardId);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                using (var reader = cmd.ExecuteReader())
                {
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
            }

            ViewState["Fees"] = fees;
            BindRepeater();
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            if (!ValidateAllFees()) return;

            var fees = ViewState["Fees"] as List<FeeDetail>;
            fees.Add(new FeeDetail());
            ViewState["Fees"] = fees;
            BindRepeater();
        }

        protected void rptFee_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int index = e.Item.ItemIndex;
                var fees = ViewState["Fees"] as List<FeeDetail>;

                if (index < fees.Count)
                {
                    int feeMasterIdToRemove = fees[index].FeeMasterID;
                    if (feeMasterIdToRemove != 0)
                    {
                        var removedFees = ViewState["RemovedFees"] as List<int>;
                        removedFees.Add(feeMasterIdToRemove);
                        ViewState["RemovedFees"] = removedFees;
                    }

                    fees.RemoveAt(index);
                    ViewState["Fees"] = fees;
                    BindRepeater();
                }
            }
        }

        protected void rptFee_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var feeDetail = e.Item.DataItem as FeeDetail;

                if (feeDetail != null)
                {
                    var txtFeeName = e.Item.FindControl("txtFeeName") as TextBox;
                    var txtFeeAmount = e.Item.FindControl("txtFeeAmount") as TextBox;
                    var ddlTermType = e.Item.FindControl("ddlTermType") as DropDownList;

                    if (txtFeeName != null)
                        txtFeeName.Text = feeDetail.FeeName;

                    if (txtFeeAmount != null)
                        txtFeeAmount.Text = feeDetail.Amount.ToString();

                    if (ddlTermType != null && !string.IsNullOrEmpty(feeDetail.TermType))
                        ddlTermType.SelectedValue = feeDetail.TermType;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateAllFees()) return;

            var fees = ViewState["Fees"] as List<FeeDetail>;
            var removedFees = ViewState["RemovedFees"] as List<int>;

            if (ddlStandard.SelectedValue == "0")
            {
                ShowErrorMessage("Please select a valid standard.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Handle removed fees
                        foreach (var feeId in removedFees)
                        {
                            var cmd = new SqlCommand("DELETE FROM FeeMaster WHERE FeeMasterID = @FeeMasterID", conn, transaction);
                            cmd.Parameters.AddWithValue("@FeeMasterID", feeId);
                            cmd.ExecuteNonQuery();
                        }

                        // Insert or update fees
                        foreach (var fee in fees)
                        {
                            var cmd = new SqlCommand("InsertUpdateFeeMaster", conn, transaction)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            cmd.Parameters.AddWithValue("@FeeMasterID", fee.FeeMasterID == 0 ? (object)DBNull.Value : fee.FeeMasterID);
                            cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                            cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                            cmd.Parameters.AddWithValue("@FeeName", fee.FeeName);
                            cmd.Parameters.AddWithValue("@Amount", fee.Amount);
                            cmd.Parameters.AddWithValue("@TermType", fee.TermType);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, GetType(), "SuccessMessage", "alert('Fee details saved successfully.');", true);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowErrorMessage($"Error saving fees: {ex.Message}");
                    }
                }
            }
        }

        private bool ValidateAllFees()
        {
            foreach (RepeaterItem item in rptFee.Items)
            {
                var txtFeeName = (TextBox)item.FindControl("txtFeeName");
                var txtFeeAmount = (TextBox)item.FindControl("txtFeeAmount");
                var ddlTermType = (DropDownList)item.FindControl("ddlTermType");

                if (string.IsNullOrWhiteSpace(txtFeeName.Text))
                {
                    ShowErrorMessage("Fee Name is required.");
                    return false;
                }

                if (!decimal.TryParse(txtFeeAmount.Text, out var amount) || amount <= 0)
                {
                    ShowErrorMessage("Valid Fee Amount is required.");
                    return false;
                }

                if (ddlTermType.SelectedValue == "0")
                {
                    ShowErrorMessage("Term Type is required.");
                    return false;
                }
            }

            return true;
        }

        private void ShowErrorMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage", $"alert('{message}');", true);
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
