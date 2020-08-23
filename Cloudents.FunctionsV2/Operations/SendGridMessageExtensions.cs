using System.Globalization;
using System.Net.Mail;
using SendGrid.Helpers.Mail;

namespace Cloudents.FunctionsV2.Operations
{
    public static class SendGridMessageExtensions
    {
        public static void AddFromResource(this SendGridMessage message, CultureInfo culture)
        {

            CultureInfo.DefaultThreadCurrentCulture = culture;
            var mailAddress = new MailAddress(ResourceWrapper.GetString("email_from"));
            message.From = new EmailAddress(mailAddress.Address, mailAddress.DisplayName);
        }
    }
}