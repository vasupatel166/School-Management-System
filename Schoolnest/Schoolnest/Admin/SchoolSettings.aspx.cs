using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Schoolnest.Admin
{
    public partial class SchoolSettings : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadExistingSettings();
            }
        }

        private void LoadExistingSettings()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // Academic Year
                            if (!reader.IsDBNull(reader.GetOrdinal("AcademicYear")))
                                txtAcademicYear.Text = reader["AcademicYear"].ToString();

                            // Time fields
                            if (!reader.IsDBNull(reader.GetOrdinal("ClassStartTime")))
                            {
                                TimeSpan classStartTime = (TimeSpan)reader["ClassStartTime"];
                                tpClassStartTime.Text = $"{classStartTime.Hours:D2}:{classStartTime.Minutes:D2}";
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("ClassEndTime")))
                            {
                                TimeSpan classEndTime = (TimeSpan)reader["ClassEndTime"];
                                tpClassEndTime.Text = $"{classEndTime.Hours:D2}:{classEndTime.Minutes:D2}";
                            }

                            // Convert Period Duration from TimeSpan to minutes
                            if (!reader.IsDBNull(reader.GetOrdinal("PeriodDuration")))
                            {
                                TimeSpan periodDuration = (TimeSpan)reader["PeriodDuration"];
                                tpPeriodDuration.Text = ((int)periodDuration.TotalMinutes).ToString();
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("BreakStartTime")))
                            {
                                TimeSpan breakStartTime = (TimeSpan)reader["BreakStartTime"];
                                tpBreakStartTime.Text = $"{breakStartTime.Hours:D2}:{breakStartTime.Minutes:D2}";
                            }

                            // Convert Break Duration from TimeSpan to minutes
                            if (!reader.IsDBNull(reader.GetOrdinal("BreakDuration")))
                            {
                                TimeSpan breakDuration = (TimeSpan)reader["BreakDuration"];
                                tpBreakDuration.Text = ((int)breakDuration.TotalMinutes).ToString();
                            }

                            // Date fields
                            if (!reader.IsDBNull(reader.GetOrdinal("FirstTermFeeDueDate")))
                            {
                                DateTime firstTermDate = Convert.ToDateTime(reader["FirstTermFeeDueDate"]);
                                txtFirstTermFeeDueDate.Text = firstTermDate.ToString("yyyy-MM-dd");
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("SecondTermFeeDueDate")))
                            {
                                DateTime secondTermDate = Convert.ToDateTime(reader["SecondTermFeeDueDate"]);
                                txtSecondTermFeeDueDate.Text = secondTermDate.ToString("yyyy-MM-dd");
                            }

                            // Late Fee Charges
                            if (!reader.IsDBNull(reader.GetOrdinal("LateFeeCharges")))
                                txtLateFeeCharges.Text = Convert.ToInt32(reader["LateFeeCharges"]).ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            $"alert('Error loading settings: {ex.Message}');", true);
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUpdateSchoolSettings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        // Convert minutes to TimeSpan for Period and Break Duration
                        TimeSpan periodDuration = TimeSpan.FromMinutes(Convert.ToInt32(tpPeriodDuration.Text));
                        TimeSpan breakDuration = TimeSpan.FromMinutes(Convert.ToInt32(tpBreakDuration.Text));

                        // Add parameters
                        cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text);
                        cmd.Parameters.AddWithValue("@ClassStartTime", Convert.ToDateTime(tpClassStartTime.Text).TimeOfDay);
                        cmd.Parameters.AddWithValue("@ClassEndTime", Convert.ToDateTime(tpClassEndTime.Text).TimeOfDay);
                        cmd.Parameters.AddWithValue("@PeriodDuration", periodDuration);
                        cmd.Parameters.AddWithValue("@BreakStartTime", Convert.ToDateTime(tpBreakStartTime.Text).TimeOfDay);
                        cmd.Parameters.AddWithValue("@BreakDuration", breakDuration);
                        cmd.Parameters.AddWithValue("@FirstTermFeeDueDate", Convert.ToDateTime(txtFirstTermFeeDueDate.Text));
                        cmd.Parameters.AddWithValue("@SecondTermFeeDueDate", Convert.ToDateTime(txtSecondTermFeeDueDate.Text));
                        cmd.Parameters.AddWithValue("@LateFeeCharges", Convert.ToInt32(txtLateFeeCharges.Text));
                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            "alert('School settings saved successfully!');", true);
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            $"alert('Error saving settings: {ex.Message}');", true);
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/SchoolSettings.aspx");
        }
    }
}