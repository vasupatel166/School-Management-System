using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Teacher
{
    public partial class ViewAnnouncements : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        private int SelectedAnnouncementId
        {
            get
            {
                object obj = ViewState["SelectedAnnouncementId"];
                return obj != null ? (int)obj : -1;
            }
            set
            {
                ViewState["SelectedAnnouncementId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            if (!IsPostBack)
            {
                string announcementId = Request.QueryString["aid"];
                if (!string.IsNullOrEmpty(announcementId) && int.TryParse(announcementId, out int aid))
                {
                    SelectedAnnouncementId = aid;
                    DisplayAnnouncementDetails(aid);
                }

                LoadAnnouncements();
            }
        }

        protected bool IsSelectedAnnouncement(object announcementId)
        {
            if (announcementId == null) return false;
            return Convert.ToInt32(announcementId) == SelectedAnnouncementId;
        }

        private void LoadAnnouncements()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT AnnouncementID, AnnouncementTitle, AnnouncementDescription,
                    CASE 
                        WHEN UpdatedDateTime IS NULL THEN CreatedDateTime 
                        ELSE UpdatedDateTime 
                    END AS DisplayDateTime
                    FROM AnnouncementMaster 
                    WHERE (TargetAudience = 'Teacher' OR TargetAudience = 'Both')
                    AND SchoolMaster_SchoolID = @SchoolID AND IsActive = 1 
                    ORDER BY DisplayDateTime DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            rptAnnouncements.DataSource = reader;
                            rptAnnouncements.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        protected void rptAnnouncements_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ShowDetails")
            {
                int announcementId = Convert.ToInt32(e.CommandArgument);
                SelectedAnnouncementId = announcementId;
                DisplayAnnouncementDetails(announcementId);
                LoadAnnouncements(); // Reload the announcements list
            }
        }

        private void DisplayAnnouncementDetails(int announcementId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT AnnouncementTitle, AnnouncementDescription,
                    CASE 
                        WHEN UpdatedDateTime IS NULL THEN CreatedDateTime 
                        ELSE UpdatedDateTime 
                    END AS DisplayDateTime
                    FROM AnnouncementMaster 
                    WHERE AnnouncementID = @AnnouncementID", conn))
                {
                    cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pnlNoAnnouncement.Visible = false;
                                pnlAnnouncementDetails.Visible = true;

                                lblTitle.Text = reader["AnnouncementTitle"].ToString();
                                lblDescription.Text = reader["AnnouncementDescription"].ToString();
                                lblDateTime.Text = Convert.ToDateTime(reader["DisplayDateTime"]).ToString("MMM dd, yyyy hh:mm tt");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("An error occurred: " + ex.Message);
                    }
                }
            }
        }
    }
}