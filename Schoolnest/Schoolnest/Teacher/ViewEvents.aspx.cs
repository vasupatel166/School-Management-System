using System;
using System.Data.SqlClient;
using System.Text;

namespace Schoolnest.Teacher
{
    public partial class ViewEvents : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();

            if (!IsPostBack)
            {
                LoadEvents();
            }
        }

        private void LoadEvents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT EventTitle, EventDescription, EventDate, EventTime 
                                 FROM EventMaster 
                                 WHERE SchoolMaster_SchoolID = @SchoolID 
                                   AND IsActive = 1 
                                 ORDER BY EventDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var eventsHtml = new StringBuilder();
                        int index = 1;
                        while (reader.Read())
                        {
                            var eventDate = Convert.ToDateTime(reader["EventDate"]).ToString("MMM dd, yyyy");
                            var eventTime = reader["EventTime"] as TimeSpan?;
                            var formattedTime = eventTime.HasValue
                                ? DateTime.Today.Add(eventTime.Value).ToString("hh:mm tt")
                                : "N/A";

                            eventsHtml.Append($@"
                                <tr>
                                    <td>{index++}</td>
                                    <td>{reader["EventTitle"]}</td>
                                    <td>{reader["EventDescription"]}</td>
                                    <td>{eventDate}</td>
                                    <td>{formattedTime}</td>
                                </tr>");
                        }

                        if (eventsHtml.Length == 0)
                        {
                            eventsHtml.Append("<tr><td colspan='5' class='text-center'>No events available.</td></tr>");
                        }

                        EventsTableBody.InnerHtml = eventsHtml.ToString();
                    }
                }
            }
        }
    }
}
