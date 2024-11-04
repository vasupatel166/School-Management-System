using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class BudgetMaster : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;
        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadBudgets();
            }
        }

        private void LoadBudgets()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BudgetID, BudgetName, TotalAllocated, TotalSpent, (TotalAllocated - TotalSpent) AS RemainingAmount, Status, AcademicYear FROM BudgetMaster WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvBudgets.DataSource = dt;
                    gvBudgets.DataBind();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("InsertUpdateBudgetMaster", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BudgetID", ViewState["BudgetID"] ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BudgetName", txtBudgetName.Text);
                        cmd.Parameters.AddWithValue("@TotalAllocated", Convert.ToDecimal(txtTotalAllocated.Text));
                        cmd.Parameters.AddWithValue("@TotalSpent", 0.00m); // Initial value for TotalSpent is zero
                        cmd.Parameters.AddWithValue("@Status", chkIsActive.Checked);
                        cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text);
                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", Session["SchoolID"]?.ToString());

                        cmd.ExecuteNonQuery();
                        ViewState["BudgetID"] = null;
                        ClearForm();
                        LoadBudgets();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Check if the error is a duplicate Academic Year error
                if (ex.Message.Contains("A budget entry for this academic year already exists."))
                {
                    // Display an error message on the form
                    lblErrorMessage.Text = "A budget entry for this academic year already exists.";
                    lblErrorMessage.Visible = true;
                }
                else
                {
                    // Handle other SQL exceptions
                    lblErrorMessage.Text = "An error occurred while saving the budget. Please try again.";
                    lblErrorMessage.Visible = true;
                }
            }
        }


        private void ClearForm()
        {
            txtBudgetName.Text = string.Empty;
            txtTotalAllocated.Text = string.Empty;
            txtAcademicYear.Text = string.Empty;
            chkIsActive.Checked = true;
            ViewState["BudgetID"] = null;
        }

        protected void gvBudgets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditBudget")
            {
                int budgetId = Convert.ToInt32(e.CommandArgument);
                LoadBudgetForEdit(budgetId);
            }
        }

        private void LoadBudgetForEdit(int budgetId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BudgetID, BudgetName, TotalAllocated, TotalSpent, Status, AcademicYear FROM BudgetMaster WHERE BudgetID = @BudgetID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@BudgetID", budgetId);
                    cmd.Parameters.AddWithValue("@SchoolID", Session["SchoolID"]?.ToString());
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtBudgetName.Text = reader["BudgetName"].ToString();
                        txtTotalAllocated.Text = reader["TotalAllocated"].ToString();
                        txtAcademicYear.Text = reader["AcademicYear"].ToString();
                        chkIsActive.Checked = Convert.ToBoolean(reader["Status"]);
                        ViewState["BudgetID"] = budgetId;
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}
