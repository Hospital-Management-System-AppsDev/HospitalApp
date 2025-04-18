using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email{
    public class EmailService{
        public static async Task SendEmail() // Use async
        {
            await SendEmailAsync();
        }

        static async Task SendEmailAsync()
        {
            try
            {
                string rx = @"./Records/Prescriptions/RX_56_20250418_10-38-26_11_Michael Smith.pdf";
                string mc = @"./Records/MedicalCertificates/MC_56_20250418_11_Michael Smith.pdf";

                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    mail.From = new MailAddress("cetamedicalarts@gmail.com");
                    mail.To.Add("jcatillo1121@gmail.com");
                    mail.Subject = "Test";
                    mail.Body = "Test Email.";
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
