using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Security.Cryptography;

namespace Schoolnest
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] != null)
                {
                    Response.Redirect("~/Logout.aspx");
                }

                rfvUsername.IsValid = true;
                rfvPassword.IsValid = true;
            }
        }

        // Event handler for the login button
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {

                // Get the input from the form fields
                string schoolId = txtSchoolId.Text.Trim();
                string userType = ddlUserType.SelectedValue;
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                // Validate the login credentials
                if (ValidateLogin(schoolId, userType, username, password))
                {
                    // Create session variables for logged-in user details
                    Session["SchoolID"] = schoolId;
                    Session["UserRole"] = userType;
                    Session["Username"] = username;

                    // If valid, redirect to the Dashboard according to user type
                    if (userType == "SA") // superadmin
                    {
                        Response.Redirect("~/SuperAdmin/Dashboard.aspx");
                    }
                    else if (userType == "A") // admin
                    {
                        Response.Redirect("~/Admin/Dashboard.aspx");
                    }
                    else if (userType == "T") // teacher
                    {
                        Response.Redirect("~/Teacher/Dashboard.aspx");
                    }
                    else // Student
                    {
                        Response.Redirect("~/Student/Dashboard.aspx");
                    }
                }
                else
                {
                    rfvPassword.IsValid = false;
                    rfvPassword.Text = "Password Incorrect";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while login: " + ex.Message);
            }

        }

        private bool ValidateLogin(string schoolId, string userType, string username, string password)
        {
            // Connection string from web.config
            string connectionString = Global.ConnectionString;

            string databaseProcedure = "";

            if (userType == "SA") // superadmin
            {
                databaseProcedure = "GetSuperAdminByUsername";
            }
            else 
            {
                databaseProcedure = "GetUserDataForLogin";
            }
            
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(databaseProcedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if(userType == "SA")
                    {
                        cmd.Parameters.AddWithValue("@SuperAdminUsername", username);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                        cmd.Parameters.AddWithValue("@RoleID", userType);
                        cmd.Parameters.AddWithValue("@Username", username);
                    }

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Fetch the hashed password from the database
                                string storedHashedPassword = reader["Password"].ToString();

                                // Verify the password using PBKDF2 (Rfc2898DeriveBytes)
                                return VerifyPassword(password, storedHashedPassword);
                            }
                            else
                            {
                                rfvUsername.IsValid = false;
                                rfvUsername.Text = "Username is incorrect";
                                return false;
                            }
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

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Extract bytes from the stored hash
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hash the entered password using the same salt and 10000 iterations
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] enteredHash = pbkdf2.GetBytes(20); // Get the 20-byte hash

            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != enteredHash[i])
                    return false; // Mismatch

            return true; // Password matches
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtSchoolId.Text = string.Empty;
            ddlUserType.SelectedIndex = 0; 
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        protected void ddlUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlUserType.SelectedValue == "SA")
            {
                txtSchoolId.Text = "superadmin";
            }
            else
            {
                txtSchoolId.Text = "";
            }
        }
    }
}
