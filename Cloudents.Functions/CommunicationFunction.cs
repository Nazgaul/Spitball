using System;
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
            [ServiceBusTrigger(TopicSubscription.Communication, nameof(TopicSubscription.Email), AccessRights.Listen)]BrokeredMessage brokeredMessage,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<Mail> emailProvider,
            IBinder binder,
            TraceWriter log,
            CancellationToken token)
        {
            if (brokeredMessage.DeliveryCount > 1)
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

            log.Info("finish sending email");
        }

        [FunctionName("FunctionEmailTest")]
        public static async Task EmailFunctionTimerAsync(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply @spitball.co>")]
            IAsyncCollector<Mail> emailProvider,
            IBinder binder,
            TraceWriter log,
            CancellationToken token)
        {
            var topicMessage = new AnswerCorrectEmail("hadar@cloudents.com", "text", "xxx",
             "https://www.spitball.co", 456.23424M);
            await ProcessEmail(emailProvider, binder, log, topicMessage, token);
        }

        private static async Task ProcessEmail(IAsyncCollector<Mail> emailProvider, IBinder binder, TraceWriter log,
            BaseEmail topicMessage, CancellationToken token)
        {
            var message = new Mail();
            message.TrackingSettings = new TrackingSettings();
            message.TrackingSettings.Ganalytics = new Ganalytics();
            message.TrackingSettings.Ganalytics.Enable = true;
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
                    message.AddCategory(topicMessage.Campaign);

                    message.TrackingSettings.Ganalytics.UtmCampaign = topicMessage.Campaign;
                    message.TrackingSettings.Ganalytics.UtmSource = topicMessage.Source;
                    message.TrackingSettings.Ganalytics.UtmMedium = topicMessage.Medium;
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



        //From = "Spitball"
        [FunctionName("FunctionSms")]
        public static async Task SmsServiceBusAsync(
            [ServiceBusTrigger(TopicSubscription.Communication, nameof(TopicSubscription.Sms), AccessRights.Listen)]BrokeredMessage message,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<SMSMessage> options,
            TraceWriter log,
            CancellationToken token
        )
        {
            if (message.DeliveryCount > 2)
            {
                log.Warning("invoking message from queue");
                return;
            }

            if (message.EnqueuedTimeUtc < DateTime.Now.AddDays(-1))
            {
                log.Warning("Too late of a message");
                return;
            }

            if (message.Label == "Email")
            {
                log.Warning("Getting message in topic sms of email - need to check it out");
                //var topicMessage = message.GetBodyInheritance<BaseEmail>();
                //var msMessage = new BrokeredMessage(topicMessage)
                //{
                //    Label = nameof(TopicSubscription.Email)
                //};
                //msMessage.Properties["messageType"] = topicMessage.GetType().AssemblyQualifiedName;
                //await meg.AddAsync(msMessage);
                return;
            }
            var msg = message.GetBody<SmsMessage2>();


            if (msg.Message == null)
            {
                log.Error("message is null");
                return;
            }

            if (msg.PhoneNumber == null)
            {
                log.Error("no phone number");
                return;
            }
            await options.AddAsync(new SMSMessage
            {
                To = msg.PhoneNumber,
                Body = "Your code to enter into Spitball is: " + msg.Message
            }, token).ConfigureAwait(false);
        }
    }
}
