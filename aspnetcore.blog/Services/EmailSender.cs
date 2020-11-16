using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace aspnetcore.blog.Services
{
    public class EmailSender
    {
        public Task Execute(string subject, string message, string email)
        {
            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Constants.Blog.EmailAddress, $"{Constants.Blog.Name} - Contact"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
