using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Schoolnest.SuperAdmin
{
    public partial class RegisterAdmin : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SelectedAdminID = "";
        public string profileImagePath = null;

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
                using (SqlCommand cmd = new SqlCommand("SELECT AdminID, Firstname, Lastname, SchoolMaster_SchoolID, IsActive FROM Admin", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string displayName = $"{reader["Firstname"]} {reader["Lastname"]} ({reader["SchoolMaster_SchoolID"]})";
                        ListItem listItem = new ListItem(displayName, reader["AdminID"].ToString());

                        ddlSearchAdmin.Items.Add(listItem);
                    }
                }
            }

            // Insert a default option at the beginning of the dropdown list
            ddlSearchAdmin.Items.Insert(0, new ListItem("Select Admin", ""));
        }

        // Load schools into dropdown
        private void LoadSchools()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SchoolID, SchoolName, IsActive FROM SchoolMaster", conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string displayName = $"{reader["SchoolName"]} ({reader["SchoolID"]})";
                        ListItem listItem = new ListItem(displayName, reader["SchoolID"].ToString());

                        // Disable the ListItem if IsActive is 0
                        if (Convert.ToInt32(reader["IsActive"]) == 0)
                        {
                            listItem.Attributes.Add("disabled", "disabled");
                        }

                        ddlSchool.Items.Add(listItem);
                    }
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
            bool isActive = chkIsActive.Checked;
            int adminID = string.IsNullOrEmpty(ddlSearchAdmin.SelectedValue) ? 0 : int.Parse(ddlSearchAdmin.SelectedValue);

            // Check if the profile image file is uploaded
            if (fileProfileImage.HasFile)
            {
                string extension = Path.GetExtension(fileProfileImage.FileName).ToLower();

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    try
                    {
                        // Generate a unique 3-digit number prefix for the image file name
                        Random random = new Random();
                        string uniquePrefix = random.Next(100, 1000).ToString();
                        string fileName = uniquePrefix + "_" + fileProfileImage.FileName;
                        string filePath = "~/assets/img/user-profile-img/admin/" + fileName;

                        // Ensure the directory exists before saving the file
                        string directoryPath = Server.MapPath("~/assets/img/user-profile-img/admin/");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Save the file
                        fileProfileImage.SaveAs(Server.MapPath(filePath));
                        profileImagePath = "admin/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('File upload failed: {ex.Message}');", true);
                        return;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Error", "alert('Only image files (jpg, jpeg, png, gif) are allowed.');", true);
                    return;
                }
            }

            try
            {
                int userID = 0;

                if (adminID == 0)
                {
                    // Execute the InsertUserMaster stored procedure to get UserID
                    userID = ExecuteInsertUserMaster(firstName, lastName, "A", schoolID);
                }

                // When updating an admin, keep the existing image if a new one is not uploaded
                if (adminID != 0 && string.IsNullOrEmpty(profileImagePath))
                {
                    profileImagePath = GetExistingProfileImagePath(adminID);
                }

                // Insert or update Admin data using InsertUpdateAdminMaster procedure
                ExecuteInsertUpdateAdminMaster(adminID, firstName, lastName, email, mobileNumber, gender, profileImagePath, userID, schoolID, isActive);
                ClientScript.RegisterStartupScript(this.GetType(), "Success", "alert('Admin details saved successfully!');", true);
                ResetForm();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('Error: {ex.Message}');", true);
            }
        }

        // Retrieve the existing profile image path for an admin
        private string GetExistingProfileImagePath(int adminID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ProfileImage FROM Admin WHERE AdminID = @AdminID", conn))
                {
                    cmd.Parameters.AddWithValue("@AdminID", adminID);
                    conn.Open();

                    return cmd.ExecuteScalar()?.ToString();
                }
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
        private void ExecuteInsertUpdateAdminMaster(int adminID, string firstName, string lastName, string email, string mobileNumber, string gender, string profileImage, int userID, string schoolID,bool isActive)
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
                    cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 :0);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            ddlGender.SelectedIndex = 0;
            txtEmail.Text = "";
            txtMobileNumber.Text = "";
            ddlSchool.SelectedIndex = 0;
            ddlSearchAdmin.SelectedIndex = 0;
            profileImagePath = null; // Reset the image path variable

            Response.Redirect("~/SuperAdmin/RegisterAdmin.aspx");
        }

        protected void ddlSearchAdmin_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAdminID = ddlSearchAdmin.SelectedValue;

            if (!string.IsNullOrEmpty(SelectedAdminID) && SelectedAdminID != "Select Admin")
            {
                // Load the selected admin details into the form fields
                LoadAdminDetails(SelectedAdminID);
            }
            else
            {
                // Reset the form if no admin is selected
                ResetForm();
            }
        }

        private void LoadAdminDetails(string AdminID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Admin WHERE AdminID = @AdminID", conn))
                {
                    cmd.Parameters.AddWithValue("@AdminID", AdminID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate form fields with data
                            txtFirstName.Text = reader["Firstname"].ToString();
                            txtLastName.Text = reader["Lastname"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtMobileNumber.Text = reader["Mobile_Number"].ToString();
                            ddlGender.SelectedValue = reader["Gender"].ToString();
                            ddlSchool.SelectedValue = reader["SchoolMaster_SchoolID"].ToString();
                            profileImagePath = reader["ProfileImage"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                        }
                    }
                }
            }
        }
    }
}
