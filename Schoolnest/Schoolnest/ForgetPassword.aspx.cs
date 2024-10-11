using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest
{
	public partial class ForgetPassword1 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        // Event triggered when the "Send Reset Link" button is clicked
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                // Simulate sending an email here
                // For now, just show the passcode panel

                lblMessage.Visible = false;
                pnlEmail.Visible = false;
                pnlPasscode.Visible = true;
            }
            else
            {
                lblMessage.Text = "Please enter a valid email address.";
                lblMessage.Visible = true;
            }
        }

        // Event triggered when the "Verify" button is clicked
        protected void btnVerify_Click(object sender, EventArgs e)
        {
            if (txtPasscode.Text == "123456") // Example passcode
            {
                pnlPasscode.Visible = false;
                pnlSuccess.Visible = true;

                // Enable the timer to redirect to the login page after 3 seconds
                Timer1.Enabled = true;
            }
            else
            {
                lblMessage.Text = "Invalid passcode, please try again.";
                lblMessage.Visible = true;
            }
        }

        // Event triggered when the timer ticks (after 3 seconds)
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            // Stop the timer and redirect to login page
            Timer1.Enabled = false;
            Response.Redirect("Login.aspx");
        }
    }

}