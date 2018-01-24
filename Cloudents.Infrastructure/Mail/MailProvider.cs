using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Cloudents.Infrastructure.Mail
{
    //public class MailProvider : IMailProvider
    //{
    //    private const string ApiKey = "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";

    //    public Task GenerateSystemEmailAsync(string subject, string text, CancellationToken token)
    //    {
    //        return SendEmailAsync(subject, text, token);
    //    }

    //    private static Task SendEmailAsync(string subject, string text, CancellationToken token)
    //    {
    //        var client = new SendGridClient(ApiKey);
    //        var msg = new SendGridMessage
    //        {
    //            From = new EmailAddress("no-reply@spitball.co", "spitball system"),
    //            Subject = subject,
    //            PlainTextContent = text,
    //            //HtmlContent = "<strong>and easy to do anywhere, even with C#</strong>"
    //        };
    //        msg.AddTo(new EmailAddress("ram@cloudents.com", "Ram Y"));
    //        return client.SendEmailAsync(msg, token);
    //    }
    //}
}
