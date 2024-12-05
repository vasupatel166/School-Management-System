using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Schoolnest.Utilities;

namespace Schoolnest.Teacher
{
    public partial class TeacherList : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string fileName = string.Empty;
        string filePath = string.Empty;
        private string SchoolID;
        public string profileImagePath = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolId"].ToString();
            
            if (!IsPostBack)
            {
                LoadStates();
                ddlSearchTeacher.Visible = false;
                PopulateTeacherDropdown();
            }
        }

        private void PopulateTeacherDropdown()
        {
            string query = "SELECT TeacherID, TeacherName FROM TeacherMaster where SchoolMaster_SchoolID='" + SchoolID + "'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchTeacher.Items.Clear();
                    ddlSearchTeacher.Items.Add(new ListItem("-- Select Teacher --", ""));
                    while (reader.Read())
                    {
                        ListItem item = new ListItem(reader["TeacherName"].ToString(), reader["TeacherID"].ToString());
                        ddlSearchTeacher.Items.Add(item);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int userID = 0;

                string teacherID = txtTeacherID.Text;

                if (teacherID == "")
                {
                    string firstName = txtFirstName.Text;
                    string lastName = txtLastName.Text;

                    // Execute the InsertUserMaster stored procedure to get UserID
                    userID = ExecuteInsertUserMaster(firstName, lastName, "T", SchoolID);
                }

                SaveTeacher(userID);
            }
        }

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
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void SaveTeacher(int userID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateTeacherMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(txtTeacherID.Text))
                    {
                        cmd.Parameters.Add(new SqlParameter("@TeacherID", SqlDbType.VarChar, 25) { Direction = ParameterDirection.Output });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@TeacherID", txtTeacherID.Text));
                    }

                    cmd.Parameters.Add(new SqlParameter("@Teacher_Firstname",txtFirstName.Text));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Lastname", txtLastName.Text));
                    cmd.Parameters.Add(new SqlParameter("@TeacherName", txtFullName.Text));
                    cmd.Parameters.Add(new SqlParameter("@Gender", ddlGender.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_DOB", Convert.ToDateTime(txtDateOfBirth.Text)));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Type", ddlTeacherType.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_MaritalStatus", ddlMaritalStatus.SelectedValue));
                    DateTime? joiningDate = null;

                    if (!string.IsNullOrEmpty(txtDateOfJoining.Text))
                    {
                        joiningDate = Convert.ToDateTime(txtDateOfJoining.Text);
                    }
                    cmd.Parameters.Add(new SqlParameter("@Teacher_JoiningDate", joiningDate.HasValue ? (object)joiningDate.Value : DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Qualification", txtQualification.Text));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Experience", txtExperience.Text));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Email", txtEmail.Text));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_MobileNumber", txtMobileNumber.Text));
                    cmd.Parameters.Add(new SqlParameter("@ProfileImage", hfFilePath.Value));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Address1", txtAddress1.Text));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_Address2", txtAddress2.Text));
                    cmd.Parameters.Add(new SqlParameter("@Teacher_LocationID", int.Parse(ddlPincode.SelectedValue)));
                    cmd.Parameters.Add(new SqlParameter("@SchoolMaster_SchoolID", SchoolID));
                    cmd.Parameters.Add(new SqlParameter("@UserMaster_UserID", userID));
                    cmd.Parameters.Add(new SqlParameter("@IsActive", chkIsActive.Checked ? 1 : 0));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        string emailid = txtEmail.Text;
                        string TeacherName = txtFullName.Text;

                        if (string.IsNullOrEmpty(txtTeacherID.Text))
                        {
                            sendEmailtoTeacher(emailid, TeacherName);
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Teacher Created Successfully');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Teacher Updated Successfully');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                    }
                }
            }

        }

        protected void btnUploadImage_Click(object sender, EventArgs e)
        {
            if (fileUploadTeacherImage.HasFile)
            {
                try
                {
                    Random random = new Random();
                    int randomNumber = random.Next(10000, 99999);

                    string fileName = Path.GetFileName(fileUploadTeacherImage.FileName);
                    string extension = Path.GetExtension(fileName);

                    // Create a new file name by appending the random number and the extension
                    string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + randomNumber + extension;

                    // Define the path to save the image with the new file name
                    string filePath = Server.MapPath("~/assets/img/user-profile-img/teacher/") + newFileName;

                    // Save the uploaded file to the server
                    fileUploadTeacherImage.SaveAs(filePath);

                    // Display the uploaded image
                    imgTeacher.ImageUrl = "~/assets/img/user-profile-img/teacher/" + newFileName;
                    imgTeacher.Visible = true;

                    btnDeleteImage.Visible = true;

                    hfFilePath.Value = "teacher/" + newFileName;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"alert('An error occurred: {ex.Message}');", true);
                }
            }
        }

        protected void btnDeleteImage_Click(object sender, EventArgs e)
        {
            try
            {
                imgTeacher.ImageUrl = "";
                imgTeacher.Visible = false;

                // Optionally, delete the image file from the server
                string filePath = Server.MapPath(imgTeacher.ImageUrl);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Hide the delete button
                btnDeleteImage.Visible = false;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"alert('An error occurred: {ex.Message}');", true);
            }
        }

        protected void txtName_TextChanged(object sender, EventArgs e)
        {
            // Get the values from the textboxes
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();

            // Concatenate the names
            string fullName = $"{firstName}  {lastName}".Trim();

            // Set the Full Name textbox value
            txtFullName.Text = fullName;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/TeacherList.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ddlSearchTeacher.Visible = true;
        }

        private void sendEmailtoTeacher(string emailid, string TeacherName)
        {
            string Username;
            string temporaryPassword;

            string SchoolName;
            string SchoolEmail;

            bool success = GetTeacherDetails(emailid, out Username, out temporaryPassword);
            bool status = GetSchoolDetails(out SchoolName, out SchoolEmail);

            try
            {
                EmailService emailService = new EmailService();

                string emailSubject = $@"Welcome to {SchoolName}! Your New Account Details";

                string emailBody = $@"
                <p>Dear {TeacherName},</p>
                <p>We are pleased to inform you that your account has been successfully created in the School system. Below are your account details for logging in:</p>
                <p><strong>Username:</strong> {Username}</p>
                <p><strong>Email:</strong> {emailid}</p>
                <p><strong>Temporary Password:</strong> {temporaryPassword}</p>
                <p>Please use the above credentials to log in to your account. You can log in using your email address as the username and the temporary password provided.</p>
                <p><i>Note: You will be prompted to change your password upon your first login to secure your account.</i></p>
                <p>If you encounter any issues with logging in or have any questions, feel free to contact our support team at {SchoolEmail}.</p>
                <br />
                <p>Best regards,<br/>The SchoolNest Team</p>
                <p>{SchoolName}</p>
                <p>{SchoolEmail}</p>
                ";

                bool emailSent = emailService.SendEmail(
                    emailid,
                    emailSubject,
                    emailBody
                );
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine("Error sending email: " + ex.Message);
            }

        }

        private bool GetTeacherDetails(string emailID, out string username, out string password)
        {

            username = null;
            password = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserDataForLogin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("RoleID", "T");
                    cmd.Parameters.AddWithValue("@Username", emailID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                username = reader["Username"].ToString();
                                password = reader["DecryptedPassword"].ToString();
                                return true;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error fetching school name: " + ex.Message + "');", true);
                    }
                }
            }

            return false;
        }

        private bool GetSchoolDetails(out string SchoolName, out string SchoolEmail)
        {
            SchoolName = "";
            SchoolEmail = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SchoolName, SchoolEmail FROM SchoolMaster WHERE SchoolID = @SchoolID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SchoolName = reader["SchoolName"].ToString();
                            SchoolEmail = reader["SchoolEmail"].ToString();
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error fetching school name: " + ex.Message + "');", true);
                }
            }

            return false;
        }

        protected void ddlSearchTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSearchTeacher.SelectedValue))
            {
                LoadTeacherDeatils(ddlSearchTeacher.SelectedValue);             
            } 
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
                            ddlCity.Enabled = true;
                            ddlPincode.Enabled = true;
                            ddlState.SelectedValue = reader["State"].ToString();
                            LoadCities(reader["State"].ToString());
                            ddlCity.SelectedValue = reader["City"].ToString();
                            LoadPincodes(reader["State"].ToString(), reader["City"].ToString());

                            string pincode = reader["Pincode"].ToString();
                            string location_id = reader["LocationID"].ToString();

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

        private void LoadTeacherDeatils(string TeacherID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetTeacherDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtTeacherID.Text = reader["TeacherID"].ToString();
                        txtFirstName.Text = reader["Teacher_Firstname"].ToString();
                        txtLastName.Text = reader["Teacher_Lastname"].ToString();
                        txtFullName.Text = reader["TeacherName"].ToString();
                        ddlGender.SelectedValue = reader["Gender"].ToString();
                        if (reader["Teacher_DOB"] != DBNull.Value)
                        {
                            DateTime dateOfBirth = Convert.ToDateTime(reader["Teacher_DOB"]);
                            txtDateOfBirth.Text = dateOfBirth.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtDateOfBirth.Text = "";
                        }
                        ddlTeacherType.SelectedValue = reader["Teacher_Type"].ToString() ;
                        ddlMaritalStatus.SelectedValue = reader["Teacher_MaritalStatus"].ToString();
                        if (reader["Teacher_JoiningDate"] != DBNull.Value)
                        {
                            DateTime DOA = Convert.ToDateTime(reader["Teacher_JoiningDate"]);
                            txtDateOfJoining.Text = DOA.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtDateOfJoining.Text = "";
                        }
                        txtQualification.Text = reader["Teacher_Qualification"].ToString();
                        txtExperience.Text = reader["Teacher_Experience"].ToString();
                      
                        txtEmail.Text = reader["Teacher_Email"].ToString();
                        txtMobileNumber.Text = reader["Teacher_MobileNumber"].ToString();
                        txtAddress1.Text = reader["Teacher_Address1"].ToString();
                        txtAddress2.Text = reader["Teacher_Address2"].ToString();
                        // Load location details (State, City, Pincode)
                        if (!reader.IsDBNull(reader.GetOrdinal("Teacher_LocationID")))
                        {
                            int locationID = reader.GetInt32(reader.GetOrdinal("Teacher_LocationID"));
                            LoadLocationDetails(locationID);
                        }
                        string imgPath = reader["ProfileImage"].ToString();

                        hfFilePath.Value = imgPath;
                        imgTeacher.Visible = true;

                        if (!string.IsNullOrEmpty(imgPath))
                        {
                            imgTeacher.ImageUrl = "~/assets/img/user-profile-img/" + imgPath; 
                        }
                        else
                        {
                            imgTeacher.ImageUrl = "~/assets/img/user-profile-img/default_user_logo.png";
                        }

                        chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                    }
                }
            }


        }
    }
}