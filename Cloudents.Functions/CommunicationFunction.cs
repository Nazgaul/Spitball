using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Twilio;

namespace Cloudents.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CommunicationFunction
    {
        [FunctionName("FunctionEmail")]
        public static async Task EmailFunctionAsync(
            [ServiceBusTrigger(TopicSubscription.Communication, nameof(TopicSubscription.Email))]BrokeredMessage brokeredMessage,
            [SendGrid(ApiKey = "SendgridKey", From = "no-reply@spitball.co")] IAsyncCollector<Mail> emailProvider,
            IBinder binder,
            TraceWriter log,
            CancellationToken token)
        {
            var topicMessage = brokeredMessage.GetBodyInheritance<BaseEmail>();
            if (topicMessage == null)
            {
                log.Error("error with parsing message");
                return;
            }
            var dynamicBlobAttribute = new BlobAttribute($"mailcontainer/Spitball/{topicMessage.Template}-mail.html");

            var htmlTemplate = await binder.BindAsync<string>(dynamicBlobAttribute, token).ConfigureAwait(false);
            if (htmlTemplate == null)
            {
                log.Error("error with template name" + topicMessage.Template);
                return;
            }
            var content = htmlTemplate.Inject(topicMessage);
            var message = new Mail
            {
                Subject = topicMessage.Subject,
            };
            message.AddContent(new Content("text/html", content));
            var personalization = new Personalization();
            personalization.AddTo(new Email(topicMessage.To));
            message.AddPersonalization(personalization);
            await emailProvider.AddAsync(message, token).ConfigureAwait(false);
        }

        //in the us there is no alpha numeric phone https://support.twilio.com/hc/en-us/articles/223133767-International-support-for-Alphanumeric-Sender-ID?_ga=2.130088527.199542025.1529834887-1745228096.1524564655
        [FunctionName("SmsHttp")]
        public static async Task<HttpResponseMessage> SmsHttpAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sms")]HttpRequestMessage req,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<SMSMessage> options,
            TraceWriter log,
            CancellationToken token
            )
        {
            var jsonContent = await req.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = JsonConvert.DeserializeObject<SmsMessage>(jsonContent);
            if (message.Message == null)
            {
                log.Error("no body " + jsonContent);
                req.CreateResponse(HttpStatusCode.BadRequest);
            }
            await options.AddAsync(new SMSMessage
            {
                To = message.PhoneNumber,
                Body = "Your code to enter into Spitball is: " + message.Message
            }, token).ConfigureAwait(false);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
