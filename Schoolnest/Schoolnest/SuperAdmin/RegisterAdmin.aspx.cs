using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.SuperAdmin
{
    public partial class RegisterAdmin : System.Web.UI.Page
    {

        // Connection string (You can get it from Global.ConnectionString or Web.config)
        private string connectionString = Global.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSchools();
                LoadAdminSearchDropdown();
            }
        }

        // Load the dropdown with existing admins
        private void LoadAdminSearchDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT AdminID, First_Name, Last_Name, SchoolMaster_SchoolID FROM Admin", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlSearchAdmin.DataSource = reader;
                    ddlSearchAdmin.DataTextField = "First_Name";
                    ddlSearchAdmin.DataValueField = "AdminID";
                    ddlSearchAdmin.DataBind();

                    while (reader.Read())
                    {
                        string displayName = $"{reader["First_Name"]} {reader["Last_Name"]} ({reader["SchoolMaster_SchoolID"]})";
                        ddlSearchAdmin.Items.Add(new ListItem(displayName, reader["AdminID"].ToString()));
                    }
                }
            }
            ddlSearchAdmin.Items.Insert(0, new ListItem("Select Admin", ""));
        }

        // Load schools into dropdown
        private void LoadSchools()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SchoolID, SchoolName FROM SchoolMaster", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlSchool.DataSource = reader;
                    ddlSchool.DataTextField = "SchoolName";
                    ddlSchool.DataValueField = "SchoolID";
                    ddlSchool.DataBind();
                }
            }
            ddlSchool.Items.Insert(0, new ListItem("Select School", ""));
        }

        // Handle form submission for adding/updating an Admin
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Collect form data
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string gender = ddlGender.SelectedValue;
            string email = txtEmail.Text.Trim();
            string mobileNumber = txtMobileNumber.Text.Trim();
            string schoolID = ddlSchool.SelectedValue;
            int adminID = string.IsNullOrEmpty(ddlSearchAdmin.SelectedValue) ? 0 : int.Parse(ddlSearchAdmin.SelectedValue);

            // Get the profile image file path if uploaded
            string profileImagePath = null;
            if (fileProfileImage.HasFile)
            {
                string filePath = "~/assets/img/user-profile-img/admin/" + fileProfileImage.FileName;
                fileProfileImage.SaveAs(Server.MapPath(filePath));
                profileImagePath = "admin/" + fileProfileImage.FileName;
            }

            try
            {
                // Execute the InsertUserMaster stored procedure to get UserID
                int userID = ExecuteInsertUserMaster(firstName, lastName, "A", schoolID);

                if (userID > 0)
                {
                    // Insert or update Admin data using InsertUpdateAdminMaster procedure
                    ExecuteInsertUpdateAdminMaster(adminID, firstName, lastName, email, mobileNumber, gender, profileImagePath, userID, schoolID);
                    ClientScript.RegisterStartupScript(this.GetType(), "Success", "alert('Admin details saved successfully!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Error", "alert('Failed to create or find UserID. Please try again.');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('Error: {ex.Message}');", true);
            }
        }

        // Execute InsertUserMaster stored procedure and return the UserID
        private int ExecuteInsertUserMaster(string firstName, string lastName, string roleID, string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUserMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@RoleMaster_RoleID", roleID);
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", schoolID);

                    conn.Open();
                    object result = cmd.ExecuteScalar(); // Use ExecuteScalar to get the UserID directly
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // Execute InsertUpdateAdminMaster stored procedure
        private void ExecuteInsertUpdateAdminMaster(int adminID, string firstName, string lastName, string email, string mobileNumber, string gender, string profileImage, int userID, string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUpdateAdminMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AdminID", adminID == 0 ? DBNull.Value : (object)adminID);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@MobileNumber", mobileNumber);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@ProfileImage", string.IsNullOrEmpty(profileImage) ? DBNull.Value : (object)profileImage);
                    cmd.Parameters.AddWithValue("@UserMaster_UserID", userID);
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", schoolID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // Clear all form fields
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            ddlGender.SelectedIndex = 0;
            txtEmail.Text = "";
            txtMobileNumber.Text = "";
            ddlSchool.SelectedIndex = 0;
            ddlSearchAdmin.SelectedIndex = 0;
        }
    }
}