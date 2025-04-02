using System.Net;
using System.Net.Mail;
using cosmeticClinic.Settings;
using Microsoft.Extensions.Configuration;

namespace cosmeticClinic.Business;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IConfiguration configuration)
    {
        _emailSettings = configuration.GetSection("EmailService").Get<EmailSettings>();
    }

    public async Task SendAccountCreatedEmail(string userId, string recipientEmail)
    {
        try
        {
            string subject = "Your Yara Choice Clinic Account is Ready!";
            string emailBody = $@"
                        <html>
                        <body style='color: black; display: flex; justify-content: center; align-items: center; height: 100vh; flex-direction: column;'>
                            <div style='text-align: center;'>
                                <h1>Welcome to Yara Choice Clinic! 🎉</h1>
                                <p>Your account has been successfully created.</p>
                                <p>For security reasons, please change your password immediately by clicking the link below:</p>
                                <a href='http://localhost:5173/{userId}' 
                                   style='
                                      display: inline-block;
                                      padding: 10px 20px;
                                      font-size: 16px;
                                      color: white;
                                      background-color: #FBD909;
                                      text-decoration: none;
                                      border-radius: 5px;'>
                                    Change Password
                                </a>
                            </div>
                        </body>
                        </html>";

            using (var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
                smtpClient.EnableSsl = true;

                using (var mail = new MailMessage
                       {
                           From = new MailAddress(_emailSettings.Email),
                           Subject = subject,
                           Body = emailBody,
                           IsBodyHtml = true
                       })
                {
                    mail.To.Add(recipientEmail);
                    Console.WriteLine("📤 Sending email...");
                    await smtpClient.SendMailAsync(mail);
                    Console.WriteLine("✅ Email sent successfully!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to send email: {ex.Message}");
        }
    }
}