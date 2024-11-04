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

        [Serializable]
        private class SubjectSelection
        {
            public string SelectedSubjectId { get; set; }
        }

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
            List<SubjectSelection> subjects = new List<SubjectSelection>();
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

                    ddlStandard.Items.Add(new ListItem("Select Standard", ""));
                    ddlDivision.Items.Add(new ListItem("Select Division", ""));

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

                    List<SubjectSelection> subjects = new List<SubjectSelection>();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        subjects.Add(new SubjectSelection { SelectedSubjectId = reader["SubjectID"].ToString() });
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
            List<SubjectSelection> subjects = new List<SubjectSelection>();
            if (ViewState["Subjects"] != null)
            {
                subjects = ((List<SubjectSelection>)ViewState["Subjects"]);
            }

            // Store current selections before adding new row
            foreach (RepeaterItem item in rptSubjects.Items)
            {
                DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                if (item.ItemIndex < subjects.Count)
                {
                    subjects[item.ItemIndex].SelectedSubjectId = ddl.SelectedValue;
                }
            }

            // Add new empty selection
            subjects.Add(new SubjectSelection());

            ViewState["Subjects"] = subjects;
            rptSubjects.DataSource = subjects;
            rptSubjects.DataBind();

            // Load subjects for each dropdown
            LoadSubjectsForDropdowns();

            // Restore previous selections
            for (int i = 0; i < subjects.Count - 1; i++) // Exclude the newly added row
            {
                DropDownList ddl = (DropDownList)rptSubjects.Items[i].FindControl("ddlSubject");
                if (!string.IsNullOrEmpty(subjects[i].SelectedSubjectId))
                {
                    ddl.SelectedValue = subjects[i].SelectedSubjectId;
                }
            }
        }

        private void LoadSubjectsForDropdowns()
        {
            List<string> selectedSubjects = new List<string>();
            List<SubjectSelection> subjects = (List<SubjectSelection>)ViewState["Subjects"];

            foreach (RepeaterItem item in rptSubjects.Items)
            {
                DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                if (ddl.SelectedValue != "" && item.ItemIndex < subjects.Count)
                {
                    selectedSubjects.Add(subjects[item.ItemIndex].SelectedSubjectId);
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
                        string currentValue = item.ItemIndex < subjects.Count ?
                            subjects[item.ItemIndex].SelectedSubjectId : "";

                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Select Subject", ""));

                        foreach (DataRow row in dt.Rows)
                        {
                            string subjectId = row["SubjectID"].ToString();
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
                List<SubjectSelection> subjects = (List<SubjectSelection>)ViewState["Subjects"];

                // Store current selections
                foreach (RepeaterItem item in rptSubjects.Items)
                {
                    if (item.ItemIndex != e.Item.ItemIndex)
                    {
                        DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                        int index = item.ItemIndex > e.Item.ItemIndex ? item.ItemIndex - 1 : item.ItemIndex;
                        if (index < subjects.Count)
                        {
                            subjects[index].SelectedSubjectId = ddl.SelectedValue;
                        }
                    }
                }

                subjects.RemoveAt(e.Item.ItemIndex);
                ViewState["Subjects"] = subjects;
                rptSubjects.DataSource = subjects;
                rptSubjects.DataBind();
                LoadSubjectsForDropdowns();

                // Restore selections
                foreach (RepeaterItem item in rptSubjects.Items)
                {
                    DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                    if (item.ItemIndex < subjects.Count)
                    {
                        ddl.SelectedValue = subjects[item.ItemIndex].SelectedSubjectId;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            // Get all subject dropdowns from repeater
            List<string> selectedSubjects = new List<string>();

            foreach (RepeaterItem item in rptSubjects.Items)
            {
                DropDownList ddlSubject = (DropDownList)item.FindControl("ddlSubject");
                if (string.IsNullOrEmpty(ddlSubject.SelectedValue))
                {
                    break;
                }
                selectedSubjects.Add(ddlSubject.SelectedValue);
            }

            // Check if no subjects are added or if any subject is not selected
            if (rptSubjects.Items.Count == 0)
            {
                cvSubject.IsValid = false;
                cvSubject.ErrorMessage = "Please add at least one subject";
                return;
            }

            if (Page.IsValid)
            {
                // Debug logging for all form inputs
                System.Diagnostics.Debug.WriteLine("=== Form Input Values ===");
                System.Diagnostics.Debug.WriteLine($"SchoolID: {SchoolID}");
                System.Diagnostics.Debug.WriteLine($"Selected Standard: {ddlStandard.SelectedItem?.Text} (ID: {ddlStandard.SelectedValue})");
                System.Diagnostics.Debug.WriteLine($"Selected Division: {ddlDivision.SelectedItem?.Text} (ID: {ddlDivision.SelectedValue})");

                System.Diagnostics.Debug.WriteLine("\nSelected Subjects:");
                foreach (RepeaterItem item in rptSubjects.Items)
                {
                    DropDownList ddlSubject = (DropDownList)item.FindControl("ddlSubject");
                    if (!string.IsNullOrEmpty(ddlSubject.SelectedValue))
                    {
                        System.Diagnostics.Debug.WriteLine($"Subject {item.ItemIndex + 1}: {ddlSubject.SelectedItem?.Text} (ID: {ddlSubject.SelectedValue})");
                    }
                }

                System.Diagnostics.Debug.WriteLine("\nSearch Dropdown Values:");
                System.Diagnostics.Debug.WriteLine($"Search Assigned Standard: {ddlSearchAssignedStandard.SelectedItem?.Text} (ID: {ddlSearchAssignedStandard.SelectedValue})");
                System.Diagnostics.Debug.WriteLine($"Search Assigned Division: {ddlSearchAssignedDivision.SelectedItem?.Text} (ID: {ddlSearchAssignedDivision.SelectedValue})");
                System.Diagnostics.Debug.WriteLine("========================");

                //using (SqlConnection conn = new SqlConnection(connectionString))
                //{
                //    conn.Open();
                //    SqlTransaction transaction = conn.BeginTransaction();

                //    try
                //    {
                //        // First, delete existing assignments for this standard and division
                //        using (SqlCommand deleteCmd = new SqlCommand(@"
                //                DELETE FROM SubjectDetail 
                //                WHERE Standards_StandardID = @StandardID 
                //                AND Divisions_DivisionID = @DivisionID 
                //                AND SchoolMaster_SchoolID = @SchoolID", conn, transaction))
                //        {
                //            deleteCmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                //            deleteCmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                //            deleteCmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                //            deleteCmd.ExecuteNonQuery();
                //        }

                //        // Then insert new assignments
                //        using (SqlCommand insertCmd = new SqlCommand(@"
                //                INSERT INTO SubjectDetail (
                //                    Standards_StandardID, 
                //                    Divisions_DivisionID, 
                //                    SubjectMaster_SubjectID, 
                //                    SchoolMaster_SchoolID,
                //                    Teachers_TeacherID
                //                ) VALUES (
                //                    @StandardID,
                //                    @DivisionID,
                //                    @SubjectID,
                //                    @SchoolID,
                //                    NULL
                //                )", conn, transaction))
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
                ScriptManager.RegisterStartupScript(this, GetType(), "Success",
                    "alert('Subjects assigned successfully!'); window.location='AssignSubjectToClass.aspx';", true);
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