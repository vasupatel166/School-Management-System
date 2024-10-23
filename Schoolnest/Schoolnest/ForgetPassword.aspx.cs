using System;
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


                    string query = "SELECT * FROM RoleMaster ";
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

            bool emailSent = SendOTPMailToUser(userEmail);

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

                // Generate new Password
                string password = GeneratePassword();

                // Update the password in database

                bool emailSent = SendPasswordUpdateMailToUser(userEmail, password);

                // Session["OtpEmail"] = null;
                // Session["RoleID"] = null;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowOTPPanel", $"setTimeout(function() {{ showOTPPanel('{pnlPasscode.ClientID}', '{pnlSuccess.ClientID}'); }}, 100);", true);

                // Redirect to login after a short delay
                Timer1.Enabled = true;
            }
            else
            {
                rfvPasscode.IsValid = false;
                rfvPasscode.Text = "Invalid OTP. Please try again.";
                // Continue the timer with the remaining time
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ContinueTimer", $"setTimeout(function() {{ updateTimer(); }}, 100);", true);

                pnlEmail.CssClass = "panel-hidden";
                pnlPasscode.CssClass = "panel-slide-in";


            }
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
