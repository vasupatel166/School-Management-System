using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Student
{
    public partial class PaymentSuccess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StripeConfiguration.ApiKey = System.Configuration.ConfigurationManager.AppSettings["StripeSecretKey"];

            string sessionId = Request.QueryString["session_id"];

            if (!string.IsNullOrEmpty(sessionId))
            {
                try
                {
                    var service = new SessionService();
                    var session = service.Get(sessionId);

                    lblTransactionDetails.Text = $"Transaction Amount: {session.AmountTotal / 100.0:C} INR";
                }
                catch (Exception ex)
                {
                    // Log the error
                    lblTransactionDetails.Text = "Unable to retrieve transaction details.";
                }
            }
        }
    }
}