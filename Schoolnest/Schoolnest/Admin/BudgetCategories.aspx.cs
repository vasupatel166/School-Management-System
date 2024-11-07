using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class BudgetCategories : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadBudgetYears();
                LoadBudgetCategories();
            }
        }

        private void LoadBudgetYears()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BudgetID, AcademicYear FROM BudgetMaster WHERE Status = 1 AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlBudgetYear.Items.Add(new ListItem("Select Budget Year", ""));

                    while (reader.Read())
                    {
                        string id = reader["BudgetID"].ToString();
                        string year = reader["AcademicYear"].ToString();
                        ddlBudgetYear.Items.Add(new ListItem(year, id));
                    }
                }
            }
        }

        private void LoadAnnualBudget()
        {
            string BudgetID = ddlBudgetYear.SelectedValue;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT TotalAllocated FROM BudgetMaster WHERE BudgetID = @BudgetID AND Status = 1 AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@BudgetID", BudgetID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        if (decimal.TryParse(reader["TotalAllocated"].ToString(), out decimal totalAllocated))
                        {
                            txtAnnualBudget.Text = string.Format(new System.Globalization.CultureInfo("en-IN"), "₹{0:N2}", totalAllocated);
                        }
                    }
                }
            }
        }

        private void LoadBudgetCategories()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT bc.BudgetCategoryID, bc.BudgetCategoryName, bc.AmountAllocated, bc.ActualExpenditure, bm.TotalAllocated, bm.AcademicYear FROM BudgetCategories bc 
                    INNER JOIN BudgetMaster bm ON bc.BudgetMaster_BudgetID = bm.BudgetID 
                    WHERE bm.Status = 1 AND bc.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    DataTable dt = new DataTable();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    gvBudgetCategories.DataSource = dt;
                    gvBudgetCategories.DataBind();
                }
            }
        }

        protected void ddlBudgetYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBudgetYear.SelectedValue == "")
            {
                txtAnnualBudget.Text = "";
            }
            else
            {
                LoadAnnualBudget();
            }

            ClearInputs();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && !string.IsNullOrEmpty(ddlBudgetYear.SelectedValue))
            {
                int budgetID = int.Parse(ddlBudgetYear.SelectedValue);
                decimal amountAllocated = decimal.Parse(txtAmountAllocated.Text);
                decimal actualExpenditure = decimal.Parse(txtActualExpenditure.Text);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("InsertUpdateBudgetCategory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BudgetCategoryID", ViewState["EditCategoryID"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BudgetCategoryName", txtCategoryName.Text);
                        cmd.Parameters.AddWithValue("@AmountAllocated", amountAllocated);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", actualExpenditure);
                        cmd.Parameters.AddWithValue("@BudgetMaster_BudgetID", budgetID);
                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);
                        cmd.ExecuteNonQuery();
                    }

                    // Update TotalSpent in BudgetMaster
                    using (SqlCommand cmd = new SqlCommand(@"
                UPDATE BudgetMaster 
                SET TotalSpent = (SELECT ISNULL(SUM(ActualExpenditure), 0) 
                                FROM BudgetCategories 
                                WHERE BudgetMaster_BudgetID = @BudgetID) 
                WHERE BudgetID = @BudgetID AND Status = 1 
                AND SchoolMaster_SchoolID = @SchoolID", conn))
                    {
                        cmd.Parameters.AddWithValue("@BudgetID", budgetID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        cmd.ExecuteNonQuery();
                    }
                }

                ViewState["EditCategoryID"] = null;
                txtActualExpenditure.Enabled = false;
                ClearInputs();
                LoadBudgetCategories();
            }
        }

        

        private void UpdateTotalSpentInBudgetMaster(SqlConnection conn, int budgetID)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE BudgetMaster 
                SET TotalSpent = (SELECT ISNULL(SUM(ActualExpenditure), 0) 
                                FROM BudgetCategories 
                                WHERE BudgetMaster_BudgetID = @BudgetID) 
                WHERE BudgetID = @BudgetID AND Status = 1 
                AND SchoolMaster_SchoolID = @SchoolID", conn))
            {
                cmd.Parameters.AddWithValue("@BudgetID", budgetID);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                cmd.ExecuteNonQuery();
            }
        }

        protected void gvBudgetCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditBudget")
            {
                // Load category data for editing
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT bc.*, bm.BudgetID, bm.AcademicYear 
                FROM BudgetCategories bc
                INNER JOIN BudgetMaster bm ON bc.BudgetMaster_BudgetID = bm.BudgetID
                WHERE bc.BudgetCategoryID = @CategoryID", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // Set the category details
                            txtCategoryName.Text = reader["BudgetCategoryName"].ToString();
                            txtAmountAllocated.Text = reader["AmountAllocated"].ToString();
                            txtActualExpenditure.Text = reader["ActualExpenditure"].ToString();
                            txtActualExpenditure.Enabled = true;
                            ViewState["EditCategoryID"] = categoryId;

                            // Set the budget year dropdown
                            string budgetId = reader["BudgetID"].ToString();
                            ddlBudgetYear.SelectedValue = budgetId;

                            // Close the first reader before executing another command
                            reader.Close();

                            // Load the annual budget for the selected year
                            using (SqlCommand budgetCmd = new SqlCommand("SELECT TotalAllocated FROM BudgetMaster WHERE BudgetID = @BudgetID AND Status = 1 AND SchoolMaster_SchoolID = @SchoolID", conn))
                            {
                                budgetCmd.Parameters.AddWithValue("@BudgetID", budgetId);
                                budgetCmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                                using (SqlDataReader budgetReader = budgetCmd.ExecuteReader())
                                {
                                    if (budgetReader.Read())
                                    {
                                        if (decimal.TryParse(budgetReader["TotalAllocated"].ToString(), out decimal totalAllocated))
                                        {
                                            txtAnnualBudget.Text = string.Format(new System.Globalization.CultureInfo("en-IN"), "₹{0:N2}", totalAllocated);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteBudget")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, get the BudgetID before deleting the category
                    int budgetID;
                    using (SqlCommand cmd = new SqlCommand("SELECT BudgetMaster_BudgetID FROM BudgetCategories WHERE BudgetCategoryID = @CategoryID", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        budgetID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Then delete the category
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM BudgetCategories WHERE BudgetCategoryID = @CategoryID", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.ExecuteNonQuery();
                    }

                    // Finally, update the TotalSpent in BudgetMaster
                    UpdateTotalSpentInBudgetMaster(conn, budgetID);
                }
                LoadBudgetCategories();
            }
        }

        private void ClearInputs()
        {
            txtCategoryName.Text = "";
            txtAmountAllocated.Text = "";
            txtActualExpenditure.Text = "0";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }
    }
}