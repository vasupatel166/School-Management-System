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
    public partial class StandardMaster : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                ddlSearchStandard.Visible = false;
                
            }
        }

        private void populatestandardDropdown(string schoolId)
        {
            string query = "Select StandardID,StandardName from Standards where SchoolMaster_SchoolID='" + schoolId + "'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchStandard.Items.Clear();
                    ddlSearchStandard.Items.Add(new ListItem("-- Select Standard --", ""));
                    while (reader.Read())
                    {
                        ListItem item = new ListItem(reader["StandardName"].ToString(), reader["StandardID"].ToString());
                        ddlSearchStandard.Items.Add(item);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            saveStandard();
        }

        private void saveStandard()
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateStandardMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@StandardID", txtStandardID.Text));
                    cmd.Parameters.Add(new SqlParameter("@StandardName", txtStandardName.Text));
                    cmd.Parameters.Add(new SqlParameter("@StandardDesc", txtStandardDesc.Text));
                    cmd.Parameters.Add(new SqlParameter("@SchoolMaster_SchoolID", schoolID));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                       
                        if (string.IsNullOrEmpty(txtStandardID.Text))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Standard Created Successfully');", true);
                            ClearForm();

                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Standard Updated Successfully');", true);
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception and show an error message
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtStandardID.Text = string.Empty;
            txtStandardName.Text = string.Empty;
            txtStandardDesc.Text = string.Empty;
            ddlSearchStandard.Visible = false;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            ddlSearchStandard.Visible = true;
            populatestandardDropdown(schoolID);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

        protected void ddlSearchStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSearchStandard.SelectedValue))
            {
                LoadStandardDeatils(ddlSearchStandard.SelectedValue);
            }
        }

        private void LoadStandardDeatils(string StandardID)
        {
            //var context = HttpContext.Current;
            //string schoolID = context.Session["SchoolID"]?.ToString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetStandardDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StandardID", StandardID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtStandardID.Text = reader["StandardID"].ToString();
                        txtStandardName.Text = reader["StandardName"].ToString();
                        txtStandardDesc.Text= reader["StandardDesc"].ToString();
                    }
                }
            }
        }
    }
}