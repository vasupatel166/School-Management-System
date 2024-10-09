using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web.Services;


namespace Schoolnest.Admin
{
    public partial class StudentList : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["schoolnestConnectionString"].ConnectionString;
        string fileName = string.Empty;
        string filePath = string.Empty;
        string schoolId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                BindStandardDropdown();
                BindDivisionDropdown();
                BindSectionDropdown();
                BindBusRoute(schoolId);
                PopulateStudentDropdown(schoolId);
                ddlStudents.Visible = false;
            }
        }

        private void PopulateStudentDropdown(string schoolId)
        {
            // Example: Retrieve student data from the database based on a search criteria (e.g., Student Name or ID)
            string query = "SELECT StudentID, student_FullName FROM StudentMaster where SchoolMaster_SchoolID='"+schoolId+"'";

            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStudents.Items.Clear();
                    ddlStudents.Items.Add(new ListItem("-- Select Student --", ""));
                    while (reader.Read())
                    {
                        ListItem item = new ListItem(reader["student_FullName"].ToString(), reader["StudentID"].ToString());
                        ddlStudents.Items.Add(item);
                    }
                }
            }
        }

        protected void ddlStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlStudents.SelectedValue))
            {
                LoadStudentDetails(ddlStudents.SelectedValue);
                //studentDetails.Visible = true; // Show student details when a student is selected
            }
            else
            {
                //studentDetails.Visible = false; // Hide details if no student is selected
            }
        }

        private void LoadStudentDetails(string studentId)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            // Call the stored procedure
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetStudentDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Specify that this is a stored procedure
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtStudentID.Text = reader["StudentID"].ToString();
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
                            txtDateOfBirth.Text = ""; // Handle cases where the date is null in the database
                        }
                        
                        txtPlaceOfBirth.Text = reader["Student_PlaceOfBirth"].ToString();
                        ddlReligion.SelectedValue = reader["Student_Religion"].ToString();
                        ddlCaste.SelectedValue = reader["Student_Caste"].ToString();
                        ddlBloodGroup.SelectedValue = reader["Student_BloodGroup"].ToString();
                        string imgPath= reader["Student_ProfileImage"].ToString();
                        if (!string.IsNullOrEmpty(imgPath))
                        {
                            imgStudent.ImageUrl = imgPath;  // Set the image path to the asp:Image control
                        }
                       
                        ddlStandard.SelectedValue = reader["Student_Standard"].ToString() ;
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
                        ddlStatus.SelectedValue = reader["Student_Status"].ToString();
                        txtLastSchoolAttended.Text = reader["LastSchoolAttended"].ToString();
                        txtMotherTongue.Text = reader["Student_MotherTongue"].ToString();
                        txtEmailID.Text = reader["Student_EmailID"].ToString();  
                        txtMobileNumber.Text = reader["Student_MobileNumber"].ToString();
                        txtAddress1.Text = reader["Student_Address1"].ToString();
                        txtAddress2.Text = reader["Student_Address2"].ToString();
                        txtAddress3.Text = reader["Student_LocationID"].ToString();
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
                        // Populate other fields as needed
                    }
                }
            }
        
    
}

        protected void Page_Init(object sender, EventArgs e)
        {
            form1.Attributes.Add("enctype", "multipart/form-data");
        }

        private void BindBusRoute(string schoolId)
        {
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
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
            lastSchoolDetails.Style["display"] = chkLastSchoolAttended.Checked ? "block" : "none";
        }

        protected void btnUploadImage_Click(object sender, EventArgs e)
        {
            if (fileUploadStudentImage.HasFile)
            {
                try
                {
                    // Define the path to save the image
                    fileName = Path.GetFileName(fileUploadStudentImage.FileName);
                    filePath = Server.MapPath("~/studentimages/") + fileName;

                    // Save the uploaded file to the server
                    fileUploadStudentImage.SaveAs(filePath);

                    // Display the uploaded image
                    imgStudent.ImageUrl = "~/studentimages/" + fileName; // Set the image URL to display
                    imgStudent.Visible = true; // Make the image visible

                    // Show the delete button
                    btnDeleteImage.Visible = true;

                    hfFilePath.Value = imgStudent.ImageUrl;
                }
                catch (Exception ex)
                {
                    // Handle the error (e.g., show a message)
                    // You can log the error or display a message to the user
                }
            }
        }

        protected void btnDeleteImage_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear the image URL
                imgStudent.ImageUrl = "";
                imgStudent.Visible = false; // Hide the image

                // Optionally, delete the image file from the server
                string filePath = Server.MapPath(imgStudent.ImageUrl);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Hide the delete button
                btnDeleteImage.Visible = false;
            }
            catch (Exception ex)
            {
                // Handle the error (e.g., show a message)
                // You can log the error or display a message to the user
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if(Page.IsValid)
            {
                SaveStudent();
            }
            
        }

        private void SaveStudent()
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateStudentMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(txtStudentID.Text))
                    {
                        // Pass NULL for insert, and let SQL generate the StudentID
                        cmd.Parameters.Add(new SqlParameter("@StudentID", SqlDbType.VarChar, 25) { Direction = ParameterDirection.Output });
                    }
                    else
                    {
                        // Pass existing StudentID for update
                        cmd.Parameters.Add(new SqlParameter("@StudentID", txtStudentID.Text));
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
                    cmd.Parameters.AddWithValue("@Student_ProfileImage", hfFilePath.Value); // Assuming you have this field
                    cmd.Parameters.AddWithValue("@Student_Standard", Convert.ToInt32(ddlStandard.SelectedValue));
                    cmd.Parameters.AddWithValue("@Student_Division", Convert.ToInt32(ddlDivision.SelectedValue));
                    cmd.Parameters.AddWithValue("@Student_Section", Convert.ToInt32(ddlSection.SelectedValue));
                    cmd.Parameters.AddWithValue("@Student_GRNumber", txtGRNumber.Text);
                    cmd.Parameters.AddWithValue("@Student_DateOfAdmission", Convert.ToDateTime(txtDateOfAdmission.Text));
                    cmd.Parameters.AddWithValue("@Student_Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@LastSchoolAttended", txtLastSchoolAttended.Text);
                    cmd.Parameters.AddWithValue("@Student_MotherTongue", txtMotherTongue.Text);
                    cmd.Parameters.AddWithValue("@Student_EmailID", txtEmailID.Text);
                    cmd.Parameters.AddWithValue("@Student_MobileNumber", txtMobileNumber.Text);
                    cmd.Parameters.AddWithValue("@Student_Address1", txtAddress1.Text);
                    cmd.Parameters.AddWithValue("@Student_Address2", txtAddress2.Text);
                    cmd.Parameters.AddWithValue("@Student_LocationID", txtAddress3.Text); // Assuming you have this dropdown
                    cmd.Parameters.AddWithValue("@Student_FatherName", txtFatherName.Text);
                    cmd.Parameters.AddWithValue("@Student_MotherName", txtMotherName.Text);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolAttended", chkLastSchoolAttended.Checked);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolName", txtLastSchoolName.Text);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolAddress", txtLastSchoolAddress.Text);
                    cmd.Parameters.AddWithValue("@Student_LastSchoolRemarks", txtLastSchoolRemarks.Text);
                    cmd.Parameters.AddWithValue("@Student_LastYearPercentage", string.IsNullOrEmpty(txtLastYearPercentage.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtLastYearPercentage.Text));
                    cmd.Parameters.AddWithValue("@Student_Grade", txtGrade.Text);
                    cmd.Parameters.AddWithValue("@Student_Remarks", txtRemarks.Text);
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", schoolID); // Assuming you have this dropdown
                    cmd.Parameters.AddWithValue("@TransportAssignment_AssignmentID", Convert.ToInt32(ddlBusRoute.SelectedValue));// Assuming you have this dropdown
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        string generatedStudentID = cmd.Parameters["@StudentID"].Value.ToString();
                        string hashedPassword = HashPassword("Student@123");
                        string emailid = txtEmailID.Text;
                        if (string.IsNullOrEmpty(txtStudentID.Text))
                        {
                            saveUser(generatedStudentID, hashedPassword, schoolID);
                            sendEmailtoStudent(generatedStudentID, emailid, schoolID);
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Student Created Successfully');", true);
                            ClearForm();

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

        private void sendEmailtoStudent(string generatedStudentID, string emailid, string schoolID)
        {
            try
            {
                // Retrieve the school name based on schoolID from the database
                string schoolName = GetSchoolNameByID(schoolID);

                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587; // Use port 587 for TLS, or 465 for SSL
                string senderEmail = "schoolnestcapstone@gmail.com"; // Your Gmail email address
                string senderPassword = "nxij uksy myur bmny"; // Your Gmail App Password (if 2FA enabled) or Gmail password

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Registration Successful!",
                    Body = $"Dear Student, \n\nWelcome to {schoolName}. Your registration has been successfully completed. " +
                                   $"Your Student ID is: {generatedStudentID}\n" +
                                   $"Your Password is: {"Student@123"}\n\n" +
                                   "Please keep this information secure.\n\nBest regards,\n" + schoolName,
                    IsBodyHtml = true // Set to true if sending HTML content
                };

                mailMessage.To.Add(emailid);

                // Create and configure the SmtpClient
                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.EnableSsl = true; // Enable SSL/TLS for secure communication
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Send the email
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log or handle any errors that occur while sending the email
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error sending email: " + ex.Message + "');", true);
            }
        }

        // Method to get the school name from the database using the schoolID
        private string GetSchoolNameByID(string schoolID)
        {
            string schoolName = "Your School"; // Default value if school not found
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                string query = "SELECT SchoolName FROM SchoolMaster WHERE SchoolID = @SchoolID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        schoolName = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Handle database connection errors here (log it, etc.)
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error fetching school name: " + ex.Message + "');", true);
                }
            }

            return schoolName;
        }



        private void saveUser(string generatedStudentID, string hashedPassword, string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(Global.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUserfromStudentMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", generatedStudentID);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@schoolID", schoolID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void ClearForm()
        {
            // Clear all form fields
            txtStudentID.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtMiddleName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtFullName.Text = string.Empty;
            ddlGender.SelectedIndex = 0;
            ddlBusRoute.SelectedIndex = 0;
            txtDateOfBirth.Text = string.Empty;
            txtPlaceOfBirth.Text = string.Empty;
            ddlReligion.SelectedIndex = 0;
            ddlCaste.SelectedIndex = 0;
            ddlBloodGroup.SelectedIndex = 0;
            ddlStandard.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
            ddlSection.SelectedIndex = 0;
            txtGRNumber.Text = string.Empty;
            txtDateOfAdmission.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            txtLastSchoolAttended.Text = string.Empty;
            txtMotherTongue.Text = string.Empty;
            txtEmailID.Text = string.Empty;
            txtMobileNumber.Text = string.Empty;
            txtAddress1.Text = string.Empty;
            txtAddress2.Text = string.Empty;
            txtAddress3.Text = string.Empty;
            txtFatherName.Text = string.Empty;
            txtMotherName.Text = string.Empty;
            txtLastSchoolName.Text = string.Empty;
            txtLastSchoolAddress.Text = string.Empty;
            txtLastSchoolRemarks.Text = string.Empty;
            txtLastYearPercentage.Text = string.Empty;
            txtGrade.Text = string.Empty;
            txtRemarks.Text = string.Empty;
            chkLastSchoolAttended.Checked= false;
        }
    

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ddlStudents.Visible = true;
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