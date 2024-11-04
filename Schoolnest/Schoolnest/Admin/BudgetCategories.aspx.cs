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
            }
        }

        private void LoadBudgetYears()
        {
            // Load distinct Academic Years from BudgetMaster
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT BudgetID, AcademicYear FROM BudgetMaster WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlBudgetYear.DataSource = reader;
                    ddlBudgetYear.DataTextField = "AcademicYear";
                    ddlBudgetYear.DataValueField = "BudgetID";
                    ddlBudgetYear.DataBind();
                }
            }
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            // Create a new row in the specified format
            var rowPanel = new Panel { CssClass = "row" };

            // Category Name field
            var col1 = new Panel { CssClass = "col-md-4" };
            var formGroup1 = new Panel { CssClass = "form-group" };
            var txtCategoryName = new TextBox { CssClass = "form-control", ID = $"txtCategoryName_{categoryContainer.Controls.Count}" };
            txtCategoryName.Attributes["placeholder"] = "Category Name";
            formGroup1.Controls.Add(txtCategoryName);
            col1.Controls.Add(formGroup1);
            rowPanel.Controls.Add(col1);

            // Amount Allocated field
            var col2 = new Panel { CssClass = "col-md-4" };
            var formGroup2 = new Panel { CssClass = "form-group" };
            var txtAmountAllocated = new TextBox { CssClass = "form-control", ID = $"txtAmountAllocated_{categoryContainer.Controls.Count}", TextMode = TextBoxMode.Number };
            txtAmountAllocated.Attributes["placeholder"] = "Amount Allocated";
            formGroup2.Controls.Add(txtAmountAllocated);
            col2.Controls.Add(formGroup2);
            rowPanel.Controls.Add(col2);

            // Actual Expenditure field (disabled by default)
            var col3 = new Panel { CssClass = "col-md-4" };
            var formGroup3 = new Panel { CssClass = "form-group" };
            var txtActualExpenditure = new TextBox { CssClass = "form-control", ID = $"txtActualExpenditure_{categoryContainer.Controls.Count}", TextMode = TextBoxMode.Number, Enabled = false };
            txtActualExpenditure.Attributes["placeholder"] = "Actual Expenditure";
            formGroup3.Controls.Add(txtActualExpenditure);
            col3.Controls.Add(formGroup3);
            rowPanel.Controls.Add(col3);

            categoryContainer.Controls.Add(rowPanel);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int budgetID = int.Parse(ddlBudgetYear.SelectedValue);
            decimal totalSpent = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (Control row in categoryContainer.Controls)
                {
                    var txtCategoryName = row.FindControl($"txtCategoryName_{categoryContainer.Controls.IndexOf(row)}") as TextBox;
                    var txtAmountAllocated = row.FindControl($"txtAmountAllocated_{categoryContainer.Controls.IndexOf(row)}") as TextBox;
                    var txtActualExpenditure = row.FindControl($"txtActualExpenditure_{categoryContainer.Controls.IndexOf(row)}") as TextBox;

                    if (txtCategoryName != null && txtAmountAllocated != null && txtActualExpenditure != null)
                    {
                        decimal amountAllocated = decimal.Parse(txtAmountAllocated.Text);
                        decimal actualExpenditure = decimal.Parse(txtActualExpenditure.Text);
                        totalSpent += actualExpenditure;

                        using (SqlCommand cmd = new SqlCommand("InsertUpdateBudgetCategory", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@BudgetCategoryName", txtCategoryName.Text);
                            cmd.Parameters.AddWithValue("@AmountAllocated", amountAllocated);
                            cmd.Parameters.AddWithValue("@ActualExpenditure", actualExpenditure);
                            cmd.Parameters.AddWithValue("@BudgetMaster_BudgetID", budgetID);
                            cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Update TotalSpent in BudgetMaster
                using (SqlCommand cmd = new SqlCommand("UPDATE BudgetMaster SET TotalSpent = @TotalSpent WHERE BudgetID = @BudgetID", conn))
                {
                    cmd.Parameters.AddWithValue("@TotalSpent", totalSpent);
                    cmd.Parameters.AddWithValue("@BudgetID", budgetID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/BudgetCategories.aspx");
        }
    }
}
