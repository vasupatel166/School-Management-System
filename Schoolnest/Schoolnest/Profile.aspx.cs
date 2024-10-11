using System;
using System.Data.SqlClient;

namespace Schoolnest
{
    public partial class Profile : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserProfile();
            }
        }

        // Event handler for the Edit Profile button click
        protected void btnEditProfile_Click(object sender, EventArgs e)
        {
            // Enable edit mode
            ToggleEditMode(true);
        }

        // Event handler for the Save Changes button click
        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            // Save profile changes
            SaveProfileChanges();
            // Disable edit mode after saving
            ToggleEditMode(false);
        }

        // Method to toggle between read-only and edit mode
        private void ToggleEditMode(bool enable)
        {
            // SuperAdmin Editable Fields
            txtSuperAdminEmail.ReadOnly = !enable;
            txtSuperAdminMobile.ReadOnly = !enable;
            txtSuperAdminAddress.ReadOnly = !enable;

            // Admin Editable Fields
            txtAdminEmail.ReadOnly = !enable;
            txtAdminMobile.ReadOnly = !enable;
            txtAdminAddress.ReadOnly = !enable;

            // Teacher Editable Fields
            txtTeacherEmail.ReadOnly = !enable;
            txtTeacherMobile.ReadOnly = !enable;
            txtTeacherAddress.ReadOnly = !enable;

            // Student Editable Fields
            txtStudentEmail.ReadOnly = !enable;
            txtStudentMobile.ReadOnly = !enable;
            txtStudentAddress.ReadOnly = !enable;

            // Toggle visibility of Save and Edit buttons
            btnSaveProfile.Visible = enable;
            btnEditProfile.Visible = !enable;
        }

        // Method to save profile changes (implement your own logic)
        private void SaveProfileChanges()
        {
            // Add your logic to update the database with profile changes
        }

        // Load user profile data based on the role
        private void LoadUserProfile()
        {
            string userRole = GetUserRole();
            string userId = GetUserID();

            switch (userRole)
            {
                case "SuperAdmin":
                    pnlSuperAdminProfile.Visible = true;
                    LoadSuperAdminProfile(userId);
                    break;
                case "Admin":
                    pnlAdminProfile.Visible = true;
                    LoadAdminProfile(userId);
                    break;
                case "Teacher":
                    pnlTeacherProfile.Visible = true;
                    LoadTeacherProfile(userId);
                    break;
                case "Student":
                    pnlStudentProfile.Visible = true;
                    LoadStudentProfile(userId);
                    break;
            }
        }

        // Get user role from session (implement your own logic)
        private string GetUserRole()
        {
            return Session["UserRole"]?.ToString() ?? "Admin";
        }

        // Get user ID from session (implement your own logic)
        private string GetUserID()
        {
            return Session["UserID"]?.ToString();
        }

        // Load SuperAdmin profile data
        private void LoadSuperAdminProfile(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Name, Email, MobileNumber, Address, ProfileImage FROM SuperAdmin WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtSuperAdminName.Text = reader["Name"].ToString();
                        txtSuperAdminEmail.Text = reader["Email"].ToString();
                        txtSuperAdminMobile.Text = reader["MobileNumber"].ToString();
                        txtSuperAdminAddress.Text = reader["Address"].ToString();
                        imgSuperAdminProfile.ImageUrl = "~/assets/img/user-profile-img/" + reader["ProfileImage"].ToString();
                    }
                }
            }
        }

        // Load Admin profile data
        private void LoadAdminProfile(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Name, Email, MobileNumber, Address, ProfileImage FROM Admin WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtAdminName.Text = reader["Name"].ToString();
                        txtAdminEmail.Text = reader["Email"].ToString();
                        txtAdminMobile.Text = reader["MobileNumber"].ToString();
                        txtAdminAddress.Text = reader["Address"].ToString();
                        imgAdminProfile.ImageUrl = "~/assets/img/user-profile-img/" + reader["ProfileImage"].ToString();
                    }
                }
            }
        }

        // Load Teacher profile data
        private void LoadTeacherProfile(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Teacher_Firstname + ' ' + Teacher_Lastname AS TeacherName, Gender, Teacher_DOB, Teacher_Email, Teacher_MobileNumber, Teacher_Address1, ProfileImage FROM TeacherMaster WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtTeacherName.Text = reader["TeacherName"].ToString();
                        txtTeacherGender.Text = reader["Gender"].ToString();
                        txtTeacherDOB.Text = reader["Teacher_DOB"].ToString();
                        txtTeacherEmail.Text = reader["Teacher_Email"].ToString();
                        txtTeacherMobile.Text = reader["Teacher_MobileNumber"].ToString();
                        txtTeacherAddress.Text = reader["Teacher_Address1"].ToString();
                        imgTeacherProfile.ImageUrl = "~/assets/img/user-profile-img/" + reader["ProfileImage"].ToString();
                    }
                }
            }
        }

        // Load Student profile data
        private void LoadStudentProfile(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Student_FirstName + ' ' + Student_LastName AS StudentName, Student_Gender, Student_DateOfBirth, Student_EmailID, Student_MobileNumber, Student_Address1, ProfileImage FROM StudentMaster WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtStudentName.Text = reader["StudentName"].ToString();
                        txtStudentGender.Text = reader["Student_Gender"].ToString();
                        txtStudentDOB.Text = reader["Student_DateOfBirth"].ToString();
                        txtStudentEmail.Text = reader["Student_EmailID"].ToString();
                        txtStudentMobile.Text = reader["Student_MobileNumber"].ToString();
                        txtStudentAddress.Text = reader["Student_Address1"].ToString();
                        imgStudentProfile.ImageUrl = "~/assets/img/user-profile-img/" + reader["ProfileImage"].ToString();
                    }
                }
            }
        }
    }
}
