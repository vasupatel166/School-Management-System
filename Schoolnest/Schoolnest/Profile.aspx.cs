using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace Schoolnest
{
    public partial class Profile : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        public string SchoolID;
        public string RoleID;
        public string Username;
        private bool isEditMode = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get session values
            RoleID = Session["UserRole"]?.ToString();
            if (RoleID != "SA")
            {
                SchoolID = Session["SchoolID"]?.ToString();
            }
            Username = Session["Username"]?.ToString();

            if (!IsPostBack)
            {

                LoadStates();

                if (!string.IsNullOrEmpty(RoleID) && !string.IsNullOrEmpty(Username))
                {
                    LoadUserProfileDetails();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Session values are missing. User might not be logged in.");
                }

                if(RoleID == "SA" || RoleID == "A")
                {
                    LocationFields.Visible = false;
                }
            }
        }

        // Load profile details dynamically from JSON configuration
        private void LoadUserProfileDetails()
        {
            if (!string.IsNullOrEmpty(RoleID) && !string.IsNullOrEmpty(Username))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("GetUserDetailsByRole", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Username", Username);
                            cmd.Parameters.AddWithValue("@RoleID", RoleID);

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string profileJson = string.Empty;

                                    // Create JSON object based on Role
                                    if (RoleID == "SA")
                                    {
                                        profileJson = GetSuperAdminJson(reader);
                                    }
                                    else if (RoleID == "A")
                                    {
                                        profileJson = GetAdminJson(reader);
                                    }
                                    else if (RoleID == "T")
                                    {
                                        profileJson = GetTeacherJson(reader);
                                    }
                                    else if (RoleID == "S")
                                    {
                                        profileJson = GetStudentJson(reader);
                                    }

                                    // Log the JSON for debugging purposes
                                    System.Diagnostics.Debug.WriteLine("Generated Profile JSON: " + profileJson);

                                    // Generate form fields from JSON
                                    RenderProfileFields(profileJson, isEditMode);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("User Data Not Found!!!!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error while fetching profile details: " + ex.Message);
                    }
                }
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            // Retrieve new password and confirm password from textboxes
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (UpdatePasswordInDatabase(newPassword))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "PasswordChanged", "alert('Password changed successfully!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "PasswordChangeFailed", "alert('Password change failed!');", true);
            }
        }

        private bool UpdatePasswordInDatabase(string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Assuming you have a stored procedure to update the user's password
                    using (SqlCommand cmd = new SqlCommand("UpdateUserPassword", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the stored procedure
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@RoleID", RoleID);
                        cmd.Parameters.AddWithValue("@NewPassword", password);

                        // Only add @SchoolID if the user is not 'SA' (Super Admin)
                        if (RoleID != "SA" && !string.IsNullOrEmpty(SchoolID))
                        {
                            cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        }

                        // Execute the stored procedure
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Return true if the password was successfully updated
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error updating password: " + ex.Message);
                    return false;
                }
            }
        }

        // Helper method to safely retrieve values from SqlDataReader
        private string GetSafeField(SqlDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? reader[fieldName].ToString() : string.Empty;
        }

        private string GetSuperAdminJson(SqlDataReader reader)
        {
            var profileData = new
            {
                fields = new[]
                {
                    new { label = "Name", value = GetSafeField(reader, "Name"), editable = false },
                    new { label = "Email", value = GetSafeField(reader, "Email"), editable = true },
                    new { label = "Profile Image", value = GetSafeField(reader, "ProfileImage"), editable = true }
                }
            };

            return JsonConvert.SerializeObject(profileData);
        }

        private string GetAdminJson(SqlDataReader reader)
        {
            var profileData = new
            {
                fields = new[]
                {
                    new { label = "First Name", value = GetSafeField(reader, "Firstname"), editable = false },
                    new { label = "Last Name", value = GetSafeField(reader, "Lastname"), editable = false },
                    new { label = "Email", value = GetSafeField(reader, "Email"), editable = true },
                    new { label = "Mobile Number", value = GetSafeField(reader, "Mobile_Number"), editable = true },
                    new { label = "Profile Image", value = GetSafeField(reader, "ProfileImage"), editable = false }
                }
            };

            return JsonConvert.SerializeObject(profileData);
        }

        private string GetTeacherJson(SqlDataReader reader)
        {
            var profileData = new
            {
                fields = new[]
                {
                    new { label = "First Name", value = GetSafeField(reader, "Teacher_Firstname"), editable = false },
                    new { label = "Last Name", value = GetSafeField(reader, "Teacher_Lastname"), editable = false },
                    new { label = "Type", value = GetSafeField(reader, "Teacher_Type"), editable = false },
                    new { label = "Appointment Type", value = GetSafeField(reader, "Teacher_TypeOfAppointment"), editable = false },
                    new { label = "Email", value = GetSafeField(reader, "Teacher_Email"), editable = true },
                    new { label = "Mobile Number", value = GetSafeField(reader, "Teacher_MobileNumber"), editable = true },
                    new { label = "Address 1", value = GetSafeField(reader, "Teacher_Address1"), editable = true },
                    new { label = "Address 2", value = GetSafeField(reader, "Teacher_Address2"), editable = true },
                    new { label = "Location", value = GetSafeField(reader, "Teacher_LocationID"), editable = true},
                    new { label = "Profile Image", value = GetSafeField(reader, "ProfileImage"), editable = true }
                }
            };

            return JsonConvert.SerializeObject(profileData);
        }

        private string GetStudentJson(SqlDataReader reader)
        {
            var profileData = new
            {
                fields = new[]
                {
                    new { label = "First Name", value = GetSafeField(reader, "Student_FirstName"), editable = false },
                    new { label = "Middle Name", value = GetSafeField(reader, "Student_MiddleName"), editable = false },
                    new { label = "Last Name", value = GetSafeField(reader, "Student_LastName"), editable = false },
                    new { label = "Father's Name", value = GetSafeField(reader, "Student_FatherName"), editable = false },
                    new { label = "Mother's Name", value = GetSafeField(reader, "Student_MotherName"), editable = false },
                    new { label = "Standard", value = GetSafeField(reader, "Student_Standard"), editable = false },
                    new { label = "Division", value = GetSafeField(reader, "Student_Division"), editable = false },
                    new { label = "Section", value = GetSafeField(reader, "Student_Section"), editable = false },
                    new { label = "GR Number", value = GetSafeField(reader, "Student_GRNumber"), editable = false },
                    new { label = "Email", value = GetSafeField(reader, "Student_EmailID"), editable = true },
                    new { label = "Mobile Number", value = GetSafeField(reader, "Student_MobileNumber"), editable = true },
                    new { label = "Address 1", value = GetSafeField(reader, "Student_Address1"), editable = true },
                    new { label = "Address 2", value = GetSafeField(reader, "Student_Address2"), editable = true },
                    new { label = "Location", value = GetSafeField(reader, "Student_LocationID"), editable = true},
                    new { label = "Profile Image", value = GetSafeField(reader, "Student_ProfileImage"), editable = true }
                }
            };

            return JsonConvert.SerializeObject(profileData);
        }


        // Method to render profile fields dynamically from JSON
        private void RenderProfileFields(string profileJson, bool isEditable)
        {
            System.Diagnostics.Debug.WriteLine("Generated Profile JSON: " + profileJson);

            if (string.IsNullOrEmpty(profileJson))
            {
                System.Diagnostics.Debug.WriteLine("Profile JSON is null or empty.");
                return;
            }

            dynamic profileData = JsonConvert.DeserializeObject(profileJson);

            if (profileData == null || profileData.fields == null)
            {
                System.Diagnostics.Debug.WriteLine("Deserialized profileData is null or invalid.");
                return;
            }

            string profileHtml = "<div class='row'>";
            int columnCount = 0;

            foreach (var field in profileData.fields)
            {

                if (field.label.ToString().ToLower().Contains("location"))
                {
                    int locationID = field.value;
                    LoadLocationDetails(locationID);
                }

                if (field.label.ToString().ToLower().Contains("profile image"))
                {
                    profileHtml += CreateFileInputField(field.label.ToString(), field.value.ToString(), isEditable && (bool)field.editable);
                }
                else
                {
                    profileHtml += CreateInputField(field.label.ToString(), field.value.ToString(), isEditable && (bool)field.editable);
                }

                columnCount++;

                if (columnCount % 3 == 0)
                {
                    profileHtml += "</div><div class='row'>";
                }
            }

            profileHtml += "</div>";
            profileFields.InnerHtml = profileHtml;
        }

        private string CreateInputField(string label, string value, bool editable)
        {
            string readOnlyAttribute = editable ? "" : "readonly";
            return $"<div class='col-md-4'>" +
                   $"<div class='form-group'>" +
                   $"<label>{label}</label>" +
                   $"<input type='text' class='form-control' value='{value}' {readOnlyAttribute} />" +
                   $"</div>" +
                   $"</div>";
        }

        private string CreateFileInputField(string label, string value, bool editable)
        {
            string newValue = value != "" ? value : "default_user_logo.png";
            string path = "~/assets/img/user-profile-img/" + newValue;

            ProfileImage.ImageUrl = path;

            string disabledAttribute = editable ? "" : "disabled";
            return $"<div class='col-md-4'>" +
                   $"<div class='form-group'>" +
                   $"<label>{label}</label>" +
                   $"<input type='file' class='form-control' {disabledAttribute} />" +
                   $"</div>" +
                   $"</div>";
        }

        private void LoadLocationDetails(int locationID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM LocationMaster WHERE LocationID = @LocationID", conn))
                {
                    cmd.Parameters.AddWithValue("@LocationID", locationID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate location dropdowns
                            ddlCity.Enabled = true;
                            ddlPincode.Enabled = true;
                            ddlState.SelectedValue = reader["State"].ToString();
                            LoadCities(reader["State"].ToString());
                            ddlCity.SelectedValue = reader["City"].ToString();
                            LoadPincodes(reader["State"].ToString(), reader["City"].ToString());

                            // Check if the pincode exists in the ddlPincode dropdown list before setting the SelectedValue
                            string pincode = reader["Pincode"].ToString();
                            string location_id = reader["LocationID"].ToString();

                            // Ensure that the pincode is available in the dropdown
                            if (ddlPincode.Items.FindByValue(location_id) != null)
                            {
                                ddlPincode.SelectedValue = location_id;
                            }
                            else
                            {
                                // If the pincode is not available in the dropdown, add it dynamically or log the issue
                                ddlPincode.Items.Insert(0, new ListItem(location_id, pincode));
                                ddlPincode.SelectedValue = location_id;
                            }
                        }
                    }
                }
            }
        }

        private void LoadStates()
        {
            // Fetch all distinct states
            DataTable dtStates = GetLocationData(null, null);
            ddlState.DataSource = dtStates;
            ddlState.DataTextField = "State";
            ddlState.DataValueField = "State";
            ddlState.DataBind();
            ddlState.Items.Insert(0, new ListItem("Select State", ""));
        }

        private void LoadCities(string state)
        {
            // Fetch all cities for the selected state
            DataTable dtCities = GetLocationData(state, null);
            ddlCity.DataSource = dtCities;
            ddlCity.DataTextField = "City";
            ddlCity.DataValueField = "City";
            ddlCity.DataBind();
            ddlCity.Items.Insert(0, new ListItem("Select City", ""));
        }

        private void LoadPincodes(string state, string city)
        {
            // Fetch all pincodes for the selected state and city
            DataTable dtPincodes = GetLocationData(state, city);
            ddlPincode.DataSource = dtPincodes;
            ddlPincode.DataTextField = "Pincode";
            ddlPincode.DataValueField = "LocationID";
            ddlPincode.DataBind();
            ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = ddlState.SelectedValue;

            if (!string.IsNullOrEmpty(selectedState))
            {
                // Load cities for the selected state and enable city dropdown
                LoadCities(selectedState);
                ddlCity.Enabled = true;

                // Reset and disable pincode dropdown
                ddlPincode.Items.Clear();
                ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
                ddlPincode.Enabled = false;
            }
            else
            {
                // If no state is selected, disable both city and pincode dropdowns
                ddlCity.Enabled = false;
                ddlCity.Items.Clear();
                ddlCity.Items.Insert(0, new ListItem("Select City", ""));

                ddlPincode.Enabled = false;
                ddlPincode.Items.Clear();
                ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
            }
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = ddlState.SelectedValue;
            string selectedCity = ddlCity.SelectedValue;

            if (!string.IsNullOrEmpty(selectedCity))
            {
                // Load pincodes for the selected city and enable pincode dropdown
                LoadPincodes(selectedState, selectedCity);
                ddlPincode.Enabled = true;
            }
            else
            {
                // If no city is selected, disable pincode dropdown
                ddlPincode.Enabled = false;
                ddlPincode.Items.Clear();
                ddlPincode.Items.Insert(0, new ListItem("Select Pincode", ""));
            }
        }

        private DataTable GetLocationData(string state, string city)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetLocationDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@State", (object)state ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)city ?? DBNull.Value);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        protected void btnEditProfile_Click(object sender, EventArgs e)
        {
            // Set edit mode to true and reload the fields
            isEditMode = true;
            LoadUserProfileDetails(); // Reload the fields in editable mode
            btnEditProfile.Visible = false;
            btnSaveProfile.Visible = true;
            btnCancelUpdate.Visible = true;
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            // Save logic for updated fields
            SaveProfileChanges();
        }

        private void SaveProfileChanges()
        {
            // Implement saving logic using the updated values from input fields
            System.Diagnostics.Debug.WriteLine("Profile changes saved.");
            Response.Redirect("~/Profile.aspx");
        }

        protected void btnCancelUpdate_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Profile.aspx");
        }
    }
}
