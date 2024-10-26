using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace Schoolnest.Utilities
{
    public class EmailService
    {
        // Fixed SMTP server details for Gmail
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        // Fixed sender credentials
        private readonly string _senderEmail = "schoolnestcapstone@gmail.com";
        private readonly string _senderPassword = "nxij uksy myur bmny";

        // Simplified method to send email
        public bool SendEmail(string recipientEmail, string subject, string htmlBody, string attachmentPath = null)
        {
            try
            {
                // Create the MailMessage object
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                // Add the recipient
                mailMessage.To.Add(recipientEmail);

                // Check if there is an attachment
                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    Attachment attachment = new Attachment(attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                }

                // Configure the SMTP client and send the email
                using (SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.EnableSsl = true; // Enable SSL
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtpClient.Send(mailMessage);
                }

                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to send email: " + ex.Message);
                return false; // Email sending failed
            }
        }
    }
}