using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;

namespace Schoolnest.Teacher
{
    public partial class TeacherList : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string fileName = string.Empty;
        string filePath = string.Empty;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolId"].ToString();
            
            if (!IsPostBack)
            {
                ddlSearchTeacher.Visible = false;
                PopulateTeacherDropdown();
            }
        }

        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    form1.Attributes.Add("enctype", "multipart/form-data");
        //}

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
                SaveTeacher();
            }
        }

        private void SaveTeacher()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateTeacherMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(txtTeacherID.Text))
                    {
                        // Pass NULL for insert, and let SQL generate the StudentID
                        cmd.Parameters.Add(new SqlParameter("@TeacherID", SqlDbType.VarChar, 25) { Direction = ParameterDirection.Output });
                    }
                    else
                    {
                        // Pass existing StudentID for update
                        cmd.Parameters.Add(new SqlParameter("@TeacherID", txtTeacherID.Text));
                    }
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Firstname",txtFirstName.Text));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Lastname", txtLastName.Text));
                        cmd.Parameters.Add(new SqlParameter("@TeacherName", txtFullName.Text));
                        cmd.Parameters.Add(new SqlParameter("@Gender", ddlGender.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_DOB", Convert.ToDateTime(txtDateOfBirth.Text)));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Type", ddlTeacherType.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_MaritalStatus", ddlMaritalStatus.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_JoiningDate", Convert.ToDateTime(txtDateOfJoining.Text)));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Qualification", txtQualification.Text));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Experience", txtExperience.Text));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Email", txtEmail.Text));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_MobileNumber", txtMobileNumber.Text));
                        cmd.Parameters.Add(new SqlParameter("@ProfileImage", hfFilePath.Value));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Address1", txtAddress1.Text));
                        cmd.Parameters.Add(new SqlParameter("@Teacher_Address2", txtAddress2.Text));
                        //cmd.Parameters.Add(new SqlParameter("@Teacher_LocationID", ));
                        cmd.Parameters.Add(new SqlParameter("@SchoolMaster_SchoolID", SchoolID));
                        
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        string emailid = txtEmail.Text;
                        string FullName = txtFullName.Text;

                        if (string.IsNullOrEmpty(txtTeacherID.Text))
                        {
                            //sendEmailtoTeacher(generatedTeacherID, emailid, schoolID, FullName);
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
                    // Define the path to save the image
                    fileName = Path.GetFileName(fileUploadTeacherImage.FileName);
                    filePath = Server.MapPath("~/assets/img/user-profile-img/teacher/") + fileName;

                    // Save the uploaded file to the server
                    fileUploadTeacherImage.SaveAs(filePath);

                    // Display the uploaded image
                    imgTeacher.ImageUrl = "~/assets/img/user-profile-img/teacher/" + fileName;
                    imgTeacher.Visible = true;

                    btnDeleteImage.Visible = true;

                    hfFilePath.Value = imgTeacher.ImageUrl;
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

        //private void sendEmailtoTeacher(string generatedTeacherID, string emailid, string schoolID,string TeacherName)
        //{
        //    try
        //    {
        //        // Retrieve the school name based on schoolID from the database
        //        string schoolName = GetSchoolNameByID(schoolID);

        //        string smtpServer = "smtp.gmail.com";
        //        int smtpPort = 587; // Use port 587 for TLS, or 465 for SSL
        //        string senderEmail = "schoolnestcapstone@gmail.com";
        //        string senderPassword = "nxij uksy myur bmny";

        //        MailMessage mailMessage = new MailMessage
        //        {
        //            From = new MailAddress(senderEmail),
        //            Subject = "Registration Successful!",
        //            Body = $"Hello {TeacherName} , \n\nWelcome to {schoolName}. Your registration has been successfully completed. " +
        //                           $"Your Login ID is: {generatedTeacherID}\n" +
        //                           $"Your Password is: {"Teacher@123"}\n\n" +
        //                           "Please keep this information secure.\n\nBest regards,\n" + schoolName,
        //            IsBodyHtml = true
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

        private string GetSchoolNameByID(string schoolID)
        {
            string schoolName = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
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

        protected void ddlSearchTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSearchTeacher.SelectedValue))
            {
                LoadTeacherDeatils(ddlSearchTeacher.SelectedValue);             
            } 
        }

        private void LoadTeacherDeatils(string TeacherID)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            // Call the stored procedure
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetTeacherDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Specify that this is a stored procedure
                    cmd.Parameters.AddWithValue("@TeacherID", TeacherID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);

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
                            txtDateOfBirth.Text = ""; // Handle cases where the date is null in the database
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
                            txtDateOfJoining.Text = ""; // Handle cases where the date is null in the database
                        }
                        txtQualification.Text = reader["Teacher_Qualification"].ToString();
                        txtExperience.Text = reader["Teacher_Experience"].ToString();
                      
                        txtEmail.Text = reader["Teacher_Email"].ToString();
                        txtMobileNumber.Text = reader["Teacher_MobileNumber"].ToString();
                        txtAddress1.Text = reader["Teacher_Address1"].ToString();
                        txtAddress2.Text = reader["Teacher_Address2"].ToString();
                        txtAddress3.Text = reader["Teacher_Address3"].ToString();
                        string imgPath = reader["Teacher_ProfileImage"].ToString();
                        if (!string.IsNullOrEmpty(imgPath))
                        {
                            imgTeacher.ImageUrl = imgPath;  // Set the image path to the asp:Image control
                        }
                    }
                }
            }


        }
    }
}