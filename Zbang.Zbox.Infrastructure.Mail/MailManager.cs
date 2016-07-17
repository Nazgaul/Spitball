using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SendGrid;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {
        //private const string SendGridUserName = "cloudents";
        //private const string SendGridPassword = "zbangitnow";
        //
        //private const string SendGridUserNameUs = "SpitballUS";
        //private const string SendGridPasswordUs = "9cloudents";


        private readonly IocFactory m_Container = IocFactory.IocWrapper;

        private static Task SendAsync(ISendGrid message, ICredentials credentials)
        {
            var transport = new Web(new NetworkCredential(credentials.UserName, credentials.Password));
            return transport.DeliverAsync(message);
        }


        public async Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

                var sendGridMail = new SendGridMessage
                {
                    From = new MailAddress(parameters.SenderEmail, parameters.SenderName)
                };

                sendGridMail.AddTo(ConfigFetcher.IsEmulated ? "ram@cloudents.com" : recipient);

                var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
                mail.AddSubject(sendGridMail);
                mail.GenerateMail(sendGridMail, parameters);


                sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
                sendGridMail.AddSubstitution("{email}", new List<string> { recipient });

                sendGridMail.EnableClickTracking();
                sendGridMail.EnableOpenTracking();
                await SendAsync(sendGridMail, new Credentials());
            }
            catch (FormatException ex)
            {
                TraceLog.WriteError("recipient: " + recipient + " on trying to send mail", ex);
                throw;
            }

        }

        public Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/unsubscribes", startTime, page, cancellationToken);
        }

        public Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/invalid_emails", startTime, page, cancellationToken);

        }
        const string ApiKey = "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";
        private async Task<IEnumerable<string>> GetEmailListFromApiCallAsync(string requestUrl, DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {


            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (long)(startTime.ToUniversalTime() - epoch).TotalSeconds;

            var client = new Client(ApiKey);
            var result = await client.Get($"{requestUrl}?limit={500}&offset={500 * page}&start_time={unixDateTime}");
            var data = await result.Content.ReadAsStringAsync();
            var emailArray = JArray.Parse(data);
            return emailArray.Select(s => s["email"].ToString());
        }

        public async Task DeleteUnsubscribeAsync(string email)
        {

            try
            {
                var client = new Client(ApiKey);
                await client.GlobalSuppressions.Delete(email);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on delete unsubscribe", ex);
            }
        }

        public async Task GenerateSystemEmailAsync(string subject, string text)
        {
            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress("no-reply@spitball.co", "spitball system"),
                Text = text,
                Subject = subject
            };
            sendGridMail.AddTo("ram@cloudents.com");
            await SendAsync(sendGridMail, new UsCredentials());
        }

        
    }
}
