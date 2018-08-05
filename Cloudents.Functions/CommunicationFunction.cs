using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Blob;
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
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<Mail> emailProvider,
            IBinder binder,
            TraceWriter log,
            CancellationToken token)
        {
            if (brokeredMessage.DeliveryCount > 5)
            {
                log.Warning("invoking message from queue");
                return;
            }
            var topicMessage = brokeredMessage.GetBodyInheritance<BaseEmail>();
            
            if (topicMessage == null)
            {
                log.Error("error with parsing message");
                return;
            }

            await ProcessEmail(emailProvider, binder, log, topicMessage, token);
        }

        //[FunctionName("FunctionEmailTest")]
        //public static async Task EmailFunctionTimerAsync(
        //    [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
        //    [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")]
        //    IAsyncCollector<Mail> emailProvider,
        //    IBinder binder,
        //    TraceWriter log,
        //    CancellationToken token)
        //{
        //    var topicMessage = new AnswerCorrectEmail("hadar@cloudents.com", "text", "xxx",
        //        "https://www.spitball.co", 456.23424M);
        //    await ProcessEmail(emailProvider, binder, log, topicMessage, token);
        //}

        private static async Task ProcessEmail(IAsyncCollector<Mail> emailProvider, IBinder binder, TraceWriter log,
            BaseEmail topicMessage, CancellationToken token)
        {
            var message = new Mail();

            void TextEmail()
            {
                message.AddContent(new Content("text/plain", topicMessage.ToString()));
                message.Subject = topicMessage.Subject;
                log.Warning("error with template name" + topicMessage.Template);
            }

            if (topicMessage.Template != null)
            {
                var dynamicBlobAttribute =
                    new BlobAttribute($"mailcontainer/Spitball/{topicMessage.Template}-mail.html");

                var blob = await binder.BindAsync<CloudBlockBlob>(dynamicBlobAttribute, token).ConfigureAwait(false);
                if (await blob.ExistsAsync(token).ConfigureAwait(false))
                {
                    var htmlTemplate = await blob.DownloadTextAsync(token).ConfigureAwait(false);

                    if (!blob.Metadata.TryGetValue("subject", out var subject))
                    {
                        subject = topicMessage.Subject;
                    }

                    message.Subject = subject;
                    message.AddCategory(message.Subject);
                    if (htmlTemplate != null)
                    {
                        var content = htmlTemplate.Inject(topicMessage);
                        message.AddContent(new Content("text/html", content));
                    }
                    else
                    {
                        TextEmail();
                    }
                }
                else
                {
                    TextEmail();
                }
            }
            else
            {
                TextEmail();
            }

            var personalization = new Personalization();
            personalization.AddTo(new Email(topicMessage.To));
            message.AddPersonalization(personalization);
            
            await emailProvider.AddAsync(message, token).ConfigureAwait(false);
        }

        //in the us there is no alpha numeric phone https://support.twilio.com/hc/en-us/articles/223133767-International-support-for-Alphanumeric-Sender-ID?_ga=2.130088527.199542025.1529834887-1745228096.1524564655
        //[FunctionName("SmsHttp")]
        //public static async Task<HttpResponseMessage> SmsHttpAsync(
        //    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sms")]HttpRequestMessage req,
        //    [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<SMSMessage> options,
        //    TraceWriter log,
        //    CancellationToken token
        //    )
        //{
        //    var jsonContent = await req.Content.ReadAsStringAsync().ConfigureAwait(false);
        //    var message = JsonConvert.DeserializeObject<SmsMessage>(jsonContent);
        //    if (message.Message == null)
        //    {
        //        log.Error("no body " + jsonContent);
        //        req.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //    await options.AddAsync(new SMSMessage
        //    {
        //        To = message.PhoneNumber,
        //        Body = "Your code to enter into Spitball is: " + message.Message
        //    }, token).ConfigureAwait(false);
        //    return req.CreateResponse(HttpStatusCode.OK);
        //}


        [FunctionName("FunctionSms")]
        public static async Task SmsServiceBusAsync(
            [ServiceBusTrigger(TopicSubscription.Communication, nameof(TopicSubscription.Sms))]SmsMessage2 message,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<SMSMessage> options,
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
                Body = "Your code to enter into Spitball is: " + message.Message
            }, token).ConfigureAwait(false);
        }
    }
}
