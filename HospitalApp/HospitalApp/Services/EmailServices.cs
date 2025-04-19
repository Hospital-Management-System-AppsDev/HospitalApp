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
                    mail.To.Add("jcatillo1121@gmail.com");
                    mail.Subject = "Medical Certificate and Prescription for Your Recent Visit";
                    mail.Body = @$"<p>Dear {app.PatientName},</p>
                                <p>We hope this email finds you in good health.</p>

                                <p>Please find attached the medical certificate and prescription as per your recent <strong>{app.AppointmentType}</strong> with <strong>Dr. {app.AssignedDoctor.Name}</strong> at <strong>CETA Medical Arts</strong> on <strong>{app.AppointmentDateTime}</strong>. Kindly review the documents for your records and follow the prescribed instructions as indicated.</p>

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
    }
}
