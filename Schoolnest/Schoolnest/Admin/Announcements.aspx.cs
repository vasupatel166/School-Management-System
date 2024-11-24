using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class Announcements : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            if (!IsPostBack)
            {
                LoadAnnouncements();
                btnSave.Text = "Save";
            }
        }

        private void LoadAnnouncements()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AnnouncementMaster WHERE SchoolMaster_SchoolID = @SchoolID ORDER BY CreatedDateTime DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAnnouncements.DataSource = dt;
                    gvAnnouncements.DataBind();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateAnnouncement", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AnnouncementID", Convert.ToInt32(hdnAnnouncementId.Value));
                        cmd.Parameters.AddWithValue("@AnnouncementTitle", txtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@AnnouncementDescription", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@TargetAudience", ddlTargetAudience.SelectedValue);
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                ClearForm();
                LoadAnnouncements();
                ShowMessage("Announcement saved successfully!");
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void gvAnnouncements_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int announcementId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditAnnouncement")
            {
                LoadAnnouncementForEdit(announcementId);
            }
            else if (e.CommandName == "DeleteAnnouncement")
            {
                DeleteAnnouncement(announcementId);
            }
        }

        private void LoadAnnouncementForEdit(int announcementId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AnnouncementMaster WHERE AnnouncementID = @AnnouncementID", conn))
                {
                    cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hdnAnnouncementId.Value = announcementId.ToString();
                            txtTitle.Text = reader["AnnouncementTitle"].ToString();
                            txtDescription.Text = reader["AnnouncementDescription"].ToString();
                            ddlTargetAudience.SelectedValue = reader["TargetAudience"].ToString();
                            chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                            btnSave.Text = "Update";
                        }
                    }
                }
            }
        }

        private void DeleteAnnouncement(int announcementId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM AnnouncementMaster WHERE AnnouncementID = @AnnouncementID", conn))
                    {
                        cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadAnnouncements();
                ShowMessage("Announcement deleted successfully!");
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            hdnAnnouncementId.Value = "0";
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlTargetAudience.SelectedIndex = 0;
            chkIsActive.Checked = false;
            btnSave.Text = "Save";
        }

        private void ShowMessage(string message)
        {
            // Implement your preferred way of showing messages (e.g., using a Label control or JavaScript alert)
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", $"alert('{message}');", true);
        }
    }
}