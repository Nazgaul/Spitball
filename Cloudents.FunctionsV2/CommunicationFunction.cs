using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Cloudents.FunctionsV2
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CommunicationFunction
    {
        [FunctionName("FunctionEmail")]
        public static async Task EmailFunctionAsync(
            [QueueTrigger(QueueName.EmailQueueName)] CloudQueueMessage cloudMessage,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
            ILogger log,
            CancellationToken token)
        {
            //if (cloudMessage.DequeueCount)
            //if (brokeredMessage.SystemProperties.DeliveryCount > 1)
            //{
            //    //log.Warning("invoking message from queue");
            //    //await brokeredMessage.DeadLetterAsync();
            //    return;
            //}

            //try
            //{
            //  var jsonMsg = Encoding.UTF8.GetString(cloudMessage.AsString);
            var topicMessage = JsonConvert.DeserializeObject<BaseEmail>(cloudMessage.AsString, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
            //var topicMessage = brokeredMessage.GetBodyInheritance<BaseEmail>();

            if (topicMessage == null)
            {
                log.LogError("error with parsing message");
                return;
            }

            await ProcessEmail(emailProvider, log, topicMessage, token);

            log.LogInformation("finish sending email");
            //}
            //catch (System.Runtime.Serialization.SerializationException ex)
            //{
            //    if (brokeredMessage.UserProperties.TryGetValue("messageType", out var messageType))
            //    {
            //        log.LogError($"Can't serialize {messageType}", ex);
            //    }
            //}
        }

        //[FunctionName("FunctionEmailTest")]
        //public static async Task EmailFunctionTimerAsync(
        //    [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
        //    [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply @spitball.co>")]
        //    IAsyncCollector<Mail> emailProvider,
        //    IBinder binder,
        //    TraceWriter log,
        //    CancellationToken token)
        //{
        //    var topicMessage = new AnswerCorrectEmail("hadar@cloudents.com", "text", "xxx",
        //     "https://www.spitball.co", 456.23424M);
        //    await ProcessEmail(emailProvider, binder, log, topicMessage, token);
        //}

        private static async Task ProcessEmail(IAsyncCollector<SendGridMessage> emailProvider, ILogger log,
            BaseEmail topicMessage, CancellationToken token)
        {
            var message = new SendGridMessage
            {
                TrackingSettings = new TrackingSettings { Ganalytics = new Ganalytics() }
            };
            message.AddTo(topicMessage.To);
            var personalization = new Personalization();
            //personalization.AddTo(new Email(topicMessage.To));
            message.Asm = new ASM
            {
                GroupId = 10926
            };

            if (topicMessage.TemplateId != null)
            {

                message.TemplateId = topicMessage.TemplateId;
                message.Subject = topicMessage.Subject;
                if (topicMessage.Campaign != null)
                {
                    message.AddCategory(topicMessage.Campaign);
                    message.TrackingSettings.Ganalytics.UtmCampaign = topicMessage.Campaign;
                    message.TrackingSettings.Ganalytics.UtmSource = "SendGrid";
                    message.TrackingSettings.Ganalytics.UtmMedium = "Email";
                    message.TrackingSettings.Ganalytics.Enable = true;
                }

                personalization.Substitutions = new Dictionary<string, string>();
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(topicMessage))
                {
                    var p = prop.GetValue(topicMessage);
                    personalization.Substitutions[$"-{prop.Name}-"] = p?.ToString() ?? string.Empty;
                    //personalization.AddSubstitution($"-{prop.Name}-", p?.ToString() ?? string.Empty);
                }
            }
            else
            {
                message.AddContent("text/plain", topicMessage.ToString());
                message.Subject = topicMessage.Subject;

                log.LogWarning("error with template name" + topicMessage.TemplateId);
            }

            message.Personalizations = new List<Personalization>()
            {
                personalization
            };

            await emailProvider.AddAsync(message, token).ConfigureAwait(false);
        }



        //From = "Spitball"
        [FunctionName("FunctionSms")]
        public static async Task SmsServiceBusAsync(
            [QueueTrigger(QueueName.SmsQueueName)] SmsMessage2 msg,
            DateTimeOffset insertionTime,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateMessageOptions> options,
            ILogger log,
            CancellationToken token
        )
        {
            //if (message.SystemProperties.DeliveryCount > 2)
            //{
            //    log.LogWarning("invoking message from queue");
            //    return;
            //}

            if (insertionTime < DateTime.UtcNow.AddMinutes(-30))
            {
                log.LogWarning("Too late of a message");
                return;
            }
            //var msg = JsonConvert.DeserializeObject<SmsMessage2>(cloudMessage.AsString);
            //var msg = message.GetBody<SmsMessage2>();


            if (msg.Message == null)
            {
                log.LogError("message is null");
                return;
            }

            if (msg.PhoneNumber == null)
            {
                log.LogError("no phone number");
                return;
            }
            await options.AddAsync(new CreateMessageOptions(new PhoneNumber(msg.PhoneNumber))
            {
                //To = msg.PhoneNumber,
                Body = "Your code to enter into Spitball is: " + msg.Message
            }, token).ConfigureAwait(false);
        }
    }
}
