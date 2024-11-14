using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class SendEmailNotifications : System.Web.UI.Page
    {
        private static List<Notification> notificationsHistory = new List<Notification>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadNotificationsHistory();
            }
        }

        protected void btnSendNotification_Click(object sender, EventArgs e)
        {
            string subject = txtSubject.Text.Trim();
            string message = txtMessage.Text.Trim();
            string recipientGroup = ddlRecipientGroup.SelectedValue;

            Notification newNotification = new Notification
            {
                Subject = subject,
                Message = message,
                RecipientGroup = recipientGroup,
                DateSent = DateTime.Now
            };

            notificationsHistory.Insert(0, newNotification);
            txtSubject.Text = string.Empty;
            txtMessage.Text = string.Empty;
            ddlRecipientGroup.SelectedIndex = 0;

            LoadNotificationsHistory();
        }

        private void LoadNotificationsHistory()
        {
            pnlNoNotifications.Visible = notificationsHistory.Count == 0;
            rptNotificationsHistory.DataSource = notificationsHistory;
            rptNotificationsHistory.DataBind();
        }

        public class Notification
        {
            public string Subject { get; set; }
            public string Message { get; set; }
            public string RecipientGroup { get; set; }
            public DateTime DateSent { get; set; }
        }
    }
}