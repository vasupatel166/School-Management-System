using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Schoolnest
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnReg.Visible = false;  // Initially, the register button is hidden
            }
        }

        protected void ddlUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show the register button only if Admin is selected
            btnReg.Visible = (ddlUserType.SelectedValue == "A");
        }

        // Event handler for the login button
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Get the input from the form fields
            string schoolId = txtSchoolId.Text.Trim();
            string userType = ddlUserType.SelectedValue;
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Check if the fields are filled before validating
            if (string.IsNullOrEmpty(schoolId) || string.IsNullOrEmpty(userType) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowAlert("Please fill all the required fields.");
                return; 
            }

            // Validate the login credentials
            if (ValidateLogin(schoolId, userType, username, password))
            {
                // If valid, redirect to the Dashboard
                Response.Redirect("~/SuperAdmin/Dashboard.aspx");
            }
            else
            {
                // If invalid, show an error message
                ShowAlert("Invalid credentials. Please try again.");
            }
        }

        // Method to display an alert message in a Bootstrap modal
        private void ShowAlert(string message)
        {
            string script = $"document.getElementById('alertMessage').innerText = '{message.Replace("'", "\\'")}'; " +
                            "$('#alertModal').modal('show');";
            ClientScript.RegisterStartupScript(this.GetType(), "alertModalScript", script, true);
        }

        // Function to validate the login credentials by checking the database
        private bool ValidateLogin(string schoolId, string userType, string username, string password)
        {
            // Connection string from web.config
            string connectionString = ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserDataForLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the parameters for the stored procedure
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@RoleID", userType);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); 

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            return reader.HasRows; 
                        }
                    }
                    catch (Exception ex)
                    {                       
                        Console.WriteLine("Error during login: " + ex.Message);
                        return false; 
                    }
                }
            }
        }

        // Event handler for the cancel button
        protected void btnCancel_Click(object sender, EventArgs e)
        {           
            ClearFields();
        }

        // Method to clear input fields
        private void ClearFields()
        {
            txtSchoolId.Text = string.Empty;
            ddlUserType.SelectedIndex = 0; 
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }
    }
}
