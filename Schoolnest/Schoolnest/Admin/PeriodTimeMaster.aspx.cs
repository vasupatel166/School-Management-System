using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Schoolnest.Admin
{
    public partial class PeriodTimeMaster : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            if (!IsPostBack)
            {
                BindPeriodTimeDropdown();
                BindPeriodTimeGridview();
                btnSave.Text = "Save";
                ViewState["PeriodTimeID"] = 0;
            }
        }

        private void BindPeriodTimeDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT ClassStartTime, ClassEndTime, PeriodDuration, BreakStartTime, BreakDuration FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", con))
                    {
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            // Get time values directly from SQL Server time type
                            TimeSpan classStartTimeSpan = dr.GetTimeSpan(dr.GetOrdinal("ClassStartTime"));
                            TimeSpan classEndTimeSpan = dr.GetTimeSpan(dr.GetOrdinal("ClassEndTime"));
                            TimeSpan breakStartTimeSpan = dr.GetTimeSpan(dr.GetOrdinal("BreakStartTime"));
                            TimeSpan periodDuration = dr.GetTimeSpan(dr.GetOrdinal("PeriodDuration"));
                            TimeSpan breakDuration = dr.GetTimeSpan(dr.GetOrdinal("BreakDuration"));

                            // Convert to base DateTime for today
                            DateTime baseDate = DateTime.Today;
                            DateTime classStartTime = baseDate.Add(classStartTimeSpan);
                            DateTime classEndTime = baseDate.Add(classEndTimeSpan);
                            DateTime breakStartTime = baseDate.Add(breakStartTimeSpan);

                            List<TimeSlot> timeSlots = GenerateTimeSlots(
                                classStartTime,
                                classEndTime,
                                periodDuration,
                                breakStartTime,
                                breakDuration
                            );

                            ddlPeriodTime.Items.Clear();
                            ddlPeriodTime.Items.Add(new ListItem("-- Select Time Slot --", ""));

                            foreach (var slot in timeSlots)
                            {
                                string timeText = $"{slot.StartTime:h:mm tt} - {slot.EndTime:h:mm tt}";
                                ddlPeriodTime.Items.Add(new ListItem(timeText, timeText));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error binding period time dropdown: " + ex.Message);
            }
        }


        private class TimeSlot
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool IsBreak { get; set; }
        }

        private List<TimeSlot> GenerateTimeSlots(DateTime classStartTime, DateTime classEndTime,
            TimeSpan periodDuration, DateTime breakStartTime, TimeSpan breakDuration)
        {
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            DateTime currentTime = classStartTime;

            while (currentTime < classEndTime)
            {
                TimeSlot slot = new TimeSlot();
                slot.StartTime = currentTime;

                // Check if this slot should be a break
                if (currentTime.TimeOfDay == breakStartTime.TimeOfDay)
                {
                    slot.EndTime = currentTime.Add(breakDuration);
                    slot.IsBreak = true;
                }
                else
                {
                    slot.EndTime = currentTime.Add(periodDuration);
                    slot.IsBreak = false;
                }

                // Add slot only if it doesn't exceed class end time
                if (slot.EndTime <= classEndTime)
                {
                    timeSlots.Add(slot);
                }

                currentTime = slot.EndTime;
            }

            return timeSlots;
        }

        private void BindPeriodTimeGridview()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM PeriodTimeMaster WHERE SchoolMaster_SchoolID = @SchoolID", con))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        gvPeriodTime.DataSource = dt;
                        gvPeriodTime.DataBind();
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int periodTimeID = Convert.ToInt32(ViewState["PeriodTimeID"]);
            string action = periodTimeID == 0 ? "INSERT" : "UPDATE";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUpdatePeriodTimeMaster", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PeriodTimeID", periodTimeID);
                    cmd.Parameters.AddWithValue("@PeriodTime", ddlPeriodTime.SelectedValue);
                    cmd.Parameters.AddWithValue("@IsBreakTime", chkIsBreakTime.Checked);
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@Action", action);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ResetForm();
            BindPeriodTimeGridview();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            ddlPeriodTime.SelectedIndex = 0;
            chkIsBreakTime.Checked = false;
            btnSave.Text = "Save";
            ViewState["PeriodTimeID"] = 0;
        }

        protected void gvPeriodTime_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int periodTimeID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditPeriod")
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM PeriodTimeMaster WHERE PeriodTimeID = @PeriodTimeID", con))
                    {
                        cmd.Parameters.AddWithValue("@PeriodTimeID", periodTimeID);
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            ddlPeriodTime.SelectedValue = dr["PeriodTime"].ToString();
                            chkIsBreakTime.Checked = Convert.ToBoolean(dr["IsBreakTime"]);
                            ViewState["PeriodTimeID"] = periodTimeID;
                            btnSave.Text = "Update";
                        }
                    }
                }
            }
            else if (e.CommandName == "DeletePeriod")
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM PeriodTimeMaster WHERE PeriodTimeID = @PeriodTimeID AND SchoolMaster_SchoolID = @SchoolID", con))
                    {
                        cmd.Parameters.AddWithValue("@PeriodTimeID", periodTimeID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                BindPeriodTimeGridview();
            }
        }
    }
}