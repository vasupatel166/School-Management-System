using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web;

namespace Schoolnest.Admin
{
    public partial class GradeManagement : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            
            if (!IsPostBack)
            {
                LoadGrades();
            }
        }

        private void LoadGrades()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("select * from GradeManagement Where SchoolID = @SchoolID ", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();
                    gvGrades.DataSource = cmd.ExecuteReader();
                    gvGrades.DataBind();
                }
            }
        }

        private void ClearForm()
        {
            txtGrade.Text = string.Empty;
            txtMaxMarks.Text = string.Empty;
            txtMinMarks.Text = string.Empty;
            hfCriteriaID.Value = string.Empty;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int GradesID;
            bool isUpdate = int.TryParse(hfCriteriaID.Value, out GradesID);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd;

                if (isUpdate)
                {
                    // Update existing record
                    cmd = new SqlCommand("UPDATE GradeManagement SET Grade = @Grade, MinMarks = @MinMarks, MaxMarks = @MaxMarks WHERE GradesID = @GradesID AND SchoolID = @SchoolID", conn);
                    cmd.Parameters.AddWithValue("@GradesID", GradesID);
                }
                else
                {
                    // Add new record
                    cmd = new SqlCommand("INSERT INTO GradeManagement (Grade, MinMarks, MaxMarks, SchoolID) VALUES (@Grade, @MinMarks, @MaxMarks, @SchoolID)", conn);
                }

                // Add parameters common to both insert and update
                cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
                cmd.Parameters.AddWithValue("@MinMarks", int.Parse(txtMinMarks.Text));
                cmd.Parameters.AddWithValue("@MaxMarks", int.Parse(txtMaxMarks.Text));
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                // Display success message
                ClientScript.RegisterStartupScript(this.GetType(), "alert", isUpdate ? "alert('Grades Updated Successfully');" : "alert('Grades Inserted Successfully');", true);

                ClearForm(); // Clear the form and hidden field after saving
                gvGrades.EditIndex = -1;
                LoadGrades();
            }
        }



        protected void gvGrades_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (gvGrades.DataKeys[e.NewEditIndex] != null) 
            {
                int GradesID = Convert.ToInt32(gvGrades.DataKeys[e.NewEditIndex].Value);
                hfCriteriaID.Value = GradesID.ToString();

                // Load data for editing
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM GradeManagement WHERE GradesID = @GradesID and schoolID=@SchoolID", conn);
                    cmd.Parameters.AddWithValue("@GradesID", GradesID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtGrade.Text = reader["Grade"].ToString();
                        txtMinMarks.Text = reader["MinMarks"].ToString();
                        txtMaxMarks.Text = reader["MaxMarks"].ToString();
                    }
                }
            }
        }

        protected void gvGrades_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int GradesID = Convert.ToInt32(gvGrades.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM GradeManagement WHERE GradesID = @GradesID and schoolID=@SchoolID", conn);
                cmd.Parameters.AddWithValue("@GradesID", GradesID);
                cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                conn.Open();
                cmd.ExecuteNonQuery();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Grades Deleted Successfully');", true);
                LoadGrades();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
    
}