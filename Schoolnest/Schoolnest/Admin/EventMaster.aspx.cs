using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class EventMaster : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadAcademicYear();
                LoadEvents();
            }
        }

        private void LoadAcademicYear()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT AcademicYear FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                    {
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        txtAcademicYear.Text = cmd.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading academic year: " + ex.Message);
            }
        }

        private void LoadEvents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM EventMaster WHERE SchoolMaster_SchoolID = @SchoolID ORDER BY EventDate", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Convert EventTime from TimeSpan to a formatted 12-hour string with AM/PM
                    dt.Columns.Add("FormattedEventTime", typeof(string)); // Add a new column for formatted time

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["EventTime"] != DBNull.Value)
                        {
                            TimeSpan time = (TimeSpan)row["EventTime"];
                            DateTime dateTime = DateTime.Today.Add(time);
                            row["FormattedEventTime"] = dateTime.ToString("hh:mm tt"); 
                        }
                        else
                        {
                            row["FormattedEventTime"] = ""; // Handle null or empty values
                        }
                    }

                    gvEvents.DataSource = dt;
                    gvEvents.DataBind();
                }
            }
        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("InsertUpdateEventMaster", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EventID", ViewState["EventID"] ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EventTitle", txtEventTitle.Text);
                        cmd.Parameters.AddWithValue("@EventDescription", txtEventDescription.Text);
                        cmd.Parameters.AddWithValue("@EventDate", Convert.ToDateTime(txtEventDate.Text));

                        // Convert HTML time input (HH:mm) to TimeSpan
                        TimeSpan eventTime;
                        if (TimeSpan.TryParse(txtEventTime.Text, out eventTime))
                        {
                            cmd.Parameters.AddWithValue("@EventTime", eventTime);
                        }
                        else
                        {
                            throw new FormatException("Invalid time format");
                        }

                        cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text);
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);

                        cmd.ExecuteNonQuery();
                        ViewState["EventID"] = null;
                        ClearForm();
                        LoadEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error saving event: " + ex.Message);
                // Add user-friendly error handling here
            }
        }

        protected void gvEvents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null)
                return;

            int eventId = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "EditEvent")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM EventMaster WHERE EventID = @EventID", conn))
                    {
                        cmd.Parameters.AddWithValue("@EventID", eventId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEventTitle.Text = reader["EventTitle"].ToString();
                                txtEventDescription.Text = reader["EventDescription"].ToString();
                                txtEventDate.Text = Convert.ToDateTime(reader["EventDate"]).ToString("yyyy-MM-dd");

                                if (reader["EventTime"] != DBNull.Value)
                                {
                                    TimeSpan time = (TimeSpan)reader["EventTime"];
                                    // Format time as HH:mm for HTML time input
                                    txtEventTime.Text = time.ToString(@"hh\:mm");
                                }

                                chkIsActive.Checked = Convert.ToBoolean(reader["IsActive"]);

                                ViewState["EventID"] = eventId;
                            }
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtEventTitle.Text = string.Empty;
            txtEventDescription.Text = string.Empty;
            txtEventDate.Text = string.Empty;
            txtEventTime.Text = string.Empty;
            ViewState["EventID"] = null;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/EventMaster.aspx");
        }
    }
}