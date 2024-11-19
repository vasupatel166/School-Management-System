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
    public partial class AssignTeacher : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var context = HttpContext.Current;
                string schoolID = context.Session["SchoolID"]?.ToString();
                BindDropdowns(schoolID);
                BindTeachersGrid(schoolID);
            }
        }

        private void BindTeachersGrid(string schoolid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT AT.AssignmentID, TM.TeacherName, SD.StandardName AS [Standard], DV.DivisionName AS [Division], SC.SectionName AS [Section] " +
                                                        "FROM AssignTeacher AT " +
                                                        "LEFT JOIN TeacherMaster TM ON TM.TeacherID = AT.TeacherID " +
                                                        "LEFT JOIN Standards SD ON SD.StandardID = AT.StandardID " +
                                                        "LEFT JOIN Divisions DV ON DV.DivisionID = AT.DivisionID " +
                                                        "LEFT JOIN Sections SC ON SC.SectionID = AT.SectionID " +
                                                        "WHERE AT.SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolid);
                    conn.Open();
                    gvAssignments.DataSource = cmd.ExecuteReader();
                    gvAssignments.DataBind();
                }
            }
        }

        private void BindDropdowns(string schoolID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDropdownDataAssignTeacher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolID));
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Bind Teachers dropdown
                        if (reader.HasRows)
                        {
                            ddlTeacher.DataSource = reader;
                            ddlTeacher.DataTextField = "TeacherName";
                            ddlTeacher.DataValueField = "TeacherID";
                            ddlTeacher.DataBind();
                            ddlTeacher.Items.Insert(0, new ListItem("--Select Teacher--", "0"));
                        }

                        // Move to next result set (Sections)
                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlSection.DataSource = reader;
                            ddlSection.DataTextField = "SectionName";
                            ddlSection.DataValueField = "SectionID";
                            ddlSection.DataBind();
                            ddlSection.Items.Insert(0, new ListItem("--Select Section--", "0"));
                        }

                        // Move to next result set (Standards)
                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlStandard.DataSource = reader;
                            ddlStandard.DataTextField = "StandardName";
                            ddlStandard.DataValueField = "StandardID";
                            ddlStandard.DataBind();
                            ddlStandard.Items.Insert(0, new ListItem("--Select Standard--", "0"));
                        }

                        // Move to next result set (Divisions)
                        if (reader.NextResult() && reader.HasRows)
                        {
                            ddlDivision.DataSource = reader;
                            ddlDivision.DataTextField = "DivisionName";
                            ddlDivision.DataValueField = "DivisionID";
                            ddlDivision.DataBind();
                            ddlDivision.Items.Insert(0, new ListItem("--Select Division--", "0"));
                        }
                    }
                }
            }
            }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();
            AssignTeacherToClass();
            gvAssignments.EditIndex = -1;
            BindTeachersGrid(schoolID);
        }

        private void AssignTeacherToClass()
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUpdateAssignTeacher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AssignmentID", txtAssignmentID.Text));
                    cmd.Parameters.Add(new SqlParameter("@TeacherID", ddlTeacher.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@StandardID", ddlStandard.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", ddlDivision.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@SectionID", ddlSection.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@SchoolID", schoolID));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (string.IsNullOrEmpty(txtAssignmentID.Text))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Teacher Assigned Successfully');", true);
                            ClearForm();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Assignment Updated Successfully');", true);
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                    }
                }
            }
        }
        protected void gvAssignments_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // First, get the AssignmentID from the GridView and populate the form with this record's data
            GridViewRow row = gvAssignments.Rows[e.NewEditIndex];
            string assignmentID = row.Cells[0].Text;

            // Ensure the dropdowns are populated
            var schoolID = HttpContext.Current.Session["SchoolID"]?.ToString();
            BindDropdowns(schoolID);

            // Assign values to the form fields
            txtAssignmentID.Text = assignmentID;

            // Set the selected values if they exist in the dropdown list
            SetDropdownSelectedValue(ddlTeacher, row.Cells[1].Text);
            SetDropdownSelectedValue(ddlStandard, row.Cells[2].Text);
            SetDropdownSelectedValue(ddlDivision, row.Cells[3].Text);
            SetDropdownSelectedValue(ddlSection, row.Cells[4].Text);
        }

        private void SetDropdownSelectedValue(DropDownList ddl, string value)
        {
            ListItem item = ddl.Items.FindByText(value);
            if (item != null)
            {
                ddl.SelectedValue = item.Value;
            }
            else
            {
                ddl.SelectedIndex = 0; // Optionally, set to a default if the value is missing
            }
        }

        protected void gvAssignments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var context = HttpContext.Current;
            string schoolID = context.Session["SchoolID"]?.ToString();

            // Get the AssignmentID of the row being deleted
            int assignmentID = Convert.ToInt32(gvAssignments.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM AssignTeacher WHERE AssignmentID = @AssignmentID", conn))
                {
                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Rebind the GridView to refresh the data
            BindTeachersGrid(schoolID);
        }


        protected void gvAssignments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the delete button in the row
                Button deleteButton = e.Row.Cells[5].Controls.OfType<Button>().FirstOrDefault(btn => btn.CommandName == "Delete");

                if (deleteButton != null)
                {
                    // Add a JavaScript confirmation dialog to the delete button
                    deleteButton.OnClientClick = "return confirm('Are you sure you want to delete this assignment?');";
                }
            }
        }

        private void ClearForm()
        {
            txtAssignmentID.Text = string.Empty;
            ddlTeacher.SelectedIndex = 0;
            ddlStandard.SelectedIndex = 0;
            ddlSection.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
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