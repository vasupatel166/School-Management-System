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
                LoadAssignedStandard();
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
                    FROM Standards s WHERE s.SchoolMaster_SchoolID = @SchoolID 
                    UNION ALL
                    SELECT d.DivisionID AS ID, d.DivisionName AS Name, d.SchoolMaster_SchoolID, 'Division' AS Type 
                    FROM Divisions d WHERE d.SchoolMaster_SchoolID = @SchoolID", conn))
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

        private void LoadAssignedStandard()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Standards_StandardID, s.StandardName FROM SubjectDetail AS sd 
                    INNER JOIN Standards AS s ON sd.Standards_StandardID = s.StandardID
                    WHERE sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchAssignedStandard.Items.Clear();
                    ddlSearchAssignedStandard.Items.Add(new ListItem("Select Standard", "0"));

                    while (reader.Read())
                    {
                        ddlSearchAssignedStandard.Items.Add(new ListItem(
                            reader["StandardName"].ToString(),
                            reader["Standards_StandardID"].ToString()));

                    }
                }
            }
        }

        protected void ddlSearchAssignedStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchAssignedStandard.SelectedValue != "0")
            {
                ddlSearchAssignedDivision.Enabled = true;
                LoadAssignedDivisions();
                btnAddRow.Enabled = false;
                ddlStandard.Enabled = false;
                ddlDivision.Enabled = false;
                lblNote.Text = "";
            }
            else
            {
                ddlSearchAssignedDivision.Enabled = false;
                lblNote.Text = "";
                ddlStandard.Enabled = true;
                ddlDivision.Enabled = true;
                btnAddRow.Enabled = true;
            }

            ddlSearchAssignedDivision.SelectedIndex = 0;
            ClearSubjectRepeater();
            ddlStandard.SelectedIndex = 0;
            ddlDivision.SelectedIndex = 0;
        }

        private void LoadAssignedDivisions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT sd.Divisions_DivisionID, d.DivisionName FROM SubjectDetail AS sd 
                    INNER JOIN Divisions AS d ON sd.Divisions_DivisionID = d.DivisionID
                    WHERE sd.Standards_StandardID = @StandardID AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                    {
                    cmd.Parameters.AddWithValue("@StandardID", ddlSearchAssignedStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlSearchAssignedDivision.Items.Clear();
                    ddlSearchAssignedDivision.Items.Add(new ListItem("Select Division", "0"));

                    while (reader.Read())
                    {
                        ddlSearchAssignedDivision.Items.Add(new ListItem(
                            reader["DivisionName"].ToString(),
                            reader["Divisions_DivisionID"].ToString()));

                    }
                }
            }
        }

        protected void ddlSearchAssignedDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSearchAssignedDivision.SelectedValue != "0")
            {
                lblNote.Text = "Note : Subjects without assigned teachers are shown below. Remove teacher assignment to see more options.";
                LoadExistingSubjects();
            }
            else
            {
                lblNote.Text = ""; 
                ddlStandard.SelectedIndex = 0;
                ddlDivision.SelectedIndex = 0;
                ClearSubjectRepeater();
            }
        }

        //private void LoadExistingSubjects()
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand(@"
        //        SELECT sm.SubjectID, sm.SubjectName
        //        FROM SubjectDetail sd
        //        INNER JOIN SubjectMaster sm ON sd.SubjectMaster_SubjectID = sm.SubjectID
        //        WHERE sd.Standards_StandardID = @StandardID 
        //        AND sd.Divisions_DivisionID = @DivisionID 
        //        AND sd.SchoolMaster_SchoolID = @SchoolID
        //        AND sd.Teachers_TeacherID IS NULL", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@StandardID", ddlSearchAssignedStandard.SelectedValue);
        //            cmd.Parameters.AddWithValue("@DivisionID", ddlSearchAssignedDivision.SelectedValue);
        //            cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

        //            List<SubjectSelection> subjects = new List<SubjectSelection>();
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                subjects.Add(new SubjectSelection { SelectedSubjectId = reader["SubjectID"].ToString() });
        //            }

        //            ViewState["Subjects"] = subjects;
        //            rptSubjects.DataSource = subjects;
        //            rptSubjects.DataBind();

        //            ddlStandard.SelectedValue = ddlSearchAssignedStandard.SelectedValue;
        //            ddlDivision.SelectedValue = ddlSearchAssignedDivision.SelectedValue;

        //            LoadSubjectsForDropdowns();
        //        }
        //    }
        //}

        private void LoadExistingSubjects()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT sm.SubjectID, sm.SubjectName FROM SubjectDetail sd
                INNER JOIN SubjectMaster sm ON sd.SubjectMaster_SubjectID = sm.SubjectID
                WHERE sd.Standards_StandardID = @StandardID AND sd.Divisions_DivisionID = @DivisionID AND sd.SchoolMaster_SchoolID = @SchoolID AND sd.Teachers_TeacherID IS NULL", conn))
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

                    reader.Close();

                    // Check if any data was retrieved
                    if (subjects.Count == 0)
                    {
                        lblNote.Text = "No subjects without assigned teachers were found.";
                        lblNote.CssClass = "fw-bold text-danger";
                    }
                    else
                    {
                        lblNote.Text = "Note : Subjects without assigned teachers are shown below. Remove teacher assignment to see more options.";
                        lblNote.CssClass = "fw-bold";
                    }

                    ViewState["Subjects"] = subjects;
                    rptSubjects.DataSource = subjects;
                    rptSubjects.DataBind();

                    ddlStandard.SelectedValue = ddlSearchAssignedStandard.SelectedValue;
                    ddlDivision.SelectedValue = ddlSearchAssignedDivision.SelectedValue;

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
            for (int i = 0; i < subjects.Count - 1; i++)
            {
                DropDownList ddl = (DropDownList)rptSubjects.Items[i].FindControl("ddlSubject");
                if (!string.IsNullOrEmpty(subjects[i].SelectedSubjectId))
                {
                    ddl.SelectedValue = subjects[i].SelectedSubjectId;
                }
            }
        }

        //private void LoadSubjectsForDropdowns()
        //{
        //    List<string> selectedSubjects = new List<string>();
        //    List<SubjectSelection> subjects = (List<SubjectSelection>)ViewState["Subjects"];

        //    // First, collect all currently selected subjects
        //    foreach (RepeaterItem item in rptSubjects.Items)
        //    {
        //        DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
        //        if (ddl.SelectedValue != "")
        //        {
        //            selectedSubjects.Add(ddl.SelectedValue);
        //        }
        //    }

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand(@"
        //            SELECT sm.SubjectID, sm.SubjectName, 
        //                CASE 
        //                    WHEN sd.SubjectMaster_SubjectID IS NOT NULL 
        //                        AND sd.Standards_StandardID = @StandardID 
        //                        AND sd.Divisions_DivisionID = @DivisionID
        //                    THEN 1 
        //                    ELSE 0 
        //                END AS IsInUse 
        //            FROM SubjectMaster sm
        //            LEFT JOIN SubjectDetail sd ON sm.SubjectID = sd.SubjectMaster_SubjectID 
        //                AND sd.Standards_StandardID = @StandardID 
        //                AND sd.Divisions_DivisionID = @DivisionID
        //            WHERE sm.SchoolMaster_SchoolID = @SchoolID
        //        ", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
        //            cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
        //            cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
        //            DataTable dt = new DataTable();
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);

        //            foreach (RepeaterItem item in rptSubjects.Items)
        //            {
        //                DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
        //                string currentValue = item.ItemIndex < subjects.Count ?
        //                    subjects[item.ItemIndex].SelectedSubjectId : "";
        //                ddl.Items.Clear();
        //                ddl.Items.Add(new ListItem("Select Subject", ""));

        //                // Get list of subjects selected in above rows
        //                List<string> previouslySelectedSubjects = new List<string>();
        //                for (int i = 0; i < item.ItemIndex; i++)
        //                {
        //                    DropDownList previousDdl = (DropDownList)rptSubjects.Items[i].FindControl("ddlSubject");
        //                    if (!string.IsNullOrEmpty(previousDdl.SelectedValue))
        //                    {
        //                        previouslySelectedSubjects.Add(previousDdl.SelectedValue);
        //                    }
        //                }

        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    string subjectId = row["SubjectID"].ToString();
        //                    bool isInUse = Convert.ToBoolean(row["IsInUse"]);
        //                    string subjectName = row["SubjectName"].ToString();

        //                    // Skip if subject is already selected in above rows
        //                    if (previouslySelectedSubjects.Contains(subjectId) && subjectId != currentValue)
        //                    {
        //                        continue;
        //                    }

        //                    // Add subject based on whether it's in use
        //                    if (isInUse && subjectId != currentValue)
        //                    {
        //                        ddl.Items.Add(new ListItem(
        //                            subjectName + " (Assigned)",
        //                            ""));
        //                    }
        //                    else
        //                    {
        //                        ddl.Items.Add(new ListItem(
        //                            subjectName,
        //                            subjectId));
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(currentValue))
        //                {
        //                    ddl.SelectedValue = currentValue;
        //                }
        //            }
        //        }
        //    }
        //}

        private void LoadSubjectsForDropdowns()
        {
            List<string> selectedSubjects = new List<string>();
            List<SubjectSelection> subjects = (List<SubjectSelection>)ViewState["Subjects"];

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
                    SELECT sm.SubjectID, sm.SubjectName,
                        CASE 
                            WHEN sd.SubjectMaster_SubjectID IS NOT NULL 
                                AND sd.Standards_StandardID = @StandardID 
                                AND sd.Divisions_DivisionID = @DivisionID
                                AND sd.Teachers_TeacherID IS NULL
                            THEN 1 
                            ELSE 0 
                        END AS IsInUse,
                        CASE 
                            WHEN sd.SubjectMaster_SubjectID IS NOT NULL 
                                AND sd.Standards_StandardID = @StandardID 
                                AND sd.Divisions_DivisionID = @DivisionID
                                AND sd.Teachers_TeacherID IS NOT NULL
                            THEN 'Teacher Assigned' 
                            ELSE '' 
                        END AS TeacherStatus 
                    FROM SubjectMaster sm
                    LEFT JOIN SubjectDetail sd ON sm.SubjectID = sd.SubjectMaster_SubjectID 
                        AND sd.Standards_StandardID = @StandardID 
                        AND sd.Divisions_DivisionID = @DivisionID
                    WHERE sm.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (RepeaterItem item in rptSubjects.Items)
                    {
                        DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                        string currentValue = item.ItemIndex < subjects.Count ? subjects[item.ItemIndex].SelectedSubjectId : "";
                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Select Subject", ""));

                        // Get list of subjects selected in above rows
                        List<string> previouslySelectedSubjects = new List<string>();
                        for (int i = 0; i < item.ItemIndex; i++)
                        {
                            DropDownList previousDdl = (DropDownList)rptSubjects.Items[i].FindControl("ddlSubject");
                            if (!string.IsNullOrEmpty(previousDdl.SelectedValue))
                            {
                                previouslySelectedSubjects.Add(previousDdl.SelectedValue);
                            }
                        }

                        foreach (DataRow row in dt.Rows)
                        {
                            string subjectId = row["SubjectID"].ToString();
                            string subjectName = row["SubjectName"].ToString();
                            string teacherStatus = row["TeacherStatus"].ToString();
                            bool isInUse = Convert.ToBoolean(row["IsInUse"]);

                            // Skip if subject is already selected in above rows
                            if (previouslySelectedSubjects.Contains(subjectId) && subjectId != currentValue)
                            {
                                continue;
                            }

                            // Add subject based on whether it's in use
                            if (isInUse && subjectId != currentValue)
                            {
                                ddl.Items.Add(new ListItem(subjectName + " (Class Assigned)",""));
                            }
                            else if (teacherStatus == "Teacher Assigned")
                            {
                                ddl.Items.Add(new ListItem( subjectName + " (Teacher Assigned)", ""));
                            }
                            else
                            {
                                ddl.Items.Add(new ListItem(subjectName,subjectId));
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

        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    List<string> selectedSubjects = new List<string>();
        //    Dictionary<string, int> existingSubjectDetails = new Dictionary<string, int>();

        //    // Get all subject dropdowns from repeater
        //    foreach (RepeaterItem item in rptSubjects.Items)
        //    {
        //        DropDownList ddlSubject = (DropDownList)item.FindControl("ddlSubject");
        //        if (string.IsNullOrEmpty(ddlSubject.SelectedValue))
        //        {
        //            break;
        //        }
        //        selectedSubjects.Add(ddlSubject.SelectedValue);
        //    }

        //    // Validation checks
        //    if (rptSubjects.Items.Count == 0)
        //    {
        //        cvSubject.IsValid = false;
        //        cvSubject.ErrorMessage = "Minimum 1 subject required";
        //        return;
        //    }

        //    if (Page.IsValid)
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            try
        //            {
        //                // First, get existing SubjectDetailIDs if we're updating
        //                if (ddlSearchAssignedStandard.SelectedValue != "0" && ddlSearchAssignedDivision.SelectedValue != "0")
        //                {
        //                    using (SqlCommand cmdGet = new SqlCommand(@"
        //                    SELECT SubjectDetailID, SubjectMaster_SubjectID FROM SubjectDetail WHERE Standards_StandardID = @StandardID AND 
        //                    Divisions_DivisionID = @DivisionID AND Teachers_TeacherID = NULL AND SchoolMaster_SchoolID = @SchoolID", conn))
        //                    {
        //                        cmdGet.Parameters.AddWithValue("@StandardID", ddlSearchAssignedStandard.SelectedValue);
        //                        cmdGet.Parameters.AddWithValue("@DivisionID", ddlSearchAssignedDivision.SelectedValue);
        //                        cmdGet.Parameters.AddWithValue("@SchoolID", SchoolID);

        //                        using (SqlDataReader reader = cmdGet.ExecuteReader())
        //                        {
        //                            while (reader.Read())
        //                            {
        //                                existingSubjectDetails.Add(
        //                                    reader["SubjectMaster_SubjectID"].ToString(),
        //                                    Convert.ToInt32(reader["SubjectDetailID"])
        //                                );
        //                            }
        //                        }
        //                    }
        //                }

        //                // Process each selected subject
        //                foreach (string subjectId in selectedSubjects)
        //                {
        //                    using (SqlCommand cmd = new SqlCommand("InsertUpdateSubjectDetail", conn))
        //                    {
        //                        cmd.CommandType = CommandType.StoredProcedure;

        //                        // Add parameters
        //                        cmd.Parameters.AddWithValue("@SubjectMaster_SubjectID", Convert.ToInt32(subjectId));
        //                        cmd.Parameters.AddWithValue("@Standards_StandardID", Convert.ToInt32(ddlStandard.SelectedValue));
        //                        cmd.Parameters.AddWithValue("@Divisions_DivisionID", Convert.ToInt32(ddlDivision.SelectedValue));
        //                        cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);
        //                        cmd.Parameters.AddWithValue("@Teachers_TeacherID", DBNull.Value);

        //                        // If updating, add SubjectDetailID
        //                        if (existingSubjectDetails.ContainsKey(subjectId))
        //                        {
        //                            cmd.Parameters.AddWithValue("@SubjectDetailID", existingSubjectDetails[subjectId]);
        //                        }
        //                        else
        //                        {
        //                            cmd.Parameters.AddWithValue("@SubjectDetailID", DBNull.Value);
        //                        }

        //                        // Execute the stored procedure and get the result
        //                        using (SqlDataReader reader = cmd.ExecuteReader())
        //                        {
        //                            if (reader.Read())
        //                            {
        //                                string status = reader["Status"].ToString();
        //                                int subjectDetailId = Convert.ToInt32(reader["SubjectDetailID"]);
        //                                System.Diagnostics.Debug.WriteLine($"Subject {subjectId}: {status} with ID {subjectDetailId}");
        //                            }
        //                        }
        //                    }
        //                }

        //                // Delete records that are no longer selected (if updating)
        //                if (existingSubjectDetails.Count > 0)
        //                {
        //                    using (SqlCommand cmdDelete = new SqlCommand(@"
        //                    DELETE FROM SubjectDetail WHERE Standards_StandardID = @StandardID AND Divisions_DivisionID = @DivisionID AND SchoolMaster_SchoolID = @SchoolID 
        //                    AND SubjectMaster_SubjectID NOT IN (SELECT value FROM STRING_SPLIT(@SelectedSubjects, ','))", conn))
        //                    {
        //                        cmdDelete.Parameters.AddWithValue("@StandardID", ddlSearchAssignedStandard.SelectedValue);
        //                        cmdDelete.Parameters.AddWithValue("@DivisionID", ddlSearchAssignedDivision.SelectedValue);
        //                        cmdDelete.Parameters.AddWithValue("@SchoolID", SchoolID);
        //                        cmdDelete.Parameters.AddWithValue("@SelectedSubjects", string.Join(",", selectedSubjects));
        //                        cmdDelete.ExecuteNonQuery();
        //                    }
        //                }

        //                ResetForm();
        //            }
        //            catch (Exception ex)
        //            {
        //                ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"alert('An error occurred while saving the subjects: {ex.Message}');", true);
        //                System.Diagnostics.Debug.WriteLine($"Error in btnSubmit_Click: {ex}");
        //            }
        //        }
        //    }
        //}

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> selectedSubjects = new List<string>();
            Dictionary<string, int> existingSubjectDetails = new Dictionary<string, int>();

            foreach (RepeaterItem item in rptSubjects.Items)
            {
                DropDownList ddlSubject = (DropDownList)item.FindControl("ddlSubject");
                if (!string.IsNullOrEmpty(ddlSubject.SelectedValue))
                {
                    selectedSubjects.Add(ddlSubject.SelectedValue);
                }
            }

            if (Page.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    try
                    {
                        // Retrieve existing SubjectDetailIDs for selected standard and division
                        using (SqlCommand cmdGet = new SqlCommand(@"
                    SELECT SubjectDetailID, SubjectMaster_SubjectID 
                    FROM SubjectDetail 
                    WHERE Standards_StandardID = @StandardID 
                    AND Divisions_DivisionID = @DivisionID 
                    AND SchoolMaster_SchoolID = @SchoolID 
                    AND Teachers_TeacherID IS NULL", conn))
                        {
                            cmdGet.Parameters.AddWithValue("@StandardID", ddlSearchAssignedStandard.SelectedValue);
                            cmdGet.Parameters.AddWithValue("@DivisionID", ddlSearchAssignedDivision.SelectedValue);
                            cmdGet.Parameters.AddWithValue("@SchoolID", SchoolID);

                            using (SqlDataReader reader = cmdGet.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    existingSubjectDetails.Add(reader["SubjectMaster_SubjectID"].ToString(), Convert.ToInt32(reader["SubjectDetailID"]));
                                }
                            }
                        }

                        // Insert or update selected subjects
                        foreach (string subjectId in selectedSubjects)
                        {
                            using (SqlCommand cmd = new SqlCommand("InsertUpdateSubjectDetail", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SubjectMaster_SubjectID", Convert.ToInt32(subjectId));
                                cmd.Parameters.AddWithValue("@Standards_StandardID", Convert.ToInt32(ddlStandard.SelectedValue));
                                cmd.Parameters.AddWithValue("@Divisions_DivisionID", Convert.ToInt32(ddlDivision.SelectedValue));
                                cmd.Parameters.AddWithValue("@SchoolMaster_SchoolID", SchoolID);
                                cmd.Parameters.AddWithValue("@Teachers_TeacherID", DBNull.Value);

                                if (existingSubjectDetails.ContainsKey(subjectId))
                                {
                                    cmd.Parameters.AddWithValue("@SubjectDetailID", existingSubjectDetails[subjectId]);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@SubjectDetailID", DBNull.Value);
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Delete subjects that were unselected
                        foreach (var subject in existingSubjectDetails)
                        {
                            if (!selectedSubjects.Contains(subject.Key))
                            {
                                using (SqlCommand cmdDelete = new SqlCommand("DELETE FROM SubjectDetail WHERE SubjectDetailID = @SubjectDetailID", conn))
                                {
                                    cmdDelete.Parameters.AddWithValue("@SubjectDetailID", subject.Value);
                                    cmdDelete.ExecuteNonQuery();
                                }
                            }
                        }

                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "Error", $"alert('An error occurred: {ex.Message}');", true);
                        System.Diagnostics.Debug.WriteLine($"Error in btnSubmit_Click: {ex}");
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void ResetForm()
        {
            Response.Redirect("~/Admin/AssignSubjectToClass.aspx");
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubjectsForDropdowns();
            ClearSubjectRepeater();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubjectsForDropdowns();
            ClearSubjectRepeater();
        }

        private void ClearSubjectRepeater()
        {
            List<SubjectSelection> subjects = new List<SubjectSelection>();
            ViewState["Subjects"] = subjects;
            rptSubjects.DataSource = subjects;
            rptSubjects.DataBind();
        }
    }
}