using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class HolidayMaster : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            if (!IsPostBack)
            {
                BindHolidayGridview();
                btnSave.Text = "Save";
                ViewState["HolidayID"] = 0;
            }
        }

        private void BindHolidayGridview()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM HolidayMaster WHERE SchoolMaster_SchoolID = @SchoolID ORDER BY HolidayDate", con))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        gvHolidays.DataSource = dt;
                        gvHolidays.DataBind();
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int holidayID = Convert.ToInt32(ViewState["HolidayID"]);
            string action = holidayID == 0 ? "INSERT" : "UPDATE";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUpdateHolidayMaster", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HolidayID", holidayID);
                    cmd.Parameters.AddWithValue("@HolidayName", txtHolidayName.Text.Trim());
                    cmd.Parameters.AddWithValue("@HolidayDate", Convert.ToDateTime(dpHolidayDate.Text));
                    cmd.Parameters.AddWithValue("@AcademicYear", txtAcademicYear.Text.Trim());
                    cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@Action", action);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ResetForm();
            BindHolidayGridview();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtHolidayName.Text = string.Empty;
            dpHolidayDate.Text = string.Empty;
            txtAcademicYear.Text = string.Empty;
            btnSave.Text = "Save";
            ViewState["HolidayID"] = 0;
        }

        protected void gvHolidays_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int holidayID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditHoliday")
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM HolidayMaster WHERE HolidayID = @HolidayID", con))
                    {
                        cmd.Parameters.AddWithValue("@HolidayID", holidayID);
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            txtHolidayName.Text = dr["HolidayName"].ToString();
                            dpHolidayDate.Text = Convert.ToDateTime(dr["HolidayDate"]).ToString("yyyy-MM-dd");
                            txtAcademicYear.Text = dr["AcademicYear"].ToString();
                            ViewState["HolidayID"] = holidayID;
                            btnSave.Text = "Update";
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteHoliday")
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM HolidayMaster WHERE HolidayID = @HolidayID AND SchoolMaster_SchoolID = @SchoolID", con))
                    {
                        cmd.Parameters.AddWithValue("@HolidayID", holidayID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                BindHolidayGridview();
            }
        }
    }
}