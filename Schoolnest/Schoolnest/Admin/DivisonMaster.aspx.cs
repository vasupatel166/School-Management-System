using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class DivisonMaster : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            schoolId = Session["SchoolID"].ToString();
            
            if (!IsPostBack)
            {
                ddlSearchDivision.Visible = false;
            }
        }

        private void PopulateDivisionDropdown()
        {
            string query = "SELECT DivisionID, DivisionName FROM Divisions WHERE SchoolMaster_SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchDivision.Items.Clear();
                    ddlSearchDivision.Items.Add(new ListItem("-- Select Division --", ""));
                    while (reader.Read())
                    {
                        ListItem item = new ListItem(reader["DivisionName"].ToString(), reader["DivisionID"].ToString());
                        ddlSearchDivision.Items.Add(item);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveDivision();
        }

        private void SaveDivision()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateDivisionMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", txtDivisionID.Text));
                    cmd.Parameters.Add(new SqlParameter("@DivisionName", txtDivisionName.Text));
                    cmd.Parameters.Add(new SqlParameter("@SchoolMaster_SchoolID", schoolId));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (string.IsNullOrEmpty(txtDivisionID.Text))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Division Created Successfully');", true);
                            ClearForm();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Division Updated Successfully');", true);
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtDivisionID.Text = string.Empty;
            txtDivisionName.Text = string.Empty;
            ddlSearchDivision.Visible = false;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ddlSearchDivision.Visible = true;
            PopulateDivisionDropdown();
        }

        protected void ddlSearchDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSearchDivision.SelectedValue))
            {
                LoadDivisionDetails(ddlSearchDivision.SelectedValue);
            }
        }

        private void LoadDivisionDetails(string divisionID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDivisionDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DivisionID", divisionID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtDivisionID.Text = reader["DivisionID"].ToString();
                        txtDivisionName.Text = reader["DivisionName"].ToString();
                    }
                }
            }
        }
    }
}