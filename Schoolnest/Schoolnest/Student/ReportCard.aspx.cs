using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Schoolnest.Student
{
    public partial class ReportCard : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        string StudentID = string.Empty;
        int UserID;
        private int totalMarksSum = 0;
        private int marksObtainedSum = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            schoolId = Session["SchoolID"].ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetStudentID();

            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                ddlExam.Visible = false;
                lblExam.Visible = false;
                btnGenerateReport.Visible = false;

                // Reset the page content when it's first loaded
                ResetPage();

                // Hide student details and results section initially
                studentDetailsSection.Style["display"] = "none";
                resultsSection.Style["display"] = "none";
            }
        }

        private void GetStudentID()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT StudentID FROM StudentMaster WHERE UserMaster_UserID = @UserID AND SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            StudentID = reader["StudentID"]?.ToString();  // Store the StudentID in the class-level variable
                        }
                    }
                }
            }
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = ddlFilter.SelectedValue;
            if (selectedValue == "S")
            {
                // Implement logic for filtering students
            }
            else if (selectedValue == "E")
            {
                ResetPage();
                ddlExam.Visible = true;
                lblExam.Visible = true;
                btnView.Visible = true;
                GetExamList();
                btnView.Visible = true;
                btnGenerateReport.Visible = false;
            }
            else if (selectedValue == "F")
            {
                ResetPage();
                btnGenerateReport.Visible = true;
                btnView.Visible = false;
                ddlExam.Visible = false;
                lblExam.Visible = false;
            }
        }

        private void GetFullReportCard()
        {
            GenerateReportCardPdf();
        }

        private void GenerateReportCardPdf()
        {
            // SQL query to get the data including subject, exam name, marks, and total marks
            string query = @"
    SELECT 
        SM.SubjectName AS SubjectName,
        E.ExamName AS ExamName,
        G.Grade AS MarksObtained,
        E.TotalMarks AS TotalMarks
    FROM 
        Grades G
    JOIN 
        Exam E ON G.ExamMaster_ExamID = E.ExamID
    JOIN 
        SubjectMaster SM ON G.SubjectID = SM.SubjectID
    WHERE 
        G.StudentID = @StudentID
        AND G.SchoolMaster_SchoolID = @SchoolID
    ORDER BY 
        SM.SubjectName, E.ExamName";

            // Create a DataTable to hold the fetched data
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            if (dt.Rows.Count > 0)
            {
                // Get School Information (Name and Address)
                (string schoolName, string schoolAddress) = GetSchoolInfo(schoolId);

                // Fetch Student Information (Name, Standard, Division)
                string studentName = "";
                string studentStandard = "";
                string studentDivision = "";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string studentQuery = @"
            SELECT 
            Student_FullName AS StudentName,
            SD.StandardName AS Standard,
            DV.DivisionName AS Division
            FROM 
            StudentMaster SM
            LEFT JOIN Standards SD ON SD.StandardID = SM.Student_Standard
            LEFT JOIN Divisions DV ON DV.DivisionID = SM.Student_Division
            WHERE 
            SM.StudentID = @StudentID
            AND SM.SchoolMaster_SchoolID = @SchoolID";

                    using (SqlCommand cmd = new SqlCommand(studentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", StudentID);
                        cmd.Parameters.AddWithValue("@SchoolID", schoolId);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                studentName = reader["StudentName"].ToString();
                                studentStandard = reader["Standard"].ToString();
                                studentDivision = reader["Division"].ToString();
                            }
                        }
                    }
                }

                // Create a Dictionary to group data by Subject
                var subjectData = new Dictionary<string, Dictionary<string, (string MarksObtained, string TotalMarks)>>();

                // Collect all distinct exam names
                HashSet<string> examNames = new HashSet<string>();

                foreach (DataRow row in dt.Rows)
                {
                    string subjectName = row["SubjectName"].ToString();
                    string examName = row["ExamName"].ToString();
                    string marksObtained = row["MarksObtained"].ToString();
                    string totalMarks = row["TotalMarks"].ToString();

                    // Add exam name to the hash set (avoids duplicates)
                    examNames.Add(examName);

                    // Group by Subject and then by Exam
                    if (!subjectData.ContainsKey(subjectName))
                    {
                        subjectData[subjectName] = new Dictionary<string, (string MarksObtained, string TotalMarks)>();
                    }
                    subjectData[subjectName][examName] = (marksObtained, totalMarks);
                }

                // Create a PDF document
                using (MemoryStream ms = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate());
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // School Information Header
                    Paragraph schoolNameParagraph = new Paragraph(schoolName, FontFactory.GetFont("Arial", 18, Font.BOLD));
                    schoolNameParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(schoolNameParagraph);

                    Paragraph schoolAddressParagraph = new Paragraph(schoolAddress + "\n\n", FontFactory.GetFont("Arial", 12));
                    schoolAddressParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(schoolAddressParagraph);

                    // Student Information (Student Name, Standard, Division, and Date)
                    Paragraph studentInfoParagraph = new Paragraph($"\nStudent Name: {studentName}\nStandard: {studentStandard}\nDivision: {studentDivision}\nDate: {DateTime.Now.ToString("dd-MMM-yyyy")}", FontFactory.GetFont("Arial", 12));
                    studentInfoParagraph.Alignment = Element.ALIGN_LEFT;
                    document.Add(studentInfoParagraph);

                    // Create a table to display all subjects and exam data
                    int numColumns = 1 + (examNames.Count * 2); // Subject column + Marks Obtained & Total Marks for each exam
                    PdfPTable table = new PdfPTable(numColumns);
                    table.WidthPercentage = 100;

                    // Add header row to the table
                    table.AddCell("Subject Name");

                    // Add dynamic exam columns with two sub-headers: Marks Obtained and Total Marks
                    foreach (var exam in examNames)
                    {
                        table.AddCell(exam + " Marks Obtained");
                        table.AddCell(exam + " Total Marks");
                    }

                    // Variables to keep track of total marks for each exam
                    var examMarksObtained = new Dictionary<string, int>();  // Marks obtained for each exam
                    var examMarksTotal = new Dictionary<string, int>();     // Total marks for each exam

                    // Initialize dictionaries for exam totals
                    foreach (var exam in examNames)
                    {
                        examMarksObtained[exam] = 0;
                        examMarksTotal[exam] = 0;
                    }

                    // Add subject rows for each exam
                    foreach (var subject in subjectData)
                    {
                        string subjectName = subject.Key;

                        // Create one row for each subject
                        table.AddCell(subjectName);

                        // Add Marks Obtained and Total Marks for each exam
                        foreach (var exam in examNames)
                        {
                            string marksObtained = "";
                            string totalMarks = "";

                            // Look for the matching exam row
                            if (subject.Value.ContainsKey(exam))
                            {
                                var examData = subject.Value[exam];
                                marksObtained = examData.MarksObtained;
                                totalMarks = examData.TotalMarks;

                                // Accumulate marks for each exam
                                if (!string.IsNullOrEmpty(marksObtained))
                                {
                                    examMarksObtained[exam] += Convert.ToInt32(marksObtained);
                                }
                                if (!string.IsNullOrEmpty(totalMarks))
                                {
                                    examMarksTotal[exam] += Convert.ToInt32(totalMarks);
                                }
                            }

                            // Add marks to the table
                            table.AddCell(marksObtained);  // Marks Obtained
                            table.AddCell(totalMarks);     // Total Marks
                        }
                    }

                    // Add the table to the document
                    document.Add(table);

                    // Now add the breakdown of each exam horizontally

                    // Create a table for the breakdown (2 columns: Exam, Details)
                    PdfPTable breakdownTable = new PdfPTable(2); // 2 columns: Exam and Details
                    breakdownTable.WidthPercentage = 100;

                    // Loop over the exams and create a row for each exam
                    foreach (var exam in examMarksObtained)
                    {
                        string examName = exam.Key;
                        int marksObtained = exam.Value;
                        int totalMarks = examMarksTotal[exam.Key];
                        double percentage = (totalMarks == 0) ? 0 : ((double)marksObtained / totalMarks) * 100;

                        // Add a row with exam details horizontally
                        breakdownTable.AddCell(examName + ":");  // Exam Name
                        breakdownTable.AddCell(
                            $"Marks Obtained: {marksObtained} | " +
                            $"Total Marks: {totalMarks} | " +
                            $"Percentage: {percentage.ToString("0.00")}%"
                        );
                    }

                    // Add the breakdown table to the document
                    document.Add(breakdownTable);

                    // Calculate and display the overall totals and percentage
                    int grandTotalMarksObtained = examMarksObtained.Values.Sum();
                    int grandTotalMarksPossible = examMarksTotal.Values.Sum();
                    double grandPercentage = (grandTotalMarksPossible == 0) ? 0 : ((double)grandTotalMarksObtained / grandTotalMarksPossible) * 100;

                    // Add the final total and percentage
                    Paragraph totalParagraph = new Paragraph(
                        $"Total Marks Obtained: {grandTotalMarksObtained} / {grandTotalMarksPossible}\n" +
                        $"Percentage: {grandPercentage.ToString("0.00")}%\n",
                        FontFactory.GetFont("Arial", 12));
                    totalParagraph.Alignment = Element.ALIGN_LEFT;
                    document.Add(totalParagraph);

                    document.Close();

                    // Save the PDF file to a specific location or send it as a download
                    byte[] pdfBytes = ms.ToArray();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=StudentReportCard.pdf");
                    Response.OutputStream.Write(pdfBytes, 0, pdfBytes.Length);
                    Response.End();
                }
            }
            else
            {
                // Handle case when no data is found
                Response.Write("No data available for the student.");
            }
        }



        // Method to fetch school name and address from the database
        private (string, string) GetSchoolInfo(string schoolID)
        {
            string schoolName = "";
            string schoolAddress = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT SchoolName, 
                            COALESCE(SchoolAddress1, '') + ' ' + COALESCE(SchoolAddress2, '') AS SchoolAddress
                         FROM SchoolMaster
                         WHERE SchoolID = @SchoolID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            schoolName = reader["SchoolName"].ToString();
                            schoolAddress = reader["SchoolAddress"].ToString();
                        }
                    }
                }
            }

            return (schoolName, schoolAddress);
        }

        private void GetExamList()
        {
            string query = "SELECT ExamID, ExamName FROM Exam WHERE SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlExam.DataSource = reader;
                    ddlExam.DataTextField = "ExamName";
                    ddlExam.DataValueField = "ExamID";
                    ddlExam.DataBind();
                }
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            string selectedValue = ddlFilter.SelectedValue;
            if (selectedValue == "E")
            {
                GetReportCardExamWise();
            }
            else
            {
                GetFullReportCard();
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            GetFullReportCard();
        }
            
        private void GetReportCardExamWise()
        {
            string examID = ddlExam.SelectedValue;

            // First query to fetch student details (name, standard, division)
            string studentDetailsQuery = @"
    SELECT s.Student_FullName AS [Student Name], 
           SD.StandardName AS [Standard], 
           DV.DivisionName AS [Division]
    FROM StudentMaster s
    LEFT JOIN Grades g ON s.StudentID = g.StudentID
    LEFT JOIN Standards SD ON SD.StandardID = g.StandardID
    LEFT JOIN Divisions DV ON DV.DivisionID = g.DivisionID
    WHERE s.SchoolMaster_SchoolID = @SchoolID 
      AND g.StudentID = @StudentID 
      AND g.ExamMaster_ExamID = @ExamID";

            // Second query to fetch grades (subjects, marks obtained, total marks)
            string gradesQuery = @"
    SELECT SM.SubjectName AS [SubjectName], 
           g.Grade AS MarksObtained,
           E.TotalMarks AS TotalMarks
    FROM StudentMaster s
    LEFT JOIN Grades g ON s.StudentID = g.StudentID
    LEFT JOIN SubjectMaster SM ON SM.SubjectID = g.SubjectID
    LEFT JOIN Exam E ON E.ExamID = g.ExamMaster_ExamID
    WHERE s.SchoolMaster_SchoolID = @SchoolID 
      AND g.StudentID = @StudentID 
      AND g.ExamMaster_ExamID = @ExamID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // First execute the student details query
                using (SqlCommand cmd = new SqlCommand(studentDetailsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@ExamID", examID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set student details
                            lblStudentNameValue.Text = reader["Student Name"].ToString();
                            lblStandardValue.Text = reader["Standard"].ToString();  // Standard
                            lblDivisionValue.Text = reader["Division"].ToString();  // Division

                            // Show student details section
                            studentDetailsSection.Style["display"] = "block";
                        }
                        else
                        {
                            lblStudentNameValue.Text = "No data found for the selected student.";
                            gvGrades.DataSource = null;
                            gvGrades.DataBind();
                            return;
                        }
                    }
                }

                // Then execute the grades query and bind the data to the GridView
                using (SqlCommand cmd = new SqlCommand(gradesQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@ExamID", examID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Load the data into a DataTable
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            // Bind data to GridView
                            gvGrades.DataSource = dt;
                            gvGrades.DataBind();

                            // Show results section
                            resultsSection.Style["display"] = "block";

                            // Now, process the data for calculations (sums, percentages)
                            CalculateTotals(dt);
                        }
                        else
                        {
                            gvGrades.DataSource = null;
                            gvGrades.DataBind();
                        }
                    }
                }
            }
        }

        private void CalculateTotals(DataTable dt)
        {
            marksObtainedSum = 0;
            totalMarksSum = 0;

            // Loop through the rows of the DataTable to calculate totals
            foreach (DataRow row in dt.Rows)
            {
                int marksObtained = Convert.ToInt32(row["MarksObtained"]);
                int totalMarks = Convert.ToInt32(row["TotalMarks"]);

                marksObtainedSum += marksObtained;
                totalMarksSum += totalMarks;
            }

            // Calculate percentage
            double percentage = (totalMarksSum == 0) ? 0 : ((double)marksObtainedSum / totalMarksSum) * 100;

            // Update the labels for total marks, percentage, etc.
            lblMarksObtainedSum.Text = marksObtainedSum.ToString();
            lblTotalMarksSum.Text = totalMarksSum.ToString();
            lblPercentage.Text = percentage.ToString("0.00") + "%";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Student/Dashboard.aspx");
        }

        protected void gvGrades_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int marksObtained = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "MarksObtained"));
                int totalMarks = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "TotalMarks"));

                marksObtainedSum += marksObtained;
                totalMarksSum += totalMarks;

                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    Label lblTotalMarks = (Label)e.Row.FindControl("lblMarksObtainedSum");
                    lblTotalMarks.Text = marksObtainedSum.ToString();

                    Label lblTotalMarksValue = (Label)e.Row.FindControl("lblTotalMarksSum");
                    lblTotalMarksValue.Text = totalMarksSum.ToString();

                    double percentage = ((double)marksObtainedSum / totalMarksSum) * 100;
                    lblPercentage.Text = percentage.ToString("0.00") + "%";
                }
            }
        }

        private void ResetPage()
        {
            lblStudentNameValue.Text = "";
            lblStandardValue.Text = "";
            lblDivisionValue.Text = "";
            gvGrades.DataSource = null;
            gvGrades.DataBind();
            lblMarksObtainedSum.Text = "0";
            lblTotalMarksSum.Text = "0";
            lblPercentage.Text = "0%";
            studentDetailsSection.Style["display"] = "none";
            resultsSection.Style["display"] = "none";
        }
    }
}
