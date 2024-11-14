using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class Announcements : System.Web.UI.Page
    {
        private static List<Announcement> announcements = new List<Announcement>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRecentAnnouncements();
            }
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAnnouncement.Text))
            {
                return;
            }

            Announcement newAnnouncement = new Announcement
            {
                AnnouncementText = txtAnnouncement.Text.Trim(),
                AnnouncementDate = DateTime.Now
            };

            announcements.Insert(0, newAnnouncement);
            txtAnnouncement.Text = string.Empty;
            LoadRecentAnnouncements();
        }

        private void LoadRecentAnnouncements()
        {
            pnlNoAnnouncements.Visible = announcements.Count == 0;
            rptRecentAnnouncements.DataSource = announcements;
            rptRecentAnnouncements.DataBind();
        }

        public class Announcement
        {
            public string AnnouncementText { get; set; }
            public DateTime AnnouncementDate { get; set; }
        }
    }
}