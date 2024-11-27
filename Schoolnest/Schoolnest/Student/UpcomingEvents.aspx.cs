using System;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace Schoolnest.Student
{
    public partial class UpcomingEvents : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadUpcomingEvents();
            }
        }

        private void LoadUpcomingEvents()
        {
            try
            {
                StringBuilder html = new StringBuilder();
                int cardCount = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT EventTitle, EventDescription, EventDate, EventTime 
                FROM EventMaster 
                WHERE SchoolMaster_SchoolID = @SchoolID AND IsActive = 1 
                      AND EventDate >= GETDATE()
                ORDER BY EventDate ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID ?? string.Empty);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    // Start a new row every 3 cards
                                    if (cardCount % 3 == 0)
                                    {
                                        if (cardCount > 0)
                                        {
                                            html.Append("</div>"); // Close previous row
                                        }
                                        html.Append("<div class='row mb-4'>"); // Start new row
                                    }

                                    // Fetch Event Data with null checks
                                    string eventTitle = reader["EventTitle"] != DBNull.Value
                                        ? reader["EventTitle"].ToString()
                                        : "Untitled Event";

                                    string eventDescription = reader["EventDescription"] != DBNull.Value
                                        ? reader["EventDescription"].ToString()
                                        : "No description available";

                                    string eventDateRaw = reader["EventDate"] != DBNull.Value
                                        ? reader["EventDate"].ToString()
                                        : null;

                                    string eventTimeRaw = reader["EventTime"] != DBNull.Value
                                        ? reader["EventTime"].ToString()
                                        : null;

                                    // Parse Event Date
                                    string eventDate = (eventDateRaw != null && DateTime.TryParse(eventDateRaw, out DateTime parsedDate))
                                        ? parsedDate.ToString("MMMM dd, yyyy")
                                        : "Invalid Date";

                                    // Parse Event Time
                                    string eventTime = (eventTimeRaw != null && TimeSpan.TryParse(eventTimeRaw, out TimeSpan parsedTime))
                                        ? DateTime.Today.Add(parsedTime).ToString("hh:mm tt")
                                        : "Invalid Time";

                                    // Build Event Card HTML with HTML encoding and Bootstrap grid
                                    html.Append($@"
                                <div class='col-md-4 mb-3'>
                                    <div class='event-card h-100'>
                                        <h4>{HttpUtility.HtmlEncode(eventTitle)}</h4>
                                        <p>{HttpUtility.HtmlEncode(eventDescription)}</p>
                                        <div class='event-date-time'>
                                            <span>{eventDate}</span><br />
                                            <span>{eventTime}</span>
                                        </div>
                                    </div>
                                </div>");

                                    cardCount++;
                                }

                                // Close the last row
                                if (cardCount > 0)
                                {
                                    html.Append("</div>");
                                }
                            }
                            else
                            {
                                html.Append("<div class='no-events'>No Upcoming Events</div>");
                            }
                        }
                    }
                }

                // Append generated HTML to the eventContainer div
                eventContainer.InnerHtml = html.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading events: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Show a user-friendly error message
                eventContainer.InnerHtml = "<div class='no-events'>Unable to load events. Please try again later.</div>";
            }
        }

    }
}
