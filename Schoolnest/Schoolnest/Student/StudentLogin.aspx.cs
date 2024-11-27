using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Student
{
    public partial class StudentLogin : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] != null)
            {
                Response.Redirect("~/Logout.aspx");
            }
        }

        protected void StudentBtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (ValidateLogin(username, password))
                {
                    Session["UserRole"] = "S";
                    Response.Redirect("~/Student/Dashboard.aspx");
                }
                else
                {
                    StudentRfvPassword.IsValid = false;
                    StudentRfvPassword.Text = "Password Incorrect";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while login: " + ex.Message);
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("GetUserDataForLogin", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@RoleID", "S");
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (password == reader["DecryptedPassword"].ToString())
                                {
                                    Session["SchoolID"] = reader["SchoolMaster_SchoolID"].ToString();
                                    Session["Username"] = reader["Username"].ToString();
                                    Session["UserID"] = Convert.ToInt32(reader["UserID"]);
                                    return true;
                                }

                                return false;
                            }
                            else
                            {
                                StudentRfvUsername.IsValid = false;
                                StudentRfvUsername.Text = "No matching user found, Username is incorrect.";
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
    }
}