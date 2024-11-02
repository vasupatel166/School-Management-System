using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Schoolnest.Admin
{
    public partial class AssignTeacherToSubject : System.Web.UI.Page
    {
        private string connectionString = Global.ConnectionString;
        private string SchoolID;

        [Serializable]
        private class SubjectTeacherPair
        {
            public string SubjectID { get; set; }
            public string TeacherID { get; set; }

            public SubjectTeacherPair()
            {
                SubjectID = "0";
                TeacherID = "0";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SchoolID = Session["SchoolID"]?.ToString();

            if (!IsPostBack)
            {
                LoadAssignedClassSearchDropdown();
                LoadStandardsWithDivisions();
                InitializeSubjectTeacherRepeater();
            }
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

        private void InitializeSubjectTeacherRepeater()
        {
            List<SubjectTeacherPair> subjectTeacherPairs = new List<SubjectTeacherPair>();
            ViewState["SubjectTeacherPairs"] = subjectTeacherPairs;
            rptSubjectsTeacher.DataSource = subjectTeacherPairs;
            rptSubjectsTeacher.DataBind();
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

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            List<SubjectTeacherPair> pairs = ViewState["SubjectTeacherPairs"] as List<SubjectTeacherPair> ?? new List<SubjectTeacherPair>();
            pairs.Add(new SubjectTeacherPair());
            ViewState["SubjectTeacherPairs"] = pairs;

            rptSubjectsTeacher.DataSource = pairs;
            rptSubjectsTeacher.DataBind();

            LoadSubjectsForDropdowns();
            LoadTeachersForDropdowns();
        }

        private void LoadSubjectsForDropdowns()
        {
            List<SubjectTeacherPair> pairs = ViewState["SubjectTeacherPairs"] as List<SubjectTeacherPair>;
            List<string> selectedSubjects = new List<string>();

            if (pairs != null)
            {
                foreach (var pair in pairs)
                {
                    if (!string.IsNullOrEmpty(pair.SubjectID) && pair.SubjectID != "0")
                    {
                        selectedSubjects.Add(pair.SubjectID);
                    }
                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT sm.SubjectID, sm.SubjectName 
                    FROM SubjectMaster as sm 
                    INNER JOIN SubjectDetail as sd ON sd.SubjectMaster_SubjectID = sm.SubjectID
                    WHERE sd.Standards_StandardID = @StandardID 
                    AND sd.Divisions_DivisionID = @DivisionID 
                    AND sd.SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@StandardID", ddlStandard.SelectedValue);
                    cmd.Parameters.AddWithValue("@DivisionID", ddlDivision.SelectedValue);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    for (int i = 0; i < rptSubjectsTeacher.Items.Count; i++)
                    {
                        RepeaterItem item = rptSubjectsTeacher.Items[i];
                        DropDownList ddl = (DropDownList)item.FindControl("ddlSubject");
                        string currentValue = pairs[i].SubjectID;

                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Select Subject", "0"));

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

                        if (!string.IsNullOrEmpty(currentValue) && currentValue != "0")
                        {
                            ddl.SelectedValue = currentValue;
                        }
                    }
                }
            }
        }

        private void LoadTeachersForDropdowns()
        {
            List<SubjectTeacherPair> pairs = ViewState["SubjectTeacherPairs"] as List<SubjectTeacherPair>;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TeacherID, Teacher_Firstname, Teacher_Lastname 
                    FROM TeacherMaster 
                    WHERE SchoolMaster_SchoolID = @SchoolID AND IsActive = 1
                    ORDER BY Teacher_Firstname, Teacher_Lastname", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    for (int i = 0; i < rptSubjectsTeacher.Items.Count; i++)
                    {
                        RepeaterItem item = rptSubjectsTeacher.Items[i];
                        DropDownList ddl = (DropDownList)item.FindControl("ddlTeacher");
                        string currentValue = pairs[i].TeacherID;

                        ddl.Items.Clear();
                        ddl.Items.Add(new ListItem("Select Teacher", "0"));

                        foreach (DataRow row in dt.Rows)
                        {
                            string teacherName = $"{row["Teacher_Firstname"]} {row["Teacher_Lastname"]}";
                            ddl.Items.Add(new ListItem(
                                teacherName,
                                row["TeacherID"].ToString()));
                        }

                        if (!string.IsNullOrEmpty(currentValue) && currentValue != "0")
                        {
                            ddl.SelectedValue = currentValue;
                        }
                    }
                }
            }
        }

        protected void rptSubjectsTeacher_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlSubject = (DropDownList)e.Item.FindControl("ddlSubject");
                DropDownList ddlTeacher = (DropDownList)e.Item.FindControl("ddlTeacher");

                ddlSubject.AutoPostBack = true;
                ddlTeacher.AutoPostBack = true;

                ddlSubject.SelectedIndexChanged += new EventHandler(ddlSubject_SelectedIndexChanged);
                ddlTeacher.SelectedIndexChanged += new EventHandler(ddlTeacher_SelectedIndexChanged);
            }
        }

        protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            int index = item.ItemIndex;

            List<SubjectTeacherPair> pairs = ViewState["SubjectTeacherPairs"] as List<SubjectTeacherPair>;
            pairs[index].SubjectID = ddl.SelectedValue;
            ViewState["SubjectTeacherPairs"] = pairs;

            LoadSubjectsForDropdowns();
        }

        protected void ddlTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            int index = item.ItemIndex;

            List<SubjectTeacherPair> pairs = ViewState["SubjectTeacherPairs"] as List<SubjectTeacherPair>;
            pairs[index].TeacherID = ddl.SelectedValue;
            ViewState["SubjectTeacherPairs"] = pairs;
        }

        protected void rptSubjectsTeacher_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                List<SubjectTeacherPair> pairs = ViewState["SubjectTeacherPairs"] as List<SubjectTeacherPair>;
                pairs.RemoveAt(Convert.ToInt32(e.CommandArgument));
                ViewState["SubjectTeacherPairs"] = pairs;

                rptSubjectsTeacher.DataSource = pairs;
                rptSubjectsTeacher.DataBind();

                if (pairs.Count > 0)
                {
                    LoadSubjectsForDropdowns();
                    LoadTeachersForDropdowns();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AssignTeacherToClass.aspx");
        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStandard.SelectedValue != "0" && ddlDivision.SelectedValue != "0")
            {
                InitializeSubjectTeacherRepeater();
                LoadSubjectsForDropdowns();
                LoadTeachersForDropdowns();
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStandard.SelectedValue != "0" && ddlDivision.SelectedValue != "0")
            {
                InitializeSubjectTeacherRepeater();
                LoadSubjectsForDropdowns();
                LoadTeachersForDropdowns();
            }
        }
    }
}