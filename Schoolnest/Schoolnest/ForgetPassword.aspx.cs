using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Schoolnest.Utilities;

namespace Schoolnest
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString; // Connection string

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRoleDropdown();
            }
        }

        private void BindRoleDropdown()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM RoleMaster";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Bind the data to the dropdown list
                    RoleDropdown.DataSource = reader;
                    RoleDropdown.DataTextField = "RoleName";
                    RoleDropdown.DataValueField = "RoleID";
                    RoleDropdown.DataBind();

                    // Optionally, add a default "Select Role" option at the top
                    RoleDropdown.Items.Insert(0, new ListItem("Select Role", ""));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching roles: " + ex.Message);
            }
        }

        private bool SendOTPMailToUser(string userEmail, string selectedRole)
        {
            // Generate new OTP
            string otp = GenerateOTP();
            Session["Passcode"] = otp;
            Session["OtpEmail"] = userEmail;
            Session["RoleID"] = selectedRole;

            // Send email with OTP
            EmailService emailService = new EmailService();
            bool emailSent = emailService.SendEmail(
                userEmail,
                "Password Reset Request",
                $"Your OTP is: {otp}. This code will expire in 1 minute."
            );

            return emailSent;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string userInput = txtEmail.Text.Trim();

            string selectedRole = RoleDropdown.SelectedValue;

            string userId = null;

            if (IsEmail(userInput))
            {
                // Execute query based on role
                string userEmail = GetEmailForRole(userInput, selectedRole, userId);

                if (!string.IsNullOrEmpty(userEmail))
                {
                    bool emailSent = SendOTPMailToUser(userEmail, selectedRole);

                    if (emailSent)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowOTPPanel",$"setTimeout(function() {{ showOTPPanel('{pnlEmail.ClientID}', '{pnlPasscode.ClientID}'); }}, 100);",true);
                    }
                    else
                    {
                        // Handle email sending failure
                        rfvEmail.IsValid = false;
                        rfvEmail.Text = "Failed to send OTP. Please try again.";
                    }
                }
                else
                {
                    rfvEmail.IsValid = false;
                    rfvEmail.Text = "Email address is not registered.";
                }
            }
            else
            {
                string userEmail = GetEmailForUsername(userInput, selectedRole);

                if (!string.IsNullOrEmpty(userEmail))
                {
                    bool emailSent = SendOTPMailToUser(userEmail, selectedRole);

                    if (emailSent)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowOTPPanel",$"setTimeout(function() {{ showOTPPanel('{pnlEmail.ClientID}', '{pnlPasscode.ClientID}'); }}, 100);",true);
                    }
                    else
                    {
                        // Handle email sending failure
                        rfvEmail.IsValid = false;
                        rfvEmail.Text = "Failed to send OTP. Please try again.";
                    }
                }
                else
                {
                    rfvEmail.IsValid = false;
                    rfvEmail.Text = "Username is not registered.";
                }
            }
        }

        protected void btnResendOTP_Click(object sender, EventArgs e)
        {
            txtPasscode.ReadOnly = false;
            // Get the email from your existing logic
            string userEmail = Session["OtpEmail"]?.ToString();
            string selectedRole = Session["RoleID"]?.ToString();

            bool emailSent = SendOTPMailToUser(userEmail, selectedRole);

            if (emailSent)
            {
                // Reset the timer and button states
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowOTPPanel", $"setTimeout(function() {{ showOTPPanel('{pnlEmail.ClientID}', '{pnlPasscode.ClientID}'); }}, 100);", true);
            }
            else
            {
                // Handle email sending failure
                rfvPasscode.IsValid = false;
                rfvPasscode.Text = "Failed to send new OTP. Please try again.";
            }
        }

        private string GenerateOTP()
        {
            // Generate a random 6-digit OTP
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Helper method to check if the input is an email
        private bool IsEmail(string input)
        {
            return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Method to get email based on username from UserMaster table
        private string GetEmailForUsername(string username, string roleId)
        {
            string email = null;
            string userId = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    string query = null;

                    if (roleId == "SA")
                    { 
                        query = "SELECT Email FROM SuperAdmin WHERE Username = @Username";
                    }
                    else
                    {
                        query = "SELECT UserID FROM UserMaster WHERE Username = @Username AND RoleMaster_RoleID = @RoleID";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);

                    if (roleId != "SA")
                    {
                        cmd.Parameters.AddWithValue("@RoleID", roleId);
                    }


                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        if(roleId == "SA")
                        {
                            email = result.ToString();
                        }
                        else
                        {
                            userId = result.ToString();
                            email = GetEmailForRole(email, roleId, userId);
                        }
                                                      
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


            return email;
        }

        private string GetEmailForRole(string email, string roleId, string userId)
        {
            string emailField = null;
            string tableName = null;
            string query = null;

            // Determine the correct table based on the selected role
            switch (roleId)
            {
                case "SA":
                    emailField = "Email";
                    tableName = "SuperAdmin";
                    break;
                case "A":
                    emailField = "Email";
                    tableName = "Admin";
                    break;
                case "T":
                    emailField = "Teacher_Email";
                    tableName = "TeacherMaster";
                    break;
                case "S":
                    emailField = "Student_EmailID";
                    tableName = "StudentMaster";
                    break;
            }

            if (userId != null) {

                query = $"SELECT {emailField} FROM {tableName} WHERE UserMaster_UserID = @UserID";
            }
            else
            {
                query = $"SELECT {emailField} FROM {tableName} WHERE {emailField} = @Email ";
            }


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (userId != null)
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                    }

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string enteredOTP = txtPasscode.Text.Trim();
            string storedOTP = Session["Passcode"]?.ToString();

            if (enteredOTP == storedOTP)
            {
                // OTP verified successfully
                Session["Passcode"] = null;
                string userEmail = Session["OtpEmail"]?.ToString();
                string RoleID = Session["RoleID"]?.ToString();

                // Generate new password
                string newPassword = GeneratePassword();

                // Get the Username and SchoolID
                var (username, schoolId) = GetUsernameWithSchoolId(userEmail, RoleID);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlTransaction transaction = null;

                    try
                    {
                        conn.Open();

                        // Begin the transaction
                        transaction = conn.BeginTransaction();

                        // Update the user's password using the stored procedure
                        using (SqlCommand cmd = new SqlCommand("UpdateUserPassword", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add parameters to the stored procedure
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@RoleID", RoleID);
                            cmd.Parameters.AddWithValue("@NewPassword", newPassword);

                            if (RoleID != "SA" && !string.IsNullOrEmpty(schoolId))
                            {
                                cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                            }

                            // Execute the stored procedure
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Attempt to send the email with the new password

                                bool emailSent = SendPasswordUpdateMailToUser(userEmail, newPassword);

                                if (emailSent)
                                {
                                    // If email is sent successfully, commit the transaction
                                    transaction.Commit();

                                    // Reset session variables if needed
                                    Session["OtpEmail"] = null;
                                    Session["RoleID"] = null;

                                    // Show the success panel
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowOTPPanel", $"setTimeout(function() {{ showOTPPanel('{pnlPasscode.ClientID}', '{pnlSuccess.ClientID}'); }}, 100);", true);

                                    // Optionally, redirect to login after a short delay
                                    Timer1.Enabled = true;
                                }
                                else
                                {
                                    // If email sending fails, rollback the transaction
                                    transaction.Rollback();
                                    System.Diagnostics.Debug.WriteLine("Email failed to send, rolling back password update.");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Password didn't update, no rows affected.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // If there's any error, rollback the transaction
                        transaction?.Rollback();
                        System.Diagnostics.Debug.WriteLine("Error updating password or sending email: " + ex.Message);
                    }
                }
            }
            else
            {
                rfvPasscode.IsValid = false;
                rfvPasscode.Text = "Invalid OTP. Please try again.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ContinueTimer", $"setTimeout(function() {{ updateTimer(); }}, 100);", true);

                pnlEmail.CssClass = "panel-hidden";
                pnlPasscode.CssClass = "panel-slide-in";
            }
        }


        private (string Username, string SchoolID) GetUsernameWithSchoolId(string email, string roleId)
        {
            string username = null;
            string schoolId = null;
            string emailField = null;
            string tableName = null;
            string query = null;

            // Assuming you are retrieving data from a database or another source
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                switch (roleId)
                {
                    case "SA":
                        emailField = "Email";
                        tableName = "SuperAdmin";
                        break;
                    case "A":
                        emailField = "Email";
                        tableName = "Admin";
                        break;
                    case "T":
                        emailField = "Teacher_Email";
                        tableName = "TeacherMaster";
                        break;
                    case "S":
                        emailField = "Student_EmailID";
                        tableName = "StudentMaster";
                        break;
                }

                if (roleId == "SA")
                {
                    query = $"SELECT Username FROM SuperAdmin WHERE {emailField} = @Email AND RoleMaster_RoleID = @RoleID";
                }
                else
                {
                    query = $"SELECT u.UserMaster_UserID, u.SchoolMaster_SchoolID, um.Username FROM {tableName} AS u JOIN UserMaster AS um ON u.UserMaster_USerID = um.UserID WHERE u.{emailField} = @Email AND um.RoleMaster_RoleID = @RoleID";
                }
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@RoleID", roleId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        username = reader["Username"].ToString();
                        if (roleId != "SA")
                        {
                            schoolId = reader["SchoolMaster_SchoolID"].ToString();
                        }
                    }
                }
            }

            // Return both UserID and SchoolID as a tuple
            return (username, schoolId);
        }

        private bool SendPasswordUpdateMailToUser(string userEmail, string password)
        {
            // Send email with Password
            EmailService emailService = new EmailService();
            bool emailSent = emailService.SendEmail(
                userEmail,
                "Password Reset Request",
                $"Your new temporary password is: {password}. Please change password after login for security reasons."
            );

            return emailSent;
        }

        private string GeneratePassword()
        {
            // Define the character sets for capital letters, small letters, and numbers
            string capitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string smallLetters = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "0123456789";

            // Combine all characters into a single set
            string allCharacters = capitalLetters + smallLetters + numbers;

            // Randomly pick characters from the combined set
            Random random = new Random();
            char[] password = new char[6]; // Password length of 6

            // Ensure password includes at least one capital, one small letter, and one number
            password[0] = capitalLetters[random.Next(capitalLetters.Length)];
            password[1] = smallLetters[random.Next(smallLetters.Length)];
            password[2] = numbers[random.Next(numbers.Length)];

            // Fill remaining characters randomly from all characters
            for (int i = 3; i < 6; i++)
            {
                password[i] = allCharacters[random.Next(allCharacters.Length)];
            }
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }

        // Event triggered when the timer ticks (after 3 seconds)
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            // Stop the timer and redirect to login page
            Timer1.Enabled = false;
            Response.Redirect("~/Login.aspx");
        }
    }
}
