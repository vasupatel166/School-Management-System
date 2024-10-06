using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Security.Cryptography;
using System.Text;

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

                    if (userType == "SA")
                    {
                        cmd.Parameters.AddWithValue("@SuperAdminUsername", username);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                        cmd.Parameters.AddWithValue("@RoleID", userType);
                        cmd.Parameters.AddWithValue("@Username", username);
                    }
                        cmd.Parameters.AddWithValue("@UserPassword", password);

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Fetch the hashed password from the database
                                string storedHashedPassword = reader["Password"].ToString();

                                string DBHashedPassword = reader["DBHashedPassword"].ToString();

                                if (storedHashedPassword != DBHashedPassword)
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }

                                // Verify the password using SHA-256
                                //return VerifyPassword(password, storedHashedPassword);
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

        // Method to verify the entered password against the stored SQL Server hash
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            try
            {
                // Remove the '0x' prefix from the stored hash if present
                if (storedHash.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    storedHash = storedHash.Substring(2);
                }

                // Hash the entered password
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Convert the password string to bytes
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(enteredPassword);

                    // Compute the hash
                    byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

                    // Convert the hashed bytes to a string for comparison
                    string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "");

                    // Compare the computed hash with the stored hash (case-insensitive)
                    return string.Equals(hashedPassword, storedHash, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in VerifyPassword: {ex.Message}");
                return false;
            }
        }

        //// Method to hash the password using SHA-256 and return the byte array
        //public static byte[] HashPasswordSHA256(string password)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        // Convert the input string to a byte array
        //        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        //        // Compute the hash of the input byte array
        //        return sha256.ComputeHash(passwordBytes);
        //    }
        //}

        //// Helper method to convert a hexadecimal string to a byte array
        //public static byte[] HexStringToByteArray(string hex)
        //{
        //    int length = hex.Length;
        //    byte[] bytes = new byte[length / 2];
        //    for (int i = 0; i < length; i += 2)
        //    {
        //        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        //    }
        //    return bytes;
        //}

        //// Helper method to compare two byte arrays for equality
        //public static bool CompareByteArrays(byte[] array1, byte[] array2)
        //{
        //    if (array1.Length != array2.Length)
        //    {
        //        return false;
        //    }

        //    for (int i = 0; i < array1.Length; i++)
        //    {
        //        if (array1[i] != array2[i])
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

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
            if (ddlUserType.SelectedValue == "SA")
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
