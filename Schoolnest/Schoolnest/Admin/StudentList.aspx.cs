using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class StudentList : System.Web.UI.Page
    {

        private string connectionString = Global.ConnectionString; // Connection string
        private string SelectedStudentID = "";
        public string profileImagePath = null;
        public string schoolId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolID"].ToString();
                LoadStates();
                BindStandardDropdown();
                BindDivisionDropdown();
                BindSectionDropdown();
                BindBusRoute(schoolId);
                PopulateStudentDropdown(schoolId);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string firstName = txtFirstName.Text;
                    string lastName = txtLastName.Text;
                    int userID = 0;

                    if (SelectedStudentID == "")
                    {
                        // Execute the InsertUserMaster stored procedure to get UserID
                        userID = ExecuteInsertUserMaster(firstName, lastName, "S", schoolId);
                        ClientScript.RegisterStartupScript(this.GetType(), "Insert",$"alert('Inserted');", true);
                    }

                    ClientScript.RegisterStartupScript(this.GetType(), "Update", $"alert('Updated');", true);

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
                                string filePath = "~/assets/img/user-profile-img/student/" + fileName;

                                // Ensure the directory exists before saving the file
                                //string directoryPath = Server.MapPath("~/assets/img/user-profile-img/student/");
                                //if (!Directory.Exists(directoryPath))
                                //{
                                //     Directory.CreateDirectory(directoryPath);
                                // }

                                // Save the file
                                //fileProfileImage.SaveAs(Server.MapPath(filePath));
                                profileImagePath = "student/" + fileName;
                                ClientScript.RegisterStartupScript(this.GetType(), "Insert", $"alert(${profileImagePath});", true);
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

                    //SaveStudent(userID, schoolId);
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('Error: {ex.Message}');", true);
                }
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
                    object result = cmd.ExecuteScalar(); // Use ExecuteScalar to get the UserID directly
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void SaveStudent(int userID, string schoolId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateStudentMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(ddlStudents.SelectedValue))
                    {
                        // Pass NULL for insert, and let SQL generate the StudentID
                        cmd.Parameters.Add(new SqlParameter("@StudentID", SqlDbType.VarChar, 25) { Direction = ParameterDirection.Output });
                    }
                    else
                    {
                        // Pass existing StudentID for update
                        cmd.Parameters.Add(new SqlParameter("@StudentID", SelectedStudentID));
                    }
                    cmd.Parameters.AddWithValue("@Student_FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@Student_MiddleName", txtMiddleName.Text);
                    cmd.Parameters.AddWithValue("@Student_LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Student_FullName", txtFullName.Text);
                    cmd.Parameters.AddWithValue("@Student_Gender", ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@Student_DateOfBirth", Convert.ToDateTime(txtDateOfBirth.Text));
                    cmd.Parameters.AddWithValue("@Student_PlaceOfBirth", txtPlaceOfBirth.Text);
                    cmd.Parameters.AddWithValue("@Student_Religion", ddlReligion.SelectedValue);
                    cmd.Parameters.AddWithValue("@Student_Caste", ddlCaste.SelectedValue);
                    cmd.Parameters.AddWithValue("@Student_BloodGroup", ddlBloodGroup.SelectedValue);
                    cmd.Parameters.AddWithValue("@Student_ProfileImage", profileImagePath);
                    cmd.Parameters.AddWithValue("@Student_Standard", Convert.ToInt32(ddlStandard.SelectedValue));
                    cmd.Parameters.AddWithValue("@Student_Division", Convert.ToInt32(ddlDivision.SelectedValue));
                    cmd.Parameters.AddWithValue("@Student_Section", Convert.ToInt32(ddlSection.SelectedValue));
                    cmd.Parameters.AddWithValue("@Student_GRNumber", txtGRNumber.Text);
                    cmd.Parameters.AddWithValue("@Student_DateOfAdmission", Convert.ToDateTime(txtDateOfAdmission.Text));
                    cmd.Parameters.AddWithValue("@Student_MotherTongue", txtMotherTongue.Text);
                    cmd.Parameters.AddWithValue("@Student_EmailID", txtEmailID.Text);
                    cmd.Parameters.AddWithValue("@Student_MobileNumber", txtMobileNumber.Text);
                    cmd.Parameters.AddWithValue("@Student_Address1", txtAddress1.Text);
                    cmd.Parameters.AddWithValue("@Student_Address2", txtAddress2.Text);
                    cmd.Parameters.AddWithValue("@Student_LocationID", ddlPincode.SelectedValue);
                    cmd.Parameters.AddWithValue("@Student_FatherName", txtFatherName.Text);
                    cmd.Parameters.AddWithValue("@Student_MotherName", txtMotherName.Text);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolAttended", chkLastSchoolAttended.Checked);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolName", txtLastSchoolName.Text);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolAddress", txtLastSchoolAddress.Text);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolRemarks", txtLastSchoolRemarks.Text);
                    cmd.Parameters.AddWithValue("@Student_LastYearPercentage", string.IsNullOrEmpty(txtLastYearPercentage.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtLastYearPercentage.Text));
                    cmd.Parameters.AddWithValue("@Student_Grade", txtGrade.Text);
                    cmd.Parameters.AddWithValue("@Student_Remarks", txtRemarks.Text);
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@TransportAssignment_AssignmentID", Convert.ToInt32(ddlBusRoute.SelectedValue));
                    cmd.Parameters.AddWithValue("@UserMaster_UserID", userID);
                    cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked ? 1 : 0);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        string generatedStudentID = cmd.Parameters["@StudentID"].Value.ToString();
                        string emailid = txtEmailID.Text;
                        if (string.IsNullOrEmpty(SelectedStudentID))
                        {
                            // sendEmailtoStudent(generatedStudentID, emailid, schoolID);
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Student Created Successfully');", true);
                            ResetForm();

                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Student Updated Successfully');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception and show an error message
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                    }
                }
            }
        }

        //private void sendEmailtoStudent(string generatedStudentID, string emailid, string schoolID)
        //{
        //    try
        //    {
        //        // Retrieve the school name based on schoolID from the database
        //        string schoolName = GetSchoolNameByID(schoolID);

        //        string smtpServer = "smtp.gmail.com";
        //        int smtpPort = 587; // Use port 587 for TLS, or 465 for SSL
        //        string senderEmail = "schoolnestcapstone@gmail.com"; // Your Gmail email address
        //        string senderPassword = "nxij uksy myur bmny"; // Your Gmail App Password (if 2FA enabled) or Gmail password

        //        MailMessage mailMessage = new MailMessage
        //        {
        //            From = new MailAddress(senderEmail),
        //            Subject = "Registration Successful!",
        //            Body = $"Dear Student, \n\nWelcome to {schoolName}. Your registration has been successfully completed. " +
        //                           $"Your Student ID is: {generatedStudentID}\n" +
        //                           $"Your Password is: {"Student@123"}\n\n" +
        //                           "Please keep this information secure.\n\nBest regards,\n" + schoolName,
        //            IsBodyHtml = true // Set to true if sending HTML content
        //        };

        //        mailMessage.To.Add(emailid);

        //        // Create and configure the SmtpClient
        //        using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
        //        {
        //            smtpClient.EnableSsl = true; // Enable SSL/TLS for secure communication
        //            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
        //            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        //            // Send the email
        //            smtpClient.Send(mailMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle any errors that occur while sending the email
        //        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error sending email: " + ex.Message + "');", true);
        //    }
        //}

        // Method to get the school name from the database using the schoolID
        //private string GetSchoolNameByID(string schoolID)
        //{
        //    string schoolName = "Your School";
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT SchoolName FROM SchoolMaster WHERE SchoolID = @SchoolID";
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@SchoolID", schoolID);

        //        try
        //        {
        //            conn.Open();
        //            object result = cmd.ExecuteScalar();
        //            if (result != null)
        //            {
        //                schoolName = result.ToString();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle database connection errors here (log it, etc.)
        //            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error fetching school name: " + ex.Message + "');", true);
        //        }
        //    }
        //    return schoolName;
        //}

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

        private void PopulateStudentDropdown(string schoolId)
        {
            string query = "SELECT StudentID, Student_FullName FROM StudentMaster where SchoolMaster_SchoolID='" + schoolId + "'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStudents.Items.Clear();
                    ddlStudents.Items.Add(new ListItem("Select Student", ""));
                    while (reader.Read())
                    {
                        ListItem item = new ListItem(reader["Student_FullName"].ToString(), reader["StudentID"].ToString());
                        ddlStudents.Items.Add(item);
                    }
                }
            }
        }

        protected void ddlStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlStudents.SelectedValue))
            {
                SelectedStudentID = ddlStudents.SelectedValue;
                LoadStudentDetails(SelectedStudentID);
            }
            else
            {
                ResetForm();
            }
        }

        private void LoadStudentDetails(string studentId)
        {
            // Call the stored procedure
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetStudentDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtFirstName.Text = reader["Student_FirstName"].ToString();
                        txtMiddleName.Text = reader["Student_MiddleName"].ToString();
                        txtLastName.Text = reader["Student_LastName"].ToString();
                        txtFullName.Text = reader["Student_FullName"].ToString();
                        ddlGender.SelectedValue = reader["Student_Gender"].ToString();
                        if (reader["Student_DateOfBirth"] != DBNull.Value)
                        {
                            DateTime dateOfBirth = Convert.ToDateTime(reader["Student_DateOfBirth"]);
                            txtDateOfBirth.Text = dateOfBirth.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtDateOfBirth.Text = "";
                        }
                        txtPlaceOfBirth.Text = reader["Student_PlaceOfBirth"].ToString();
                        ddlReligion.SelectedValue = reader["Student_Religion"].ToString();
                        ddlCaste.SelectedValue = reader["Student_Caste"].ToString();
                        ddlBloodGroup.SelectedValue = reader["Student_BloodGroup"].ToString();
                        profileImagePath = reader["Student_ProfileImage"].ToString();
                        ddlStandard.SelectedValue = reader["Student_Standard"].ToString();
                        ddlDivision.SelectedValue = reader["Student_Division"].ToString();
                        ddlSection.SelectedValue = reader["Student_Section"].ToString();
                        txtGRNumber.Text = reader["Student_GRNumber"].ToString();
                        if (reader["Student_DateOfAdmission"] != DBNull.Value)
                        {
                            DateTime DOA = Convert.ToDateTime(reader["Student_DateOfAdmission"]);
                            txtDateOfAdmission.Text = DOA.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            txtDateOfAdmission.Text = ""; // Handle cases where the date is null in the database
                        }

                        txtMotherTongue.Text = reader["Student_MotherTongue"].ToString();
                        txtEmailID.Text = reader["Student_EmailID"].ToString();
                        txtMobileNumber.Text = reader["Student_MobileNumber"].ToString();
                        txtAddress1.Text = reader["Student_Address1"].ToString();
                        txtAddress2.Text = reader["Student_Address2"].ToString();
                        // Load location details (State, City, Pincode)
                        if (!reader.IsDBNull(reader.GetOrdinal("Student_LocationID")))
                        {
                            int locationID = reader.GetInt32(reader.GetOrdinal("Student_LocationID"));
                            LoadLocationDetails(locationID);
                        }
                        txtFatherName.Text = reader["Student_FatherName"].ToString();
                        txtMotherName.Text = reader["Student_MotherName"].ToString();
                        chkLastSchoolAttended.Checked = Convert.ToBoolean(reader["Student_LastSchoolAttended"]);
                        ShowHideLastSchoolDetails();
                        txtLastSchoolName.Text = reader["Student_LastSchoolName"].ToString();
                        txtLastSchoolAddress.Text = reader["Student_LastSchoolAddress"].ToString();
                        txtLastSchoolRemarks.Text = reader["Student_LastSchoolRemarks"].ToString();
                        txtLastYearPercentage.Text = reader["Student_LastYearPercentage"].ToString();
                        txtGrade.Text = reader["Student_Grade"].ToString();
                        txtRemarks.Text = reader["Student_Remarks"].ToString();
                        ddlBusRoute.SelectedValue = reader["TransportAssignment_AssignmentID"].ToString();
                        chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                    }
                }
            }
        }

        private void BindBusRoute(string schoolId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetBusRoutes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();
                    ddlBusRoute.DataSource = cmd.ExecuteReader();
                    ddlBusRoute.DataTextField = "Route";
                    ddlBusRoute.DataValueField = "TransportID";
                    ddlBusRoute.DataBind();
                }
            }
            ddlBusRoute.Items.Insert(0, new ListItem("--Select Bus Route--", "0"));
        }

        private void BindSectionDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSections", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    ddlSection.DataSource = cmd.ExecuteReader();
                    ddlSection.DataTextField = "SectionName";
                    ddlSection.DataValueField = "SectionID";
                    ddlSection.DataBind();
                }
            }
            ddlSection.Items.Insert(0, new ListItem("--Select Section--", "0"));
        }

        private void BindDivisionDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetDivisions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    ddlDivision.DataSource = cmd.ExecuteReader();
                    ddlDivision.DataTextField = "DivisionName";
                    ddlDivision.DataValueField = "DivisionID";
                    ddlDivision.DataBind();
                }
            }
            ddlDivision.Items.Insert(0, new ListItem("--Select Division--", "0"));
        }

        private void BindStandardDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetStandards", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    ddlStandard.DataSource = cmd.ExecuteReader();
                    ddlStandard.DataTextField = "StandardName";
                    ddlStandard.DataValueField = "StandardID";
                    ddlStandard.DataBind();
                }
            }
            ddlStandard.Items.Insert(0, new ListItem("--Select Standard--", "0"));
        }

        protected void chkLastSchoolAttended_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideLastSchoolDetails();
        }

        private void ShowHideLastSchoolDetails()
        {
            lastSchoolDetails.Visible = chkLastSchoolAttended.Checked ? true : false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void ResetForm()
        {
            Response.Redirect("~/Admin/StudentList.aspx");
        }

        protected void txtName_TextChanged(object sender, EventArgs e)
        {
            // Get the values from the textboxes
            string firstName = txtFirstName.Text.Trim();
            string middleName = txtMiddleName.Text.Trim();
            string lastName = txtLastName.Text.Trim();

            // Concatenate the names
            string fullName = $"{firstName} {middleName} {lastName}".Trim();

            // Set the Full Name textbox value
            txtFullName.Text = fullName;
        }
    }
}