using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Schoolnest
{
    public partial class schoolmaster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string connString = ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand("InsertUpdateSchoolMaster", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Generate the SchoolID if it's a new record
                if (string.IsNullOrEmpty(txtSchoolID.Text))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", DBNull.Value);  // Pass NULL to get new ID
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SchoolID", txtSchoolID.Text);  // Existing ID for update
                }

                // Add parameters for school details
                cmd.Parameters.AddWithValue("@School_Name", txtSchoolName.Text);
                cmd.Parameters.AddWithValue("@School_Add1", txtSchoolAdd1.Text);
                cmd.Parameters.AddWithValue("@School_Add2", txtSchoolAdd2.Text);
                cmd.Parameters.AddWithValue("@School_Add3", txtSchoolAdd3.Text);
                cmd.Parameters.AddWithValue("@Phone_No", txtPhoneNo.Text);
                cmd.Parameters.AddWithValue("@School_Email", txtSchoolEmail.Text);
                cmd.Parameters.AddWithValue("@School_Website", txtSchoolWebsite.Text);
                cmd.Parameters.AddWithValue("@School_Type", txtSchoolType.Text);
                cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year.ToString());

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    if (string.IsNullOrEmpty(txtSchoolID.Text))
                    {
                        Response.Write("School inserted successfully!");
                    }
                    else
                    {
                        Response.Write("School updated successfully!");
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    Response.Write("Error: " + ex.Message);
                }
            }
        }
    }
}