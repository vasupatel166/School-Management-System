using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class ExamMaster : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();
            if (!IsPostBack)
            {
                ViewState["ExamID"] = 0;
                BindExamGridview();
            }
        }

        protected void BindExamGridview()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ExamID, ExamName, ExamDescription, TotalMarks FROM Exam WHERE SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        gvExams.DataSource = dt;
                        gvExams.DataBind();
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int examID = Convert.ToInt32(ViewState["ExamID"]);
            string examName = txtExamName.Text.Trim();
            string description = txtExamDescription.Text.Trim();
            int totalMarks = int.Parse(txtTotalMarks.Text);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUpdateExam", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamID", examID == 0 ? (object)DBNull.Value : examID);
                    cmd.Parameters.AddWithValue("@ExamName", examName);
                    cmd.Parameters.AddWithValue("@ExamDescription", description);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@TotalMarks", totalMarks);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            ResetForm();
            BindExamGridview();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtExamName.Text = string.Empty;
            txtExamDescription.Text = string.Empty;
            txtTotalMarks.Text = string.Empty;
            btnSave.Text = "Save";
            ViewState["ExamID"] = 0;
        }

        protected void gvExams_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int examID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditExam")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT ExamID, ExamName, ExamDescription, TotalMarks FROM Exam WHERE ExamID = @ExamID AND SchoolID = @SchoolID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ExamID", examID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            txtExamName.Text = dr["ExamName"].ToString();
                            txtExamDescription.Text = dr["ExamDescription"].ToString();
                            txtTotalMarks.Text = dr["TotalMarks"].ToString();
                            ViewState["ExamID"] = examID;
                            btnSave.Text = "Update";
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteExam")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Exam WHERE ExamID = @ExamID AND SchoolID = @SchoolID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ExamID", examID);
                        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                BindExamGridview();
            }
        }
    }
}
