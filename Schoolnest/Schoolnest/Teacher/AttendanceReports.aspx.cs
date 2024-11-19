using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO.Compression;

namespace Schoolnest.Teacher
{
    public partial class AttendanceReports : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        string schoolId = string.Empty;
        string Username = string.Empty;
        int standard, division;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                schoolId = Session["SchoolId"].ToString();
                Username = Session["Username"].ToString();
                LoadTeacherInfo(schoolId, Username);
            }
        }

        private void LoadTeacherInfo(string schoolId, string Username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TM.TeacherName AS TeacherName,
                                SD.StandardName AS [Standard],
                                DV.DivisionName AS [Division],
                                AT.StandardID,
                                AT.DivisionID
                         FROM AssignTeacher AT 
                         LEFT JOIN TeacherMaster TM ON TM.TeacherID = AT.TeacherID
                         LEFT JOIN Standards SD ON SD.StandardID = AT.StandardID
                         LEFT JOIN Divisions DV ON DV.DivisionID = AT.DivisionID
                         WHERE AT.TeacherID = @TeacherID AND AT.SchoolID = @SchoolID";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@TeacherID", Username);
                cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        lblTeacher.Text = reader["TeacherName"].ToString();
                        lblStd.Text = reader["Standard"].ToString();
                        lblDivision.Text = reader["Division"].ToString();
                       
                    }
                }
                reader.Close();
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.ParseExact(txtFromDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(txtToDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string schoolID = Session["SchoolID"].ToString();
            (string schoolName, string schoolAddress) = GetSchoolInfo(schoolID);
            string teacherName = lblTeacher.Text;
            string standard = lblStd.Text;
            string division = lblDivision.Text;

            using (MemoryStream zipStream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        DataTable dt = GetAttendanceData(date.ToString("yyyy-MM-dd"));
                        if (dt.Rows.Count > 0)
                        {
                            string reportDate = date.ToString("dd-MMM-yyyy");
                            using (MemoryStream pdfStream = GeneratePdf(dt, reportDate, schoolName, schoolAddress, teacherName, standard, division))
                            {
                                ZipArchiveEntry zipEntry = zip.CreateEntry($"AttendanceReport_{reportDate}.pdf", CompressionLevel.Fastest);
                                using (Stream entryStream = zipEntry.Open())
                                {
                                    pdfStream.Seek(0, SeekOrigin.Begin);
                                    pdfStream.CopyTo(entryStream);
                                }
                            }
                        }
                    }
                }

                // Serve the ZIP file to the user
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "attachment; filename=AttendanceReports.zip");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                zipStream.Seek(0, SeekOrigin.Begin);
                zipStream.CopyTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

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

        private DataTable GetAttendanceData(string date)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT ROW_NUMBER() OVER(ORDER BY StudentName) AS SrNo,
                                    StudentName,
                                    Status
                             FROM StudentAttendance
                             WHERE Date = @Date";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", date);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        private MemoryStream GeneratePdf(DataTable dt, string reportDate, string schoolName, string schoolAddress, string teacherName, string standard, string division)
        {
            MemoryStream memoryStream = new MemoryStream();

            Document pdfDoc = new Document(PageSize.A4, 36, 36, 72, 72);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
            writer.CloseStream = false; // Prevent PdfWriter from closing the MemoryStream
            pdfDoc.Open();

            // School Information Header
            Paragraph schoolNameParagraph = new Paragraph(schoolName, FontFactory.GetFont("Arial", 18, Font.BOLD));
            schoolNameParagraph.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(schoolNameParagraph);

            Paragraph schoolAddressParagraph = new Paragraph(schoolAddress + "\n\n", FontFactory.GetFont("Arial", 12));
            schoolAddressParagraph.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(schoolAddressParagraph);

            // Teacher and Class Information
            PdfPTable infoTable = new PdfPTable(2);
            infoTable.WidthPercentage = 100;
            infoTable.SetWidths(new float[] { 1, 2 });
            infoTable.AddCell(new PdfPCell(new Phrase("Teacher Name:", FontFactory.GetFont("Arial", 12, Font.BOLD))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase(teacherName, FontFactory.GetFont("Arial", 12))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase("Date:", FontFactory.GetFont("Arial", 12, Font.BOLD))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase(reportDate, FontFactory.GetFont("Arial", 12))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase("Standard:", FontFactory.GetFont("Arial", 12, Font.BOLD))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase(standard, FontFactory.GetFont("Arial", 12))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase("Division:", FontFactory.GetFont("Arial", 12, Font.BOLD))) { Border = 0 });
            infoTable.AddCell(new PdfPCell(new Phrase(division, FontFactory.GetFont("Arial", 12))) { Border = 0 });
            pdfDoc.Add(infoTable);

            // Title for Attendance Sheet
            Paragraph title = new Paragraph("\nAttendance Sheet\n\n", FontFactory.GetFont("Arial", 16, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(title);

            // Attendance Table with Column Headers
            PdfPTable pdfTable = new PdfPTable(3);
            pdfTable.WidthPercentage = 100;
            pdfTable.SetWidths(new float[] { 1, 3, 2 });

            // Column Headers
            pdfTable.AddCell(new PdfPCell(new Phrase("Sr No", FontFactory.GetFont("Arial", 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
            pdfTable.AddCell(new PdfPCell(new Phrase("Student Name", FontFactory.GetFont("Arial", 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
            pdfTable.AddCell(new PdfPCell(new Phrase("Status", FontFactory.GetFont("Arial", 12, Font.BOLD))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });

            // Add Table Rows
            foreach (DataRow row in dt.Rows)
            {
                pdfTable.AddCell(new PdfPCell(new Phrase(row["SrNo"].ToString(), FontFactory.GetFont("Arial", 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                pdfTable.AddCell(new PdfPCell(new Phrase(row["StudentName"].ToString(), FontFactory.GetFont("Arial", 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                pdfTable.AddCell(new PdfPCell(new Phrase(row["Status"].ToString(), FontFactory.GetFont("Arial", 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }

            pdfDoc.Add(pdfTable);
            pdfDoc.Close();
            writer.Close();

            memoryStream.Position = 0; // Reset the stream position to the beginning for reading
            return memoryStream;
        }




    }
}