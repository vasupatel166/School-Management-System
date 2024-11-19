using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class SectionMaster : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolID"].ToString();
                ddlSearchSection.Visible = false;
            }
        }

        private void PopulateSectionDropdown(string schoolId)
        {
            string query = "SELECT SectionID, SectionName FROM Sections WHERE SchoolMaster_SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchSection.Items.Clear();
                    ddlSearchSection.Items.Add(new ListItem("-- Select Section --", ""));
                    while (reader.Read())
                    {
                        ListItem item = new ListItem(reader["SectionName"].ToString(), reader["SectionID"].ToString());
                        ddlSearchSection.Items.Add(item);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveSection();
        }

        private void SaveSection()
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateSectionMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SectionID", txtSectionCode.Text));
                    cmd.Parameters.Add(new SqlParameter("@SectionName", txtSectionName.Text));
                    cmd.Parameters.Add(new SqlParameter("@SectionDesc", txtSectionDesc.Text));
                    cmd.Parameters.Add(new SqlParameter("@SchoolMaster_SchoolID", schoolID));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (string.IsNullOrEmpty(txtSectionCode.Text))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Section Created Successfully');", true);
                            ClearForm();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Section Updated Successfully');", true);
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
            txtSectionCode.Text=string.Empty;
            txtSectionName.Text = string.Empty;
            txtSectionDesc.Text = string.Empty;
            ddlSearchSection.Visible = false;
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            ddlSearchSection.Visible = true;
            PopulateSectionDropdown(schoolID);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

        protected void ddlSearchSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSearchSection.SelectedValue))
            {
                LoadSectionDetails(ddlSearchSection.SelectedValue);
            }
        }

        private void LoadSectionDetails(string SectionID)
        {
            string schoolID = Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetSectionDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SectionID", SectionID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtSectionCode.Text = reader["SectionID"].ToString();
                        txtSectionName.Text = reader["SectionName"].ToString();
                        txtSectionDesc.Text = reader["SectionDesc"].ToString();
                    }
                }
            }
        }
    }
}