using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Web;

namespace Schoolnest.Student
{
    public partial class StudentFeeInvoice : System.Web.UI.Page
    {
        string connectionString = Global.ConnectionString;
        private string SchoolID;
        private string StudentID;
        private string AcademicYear;
        private int UserID;
        dynamic SchoolDetails = null;
        dynamic StudentDetails = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            SchoolID = Session["SchoolID"]?.ToString();
            UserID = Convert.ToInt32(Session["UserID"]);
            GetAcademicYear();
            StudentID = GetStudentIDFromSession(UserID, SchoolID);

            string feeRecordID = Request.QueryString["FeeRecordID"];
            if (!string.IsNullOrEmpty(feeRecordID))
            {
                GenerateInvoicePDF(feeRecordID);
            }
        }

        private void GetAcademicYear()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT AcademicYear FROM SchoolSettings WHERE SchoolMaster_SchoolID = @SchoolID", conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AcademicYear = reader["AcademicYear"]?.ToString();
                        }
                    }
                }
            }
        }

        private string GetStudentIDFromSession(int userID, string schoolID)
        {
            string studentId = string.Empty;

            string query = @"
                SELECT StudentID 
                FROM StudentMaster 
                WHERE UserMaster_UserID = @UserID 
                AND SchoolMaster_SchoolID = @SchoolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@SchoolID", schoolID);

                conn.Open();
                studentId = cmd.ExecuteScalar()?.ToString();
            }

            return studentId;
        }

        private dynamic GetSchoolDetails()
        {
            dynamic schoolDetails = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"Select SM.*, LM.State, LM.City, LM.Pincode from SchoolMaster as SM 
                LEFT JOIN LocationMaster as LM ON LM.LocationID = SM.School_LocationID WHERE SM.SchoolID = @SchoolID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            schoolDetails = new
                            {
                                SchoolName = reader["SchoolName"].ToString(),
                                SchoolAddress1 = reader["SchoolAddress1"].ToString(),
                                SchoolAddress2 = reader["SchoolAddress2"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = reader["Pincode"].ToString(),
                                SchoolPhone = reader["SchoolPhone"].ToString(),
                                SchoolAlternatePhone = reader["SchoolAlternatePhone"].ToString(),
                                SchoolEmail = reader["SchoolEmail"].ToString(),
                                SchoolLogo = reader["SchoolLogo"].ToString(),
                            };
                        }
                    }
                }

            }
            
            return schoolDetails;
        }

        private dynamic GetStudentDetails()
        {
            dynamic studentDetails = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"Select SM.*, S.StandardName, D.DivisionName, LM.State, LM.City, LM.Pincode from StudentMaster as SM 
                    LEFT JOIN Standards as S ON S.StandardID = SM.Student_Standard
                    LEFT JOIN Divisions as D ON D.DivisionID = SM.Student_Division
                    LEFT JOIN LocationMaster as LM ON LM.LocationID = SM.Student_LocationID 
                    WHERE SM.StudentID = @StudentID AND SM.SchoolMaster_SchoolID = @SchoolID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);
                    cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            studentDetails = new
                            {
                                Student_FullName = reader["Student_FullName"].ToString(),
                                Student_Address1 = reader["Student_Address1"].ToString(),
                                Student_Address2 = reader["Student_Address2"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = reader["Pincode"].ToString(),
                                Student_GRNumber = reader["Student_GRNumber"].ToString(),
                                Student_EmailID = reader["Student_EmailID"].ToString(),
                                Student_MobileNumber = reader["Student_MobileNumber"].ToString(),
                                StandardName = reader["StandardName"].ToString(),
                                DivisionName = reader["DivisionName"].ToString(),
                            };
                        }
                    }
                }

            }
            
            return studentDetails;
        }

        private dynamic GetInvoiceDetails(string feeRecordID)
        {
            dynamic invoiceDetails = new ExpandoObject();
            var detailsDict = (IDictionary<string, object>)invoiceDetails;

            detailsDict["FeeNames"] = new List<string>();
            detailsDict["PayAmounts"] = new List<string>();
            detailsDict["LateFees"] = new List<string>();
            detailsDict["PaymentDates"] = new List<string>();

            detailsDict["ReceiptNumber"] = null;
            detailsDict["TermType"] = null;
            detailsDict["PaidAmount"] = null;
            detailsDict["PaymentDate"] = null;

            CultureInfo indiaCulture = new CultureInfo("en-IN");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT FR.*, FD.*, FM.FeeName 
                FROM FeeRecords AS FR
                LEFT JOIN FeeDetail AS FD ON FD.FeeDetailID = FR.FeeDetail_FeeDetailID
                LEFT JOIN FeeMaster AS FM ON FM.FeeMasterID = FD.FeeMasterID
                WHERE FR.FeeRecordID = @FeeRecordID AND FR.FeeDetail_FeeDetailID IS NOT NULL
";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FeeRecordID", feeRecordID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (detailsDict["ReceiptNumber"] == null)
                            {
                                detailsDict["ReceiptNumber"] = reader["ReceiptNumber"].ToString();
                                detailsDict["TermType"] = reader["TermType"].ToString();
                                detailsDict["PaidAmount"] = Convert.ToDecimal(reader["PaidAmount"]).ToString("C0", indiaCulture);
                                detailsDict["PaymentDate"] = Convert.ToDateTime(reader["PaymentDate"]).ToString("dd-MM-yyyy");
                            }

                            string feeName = reader["FeeName"]?.ToString();
                            if (string.IsNullOrEmpty(feeName))
                            {
                                //feeName = reader["LateFee"]?.ToString();

                                if (reader["LateFee"] != DBNull.Value && Convert.ToInt32(reader["LateFee"]) == 1)
                                {
                                    feeName = "Late Fee";
                                }
                            }

                            ((List<string>)detailsDict["FeeNames"]).Add(feeName);

                            decimal payAmount = Convert.ToDecimal(reader["PayAmount"]);
                            decimal lateFee = Convert.ToDecimal(reader["LateFee"]);

                            ((List<string>)detailsDict["PayAmounts"]).Add(payAmount.ToString("C0", indiaCulture));
                            ((List<string>)detailsDict["LateFees"]).Add(lateFee.ToString("C0", indiaCulture));

                            ((List<string>)detailsDict["PaymentDates"]).Add(Convert.ToDateTime(reader["PaymentDate"]).ToString("dd-MM-yyyy"));
                        }
                    }
                }
            }

            return invoiceDetails;
        }

        private void GenerateInvoicePDF(string feeRecordID)
        {
            var schoolDetails = GetSchoolDetails();
            var studentDetails = GetStudentDetails();
            var invoiceDetails = GetInvoiceDetails(feeRecordID);

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 20, 20, 20, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                writer.PageEvent = new CustomPdfPageEventHelper();

                document.Open();

                // Add content to PDF
                AddInvoiceContent(document, schoolDetails, studentDetails, invoiceDetails);

                document.Close();

                // Convert the document to bytes
                byte[] pdfBytes = ms.ToArray();

                // Write the PDF to the response stream
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=Invoice.pdf");
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
        }

        private void AddInvoiceContent(Document document, dynamic schoolDetails, dynamic studentDetails, dynamic invoiceDetails)
        {
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            PdfPTable headerTable = new PdfPTable(2);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 2f, 1.5f }); // Adjust the widths as needed

            // Add Logo (SchoolLogo) to the left
            if (!string.IsNullOrEmpty(schoolDetails.SchoolLogo))
            {
                // Build the path to the logo image
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string logoPath = Path.Combine(baseDirectory, "assets", "img", schoolDetails.SchoolLogo);

                if (File.Exists(logoPath))
                {
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(100f, 100f); // Adjust size of the logo as needed
                    PdfPCell logoCell = new PdfPCell(logo) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    PdfPCell logoPlaceholder = new PdfPCell(new Phrase("Logo", boldFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                    headerTable.AddCell(logoPlaceholder);
                }
            }
            else
            {
                // If logo is not provided, add a placeholder (optional)
                PdfPCell logoPlaceholder = new PdfPCell(new Phrase("Logo", boldFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                headerTable.AddCell(logoPlaceholder);
            }

            // Add School Address and Contact Info to the right
            PdfPCell addressCell = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_TOP };
            addressCell.Phrase = new Phrase($"{schoolDetails.SchoolName}\n{schoolDetails.SchoolAddress1}", normalFont);
            addressCell.Phrase.Add(new Phrase($"\n{schoolDetails.SchoolAddress2}\n", normalFont));
            addressCell.Phrase.Add(new Phrase($"{schoolDetails.City}, {schoolDetails.State} - {schoolDetails.Pincode}\n", normalFont));

            // Add Phone and Email if available
            if (!string.IsNullOrEmpty(schoolDetails.SchoolPhone))
            {
                addressCell.Phrase.Add(new Phrase($"Phone: {schoolDetails.SchoolPhone}\n", normalFont));
            }

            if (!string.IsNullOrEmpty(schoolDetails.SchoolAlternatePhone))
            {
                addressCell.Phrase.Add(new Phrase($"Alternate Phone: {schoolDetails.SchoolAlternatePhone}\n", normalFont));
            }

            if (!string.IsNullOrEmpty(schoolDetails.SchoolEmail))
            {
                addressCell.Phrase.Add(new Phrase($"Email: {schoolDetails.SchoolEmail}\n", normalFont));
            }

            headerTable.AddCell(addressCell);

            // Add the header table to the document
            document.Add(headerTable);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            // Add invoice title (centered)
            Paragraph invoiceTitle = new Paragraph("Student Fees Invoice", titleFont);
            invoiceTitle.Alignment = Element.ALIGN_CENTER;
            document.Add(invoiceTitle);

            document.Add(new Paragraph(" "));

            // Create a PdfPTable with 2 columns (left and right aligned)
            PdfPTable studentDetailsTable = new PdfPTable(2);
            studentDetailsTable.WidthPercentage = 100; // 100% width
            studentDetailsTable.SetWidths(new float[] { 4f, 2f }); // Equal column widths

            // Left-aligned student details
            PdfPCell leftCell = new PdfPCell();
            leftCell.Border = Rectangle.NO_BORDER;
            leftCell.AddElement(new Phrase($"{studentDetails.Student_FullName}", boldFont));
            // Add the Student Address (School Address)
            if (!string.IsNullOrEmpty(studentDetails.Student_Address1))
            {
                leftCell.AddElement(new Phrase($"{studentDetails.Student_Address1}"));
            }

            if (!string.IsNullOrEmpty(studentDetails.Student_Address2))
            {
                leftCell.AddElement(new Phrase($"{studentDetails.Student_Address2}"));
            }

            leftCell.AddElement(new Phrase($"{studentDetails.City},{studentDetails.State} - {studentDetails.Pincode}"));

            // Add Student Email and Mobile (Phone number)
            if (!string.IsNullOrEmpty(studentDetails.Student_EmailID))
            {
                leftCell.AddElement(new Phrase($"{studentDetails.Student_EmailID}"));
            }

            if (!string.IsNullOrEmpty(studentDetails.Student_MobileNumber))
            {
                leftCell.AddElement(new Phrase($"{studentDetails.Student_MobileNumber}"));
            }

            leftCell.AddElement(new Phrase($"Standard: {studentDetails.StandardName}"));
            leftCell.AddElement(new Phrase($"Division: {studentDetails.DivisionName}"));

            // Right-aligned student details
            PdfPCell rightCell = new PdfPCell();
            rightCell.Border = Rectangle.NO_BORDER;
            rightCell.HorizontalAlignment = Element.ALIGN_RIGHT; // Right-aligned

            rightCell.AddElement(new Phrase($"GR Number: {studentDetails.Student_GRNumber}", boldFont));
            rightCell.AddElement(new Phrase($"{invoiceDetails.TermType} Fee"));
            rightCell.AddElement(new Phrase($"Reciept: {invoiceDetails.ReceiptNumber}"));
            rightCell.AddElement(new Phrase($"Payment Date: {invoiceDetails.PaymentDate}"));
            rightCell.AddElement(new Phrase($"Amount Paid: {invoiceDetails.PaidAmount}", boldFont));

            // Add both cells to the table
            studentDetailsTable.AddCell(leftCell);
            studentDetailsTable.AddCell(rightCell);

            // Add the table to the document
            document.Add(studentDetailsTable);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            // Table for Fee Payment Details
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.5f, 3f }); // Adjust column widths to suit

            // Add header row (Description and Amount)

            // Description header - Left aligned
            PdfPCell descriptionHeader = new PdfPCell(new Phrase("Description", titleFont));
            descriptionHeader.HorizontalAlignment = Element.ALIGN_LEFT; // Left-align the Description
            descriptionHeader.Border = Rectangle.BOTTOM_BORDER; // Add a bottom border for line below header
            descriptionHeader.BorderWidth = 2f;
            table.AddCell(descriptionHeader);

            // Amount header - Right aligned
            PdfPCell amountHeader = new PdfPCell(new Phrase("Amount", titleFont));
            amountHeader.HorizontalAlignment = Element.ALIGN_RIGHT; // Right-align the Amount
            amountHeader.Border = Rectangle.BOTTOM_BORDER; // Add a bottom border for line below header
            amountHeader.BorderWidth = 2f;
            table.AddCell(amountHeader);

            // Add a line after the headers
            table.CompleteRow();

            // Add rows for FeeName and PayAmount (loop through your data)
            for (int i = 0; i < invoiceDetails.FeeNames.Count; i++)
            {
                AddFeeRow(table, invoiceDetails.FeeNames[i], invoiceDetails.PayAmounts[i], normalFont, normalFont);
            }

            // Add a line before the Total row
            PdfPCell emptyCell1 = new PdfPCell(new Phrase(""));
            emptyCell1.Border = Rectangle.BOTTOM_BORDER;
            emptyCell1.BorderWidth = 2f;
            emptyCell1.Colspan = 2;
            table.AddCell(emptyCell1);

            // Add the total row
            AddFeeRow(table, "Total", invoiceDetails.PaidAmount, boldFont, boldFont);

            // Add the table to the document
            document.Add(table);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            // Footer
            Paragraph footer = new Paragraph("*** This is a computer-generated invoice ***", normalFont);
            footer.Alignment = Element.ALIGN_CENTER;
            document.Add(footer);
        }
        private void AddTableRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.PaddingBottom = 5;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.PaddingBottom = 5;
            table.AddCell(valueCell);
        }

        private void AddFeeRow(PdfPTable table, string label, string value, Font normalFont, Font boldFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, boldFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value, normalFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(valueCell);
        }

        public class CustomPdfPageEventHelper : PdfPageEventHelper
        {
            private readonly Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // Override the OnEndPage method to add the footer content
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // Get the current date
                string currentDate = DateTime.Now.ToString("dd-MM-yyyy");

                // Get the current page number
                string pageNumber = $"Page {writer.PageNumber}";

                // Get the PdfContentByte instance to add content to the page
                PdfContentByte canvas = writer.DirectContent;

                // Add Today's Date to the left side of the footer
                Phrase datePhrase = new Phrase(currentDate, footerFont);
                float dateX = document.Left; // Left margin
                float dateY = document.Bottom - 0; // 20 units above the bottom margin
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, datePhrase, dateX, dateY, 0);

                // Add the Page Number to the right side of the footer
                Phrase pagePhrase = new Phrase(pageNumber, footerFont);
                float pageX = document.Right - 10; // Right margin, adjust to fit text
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, pagePhrase, pageX, dateY, 0);
            }
        }
    }
}
