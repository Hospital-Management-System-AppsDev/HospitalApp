using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HospitalApp.Models;

namespace Email
{
    public class EmailService
    {
        public static async Task SendEmail(string mcpath, string rxpath, Appointment app, Patient patient) 
        {
            await SendEmailAsync(mcpath, rxpath, app, patient);
        }

        public static async Task SendEmailAsync(string mcpath, string rxpath, Appointment app, Patient patient)
        {
            try
            {
                string rx = @$"{rxpath}";
                string mc = @$"{mcpath}";

                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    mail.From = new MailAddress("cetamedicalarts@gmail.com");
                    mail.To.Add($"{patient.Email}");
                    mail.Subject = "Medical Certificate and Prescription for Your Recent Visit";
                    mail.Body = @$"<p>Dear {app.PatientName},</p>
                                <p>We hope this email finds you in good health.</p>

                                <p>Please find attached the medical certificate and prescription as per your recent <strong>{app.AppointmentType}</strong> with <strong> {app.AssignedDoctor.Name}</strong> at <strong>CETA Medical Arts</strong> on <strong>{app.AppointmentDateTime}</strong>. Kindly review the documents for your records and follow the prescribed instructions as indicated.</p>

                                <p>If you have any questions or require further assistance, please do not hesitate to reach out to us.</p>

                                <p>Wishing you a speedy recovery.</p>

                                <p>Best regards,<br/>
                                <strong>CETA Medical Arts</strong></p>

                                <p><strong>Contact Information:</strong><br/>
                                Gmail: cetamedicalarts@gmail.com<br/>
                                Phone Number: +6395095748892<br/>
                                Telephone: (746) 636-4575</p>";

                    mail.IsBodyHtml = true;

                    mail.Attachments.Add(new Attachment(rx));
                    mail.Attachments.Add(new Attachment(mc));

                    smtp.Credentials = new NetworkCredential("cetamedicalarts@gmail.com", "nrtuobqzpyqukebd");
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail); // Asynchronous sending
                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public static async Task SendPharmacyReceiptEmail(string receiptPath, string email)
        {
            await SendPharmacyReceiptEmailAsync(receiptPath, email);
        }
        public static async Task<bool> SendPharmacyReceiptEmailAsync(string receiptPath, string email)
        {
            try
            {
                string receipt = @$"{receiptPath}";

                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    mail.From = new MailAddress("cetamedicalarts@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "CETA Medical Arts - Pharmacy Receipt";
                    mail.Body = @$"<p>Dear Valued Customer,</p>
                                <p>Thank you for your recent purchase at CETA Medical Arts Pharmacy.</p>

                                <p>Please find attached the receipt for your pharmacy purchase. This document serves as proof of purchase for your records.</p>

                                <p>If you have any questions regarding your purchase or need assistance with your medications, please don't hesitate to contact our pharmacy team.</p>

                                <p>We appreciate your business and trust in our services.</p>

                                <p>Best regards,<br/>
                                <strong>CETA Medical Arts Pharmacy</strong></p>

                                <p><strong>Contact Information:</strong><br/>
                                Gmail: cetamedicalarts@gmail.com<br/>
                                Phone Number: +6395095748892<br/>
                                Telephone: (746) 636-4575</p>";

                    mail.IsBodyHtml = true;

                    mail.Attachments.Add(new Attachment(receipt));

                    smtp.Credentials = new NetworkCredential("cetamedicalarts@gmail.com", "nrtuobqzpyqukebd");
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                    Console.WriteLine($"Pharmacy receipt email sent successfully to {email}!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending pharmacy receipt email: " + ex.Message);
                return false;
            }
        }

    }
}
