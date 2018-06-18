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

        [FunctionName("SmsHttp")]
        public static async Task<HttpResponseMessage> SmsHttpAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sms")]HttpRequestMessage req,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "Spitball")] IAsyncCollector<SMSMessage> options,
            CancellationToken token
            )
        {
            var jsonContent = await req.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = JsonConvert.DeserializeObject<SmsMessage>(jsonContent);
            await options.AddAsync(new SMSMessage
            {
                To = message.PhoneNumber,
                Body = message.Message
            }, token).ConfigureAwait(false);
            return req.CreateResponse(HttpStatusCode.OK);
        }


        [FunctionName("FunctionSms")]
        public static async Task SmsServiceBusAsync(
            [ServiceBusTrigger(TopicSubscription.Communication, nameof(TopicSubscription.Sms))]SmsMessage message,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "Spitball")] IAsyncCollector<SMSMessage> options,
            TraceWriter log,
            CancellationToken token
        )
        {
            if (message.Message == null)
            {
                log.Error("message is null");
                return;
            }

            if (message.PhoneNumber == null)
            {
                log.Error("no phone number");
                return;
            }
            await options.AddAsync(new SMSMessage
            {
                To = message.PhoneNumber,
                Body = message.Message
            }, token).ConfigureAwait(false);
        }
    }
}
