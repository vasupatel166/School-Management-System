using System;
using System.Data;
using System.Data.SqlClient;

namespace Schoolnest
{
    public partial class Login : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;

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
                string schoolId = txtSchoolId.Text.Trim();
                string userType = ddlUserType.SelectedValue;
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (ValidateLogin(schoolId, userType, username, password))
                {
                    Session["SchoolID"] = schoolId;
                    Session["UserRole"] = userType;

                    switch (userType)
                    {
                        case "SA":
                            Response.Redirect("~/SuperAdmin/Dashboard.aspx");
                            break;
                        case "A":
                            Response.Redirect("~/Admin/Dashboard.aspx");
                            break;
                        case "T":
                            Response.Redirect("~/Teacher/Dashboard.aspx");
                            break;
                        default:
                            //Response.Redirect("~/Student/Dashboard.aspx");
                            break;
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
            // Set the stored procedure name based on the user type
            string databaseProcedure = userType == "SA" ? "GetSuperAdminByUsername" : "GetUserDataForLogin";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection opened successfully.");

                    // Construct the command for the stored procedure execution
                    using (SqlCommand cmd = new SqlCommand(databaseProcedure, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters based on user type
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

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"{password}, {reader["DecryptedPassword"]}");

                                if (password == reader["DecryptedPassword"].ToString())
                                {
                                    Session["Username"] = reader["Username"].ToString();
                                    Session["UserID"] = Convert.ToInt32(reader["UserID"]);
                                    return true;
                                }

                                return false;
                            }
                            else
                            {
                                rfvUsername.IsValid = false;
                                rfvUsername.Text = "No matching user found, SchoolID / Username is incorrect.";
                                System.Diagnostics.Debug.WriteLine("No matching user found in the database.");
                                return false;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle exceptions thrown during command execution
                    System.Diagnostics.Debug.WriteLine($"SqlException: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Error Number: {ex.Number}");
                    System.Diagnostics.Debug.WriteLine($"Error State: {ex.State}");
                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error during connection or command execution: " + ex.Message);
                    return false;
                }
            }
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
