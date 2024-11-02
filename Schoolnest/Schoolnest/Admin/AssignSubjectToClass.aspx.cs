using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class AssignSubjectToClass : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadAssignedClassSearchDropdown();
                LoadStandardsWithDivisions();
                InitializeSubjectRepeater();
            }
        }

        private void InitializeSubjectRepeater()
        {
            List<object> subjects = new List<object>();
            ViewState["Subjects"] = subjects;
            rptSubjects.DataSource = subjects;
            rptSubjects.DataBind();
        }

        private void LoadStandardsWithDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT s.StandardID AS ID, s.StandardName AS Name, s.SchoolMaster_SchoolID, 'Standard' AS Type 
                    FROM Standards s 
                    WHERE s.SchoolMaster_SchoolID = @SchoolID 
                    UNION ALL
                    SELECT d.DivisionID AS ID, d.DivisionName AS Name, d.SchoolMaster_SchoolID, 'Division' AS Type 
                    FROM Divisions d 
                    WHERE d.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlStandard.Items.Clear();
                    ddlDivision.Items.Clear();

                    ddlStandard.Items.Add(new ListItem("Select Standard", "0"));
                    ddlDivision.Items.Add(new ListItem("Select Division", "0"));

                    while (reader.Read())
                    {
                        string id = reader["ID"].ToString();
                        string name = reader["Name"].ToString();
                        string type = reader["Type"].ToString();

                        if (type == "Standard")
                        {
                            ddlStandard.Items.Add(new ListItem(name, id));
                        }
                        else if (type == "Division")
                        {
                            ddlDivision.Items.Add(new ListItem(name, id));
                        }
                    }
                }
            }
        }

        private void LoadAssignedClassSearchDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Standards_StandardID, sd.Divisions_DivisionID, s.StandardName, d.DivisionName 
                    FROM SubjectDetail AS sd 
                    INNER JOIN Standards AS s ON sd.Standards_StandardID = s.StandardID
                    INNER JOIN Divisions AS d ON sd.Divisions_DivisionID = d.DivisionID 
                    WHERE sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchAssignedStandard.Items.Clear();
                    ddlSearchAssignedDivision.Items.Clear();

                    ddlSearchAssignedStandard.Items.Add(new ListItem("Select Standard", "0"));
                    ddlSearchAssignedDivision.Items.Add(new ListItem("Select Division", "0"));

                    while (reader.Read())
                    {
                        ddlSearchAssignedStandard.Items.Add(new ListItem(
                            reader["StandardName"].ToString(),
                            reader["Standards_StandardID"].ToString()));

                        ddlSearchAssignedDivision.Items.Add(new ListItem(
                            reader["DivisionName"].ToString(),
                            reader["Divisions_DivisionID"].ToString()));
                    }
                }
            }
        }

        protected void ddlSearchAssignedStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchAssignedStandard.SelectedValue != "0")
            {
                ddlSearchAssignedDivision.Enabled = true;
            }
            else
            {
                ddlSearchAssignedDivision.Enabled = false;
            }
        }
        protected void ddlSearchAssignedDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchAssignedDivision.SelectedValue != "0")
            {
                LoadExistingSubjects();
            }
        }

        private void LoadExistingSubjects()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT sm.SubjectID, sm.SubjectName
                    FROM SubjectDetail sd
                    INNER JOIN SubjectMaster sm ON sd.SubjectMaster_SubjectID = sm.SubjectID
                    WHERE sd.Standards_StandardID = @StandardID 
                    AND sd.Divisions_DivisionID = @DivisionID 
                    AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlSearchAssignedStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlSearchAssignedDivision.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    List<object> subjects = new List<object>();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        subjects.Add(new object()); // Add placeholder for each subject
                    }

                    ViewState["Subjects"] = subjects;
                    rptSubjects.DataSource = subjects;
                    rptSubjects.DataBind();
                    LoadSubjectsForDropdowns();
                }
            }
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            List<object> subjects = new List<object>();
            if (ViewState["Subjects"] != null)
            {
                subjects = ((List<object>)ViewState["Subjects"]);
            }
            subjects.Add(new object());

            ViewState["Subjects"] = subjects;
            rptSubjects.DataSource = subjects;
            rptSubjects.DataBind();

            // Load subjects for each dropdown
            LoadSubjectsForDropdowns();
        }

        private void LoadSubjectsForDropdowns()
        {
            // Get currently selected subjects
            List<string> selectedSubjects = new List<string>();
            foreach (RepeaterItem item in rptSubjects.Items)
            {
                DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                if (ddl.SelectedValue != "")
                {
                    selectedSubjects.Add(ddl.SelectedValue);
                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT SubjectID, SubjectName 
                    FROM SubjectMaster 
                    WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (RepeaterItem item in rptSubjects.Items)
                    {
                        DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                        string currentValue = ddl.SelectedValue;
                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Select Subject", ""));

                        foreach (DataRow row in dt.Rows)
                        {
                            string subjectId = row["SubjectID"].ToString();
                            // Only add subject if it's not already selected in another dropdown or if it's the current value
                            if (!selectedSubjects.Contains(subjectId) || subjectId == currentValue)
                            {
                                ddl.Items.Add(new ListItem(
                                    row["SubjectName"].ToString(),
                                    subjectId));
                            }
                        }

                        if (!string.IsNullOrEmpty(currentValue))
                        {
                            ddl.SelectedValue = currentValue;
                        }
                    }
                }
            }
        }

        protected void rptSubjects_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                List<object> subjects = (List<object>)ViewState["Subjects"];
                subjects.RemoveAt(e.Item.ItemIndex);
                ViewState["Subjects"] = subjects;
                rptSubjects.DataSource = subjects;
                rptSubjects.DataBind();
                LoadSubjectsForDropdowns();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //using (SqlConnection conn = new SqlConnection(connectionString))
                //{
                //    conn.Open();
                //    SqlTransaction transaction = conn.BeginTransaction();

                //    try
                //    {
                //        // First, delete existing assignments for this standard and division
                //        using (SqlCommand deleteCmd = new SqlCommand(@"
                //            DELETE FROM SubjectDetail 
                //            WHERE Standards_StandardID = @StandardID 
                //            AND Divisions_DivisionID = @DivisionID 
                //            AND SchoolMaster_SchoolID = @SchoolID", conn, transaction))
                //        {
                //            deleteCmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                //            deleteCmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                //            deleteCmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                //            deleteCmd.ExecuteNonQuery();
                //        }

                //        // Then insert new assignments
                //        using (SqlCommand insertCmd = new SqlCommand(@"
                //            INSERT INTO SubjectDetail (
                //                Standards_StandardID, 
                //                Divisions_DivisionID, 
                //                SubjectMaster_SubjectID, 
                //                SchoolMaster_SchoolID,
                //                Teachers_TeacherID
                //            ) VALUES (
                //                @StandardID,
                //                @DivisionID,
                //                @SubjectID,
                //                @SchoolID,
                //                NULL
                //            )", conn, transaction))
                //        {
                //            foreach (RepeaterItem item in rptSubjects.Items)
                //            {
                //                DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                //                if (!string.IsNullOrEmpty(ddl.SelectedValue))
                //                {
                //                    insertCmd.Parameters.Clear();
                //                    insertCmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                //                    insertCmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                //                    insertCmd.Parameters.AddWithValue("@SubjectID", ddl.SelectedValue);
                //                    insertCmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                //                    insertCmd.ExecuteNonQuery();
                //                }
                //            }
                //        }

                //        transaction.Commit();
                //        ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                //            "alert('Subjects assigned successfully!'); window.location='AssignSubjectToClass.aspx';", true);
                //    }
                //    catch (Exception ex)
                //    {
                //        transaction.Rollback();
                //        ScriptManager.RegisterStartupScript(this, GetType(), "Error",
                //            $"alert('Error: {ex.Message}');", true);
                //    }
                //}
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AssignSubjectToClass.aspx");
        }
    }
}