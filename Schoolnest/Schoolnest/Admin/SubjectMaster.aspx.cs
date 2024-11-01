using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class SubjectMaster : System.Web.UI.Page
    {

        private string connectionString = Global.ConnectionString; // Connection string
        private string SelectedSubjectID = "";
        private string SchoolID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"].ToString();

            if (!IsPostBack)
            {
                LoadSubjectSearchDropdown();
            }
        }

        private void LoadSubjectSearchDropdown() 
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SubjectID, SubjectName FROM SubjectMaster WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string displayName = $"{reader["SubjectName"]}";
                        ListItem listItem = new ListItem(displayName, reader["SubjectID"].ToString());

                        ddlSearchSubject.Items.Add(listItem);
                    }
                }
            }

            ddlSearchSubject.Items.Insert(0, new ListItem("Select Subject", "0"));
        }

        protected void ddlSearchSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Code to handle the event when a different item is selected from the dropdown
            SelectedSubjectID = ddlSearchSubject.SelectedValue;

            if (!string.IsNullOrEmpty(SelectedSubjectID) && SelectedSubjectID != "0")
            {
                // Load the selected admin details into the form fields
                LoadSubjectDetails(SelectedSubjectID);
            }
            else
            {
                // Reset the form if no admin is selected
                ResetForm();
            }
        }

        private void LoadSubjectDetails(string SubjectID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SubjectName, SubjectCode FROM SubjectMaster WHERE SubjectID = @SubjectID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate form fields with data
                            txtSubjectName.Text = reader["SubjectName"].ToString();
                            txtSubjectCode.Text = reader["SubjectCode"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Code to handle form submission
            string SubjectName = txtSubjectName.Text;
            string SubjectCode = txtSubjectCode.Text;
            int SubjectID = string.IsNullOrEmpty(ddlSearchSubject.SelectedValue) ? 0 : int.Parse(ddlSearchSubject.SelectedValue);

            if (Page.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("InsertUpdateSubjectMaster", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                            cmd.Parameters.AddWithValue("@SubjectName", SubjectName);
                            cmd.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                            cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    ResetForm();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Insert or Update Subject Master Failed. " + ex.Message);
                }
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            Response.Redirect("~/Admin/SubjectMaster.aspx");
        }
    }
}