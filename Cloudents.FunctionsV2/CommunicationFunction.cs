using Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
            var topicMessage = JsonConvert.DeserializeObject<BaseEmail>(cloudMessage.AsString, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });

            if (topicMessage == null)
            {
                log.LogError("error with parsing message");
                return;
            }

            await ProcessEmail(emailProvider, log, topicMessage, token);

            log.LogInformation("finish sending email");
        }

        //[FunctionName("FunctionEmailTest")]
        //public static async Task EmailFunctionTimerAsync(
        //    [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
        //    [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply @spitball.co>")]
        //    IAsyncCollector<SendGridMessage> emailProvider,

        //    [Inject] ILifetimeScope lifetimeScope,
        //    IBinder binder,
        //    ILogger log,
        //    CancellationToken token)
        //{


        //    var message = new DocumentPurchasedMessage(Guid.Parse("439B602A-421F-40F8-8A97-A9C60102D069"));

        //    var handlerType =
        //        typeof(ISystemOperation<>).MakeGenericType(message.GetType());
        //    using (var child = lifetimeScope.BeginLifetimeScope())
        //    {
        //        dynamic operation = child.Resolve(handlerType);
        //        await operation.DoOperationAsync((dynamic)message, binder, token);
        //    }
        //}

        private static async Task ProcessEmail(IAsyncCollector<SendGridMessage> emailProvider, ILogger log,
            BaseEmail topicMessage, CancellationToken token)
        {
            var message = new SendGridMessage();
            var personalization = new Personalization();
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
                    message.TrackingSettings = new TrackingSettings
                    {
                        Ganalytics = new Ganalytics
                        {
                            UtmCampaign = topicMessage.Campaign,
                            UtmSource = "SendGrid",
                            UtmMedium = "Email",
                            Enable = true
                        }
                    };
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
            message.AddTo(topicMessage.To);

            await emailProvider.AddAsync(message, token);
        }



        //[FunctionName("FunctionSms")]
        //public static async Task SmsStorageQueueAsync(
        //    [QueueTrigger(QueueName.SmsQueueName)] SmsMessage2 msg,
        //    DateTimeOffset insertionTime,
        //    [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateMessageOptions> options,
        //    ILogger log,
        //    CancellationToken token
        //)
        //{
        //    if (insertionTime < DateTime.UtcNow.AddMinutes(-30))
        //    {
        //        log.LogWarning("Too late of a message");
        //        return;
        //    }

        //    await ProcessSmsMessageAsync(msg, options, log, token);
        //}


        [FunctionName("FunctionSmsServiceBus")]
        public static async Task SmsServiceBusAsync(
            [ServiceBusTrigger("sms", Connection = "AzureWebJobsServiceBus")] SmsMessage2 msg,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateMessageOptions> options,
            ILogger log,
            CancellationToken token
        )
        {
            await ProcessSmsMessageAsync(msg, options, log, token);
        }

        private static async Task ProcessSmsMessageAsync(SmsMessage2 msg, IAsyncCollector<CreateMessageOptions> options, ILogger log,
            CancellationToken token)
        {
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
                Body = "Your code to enter into Spitball is: " + msg.Message
            }, token);
        }
    }

    public class TemplateData
    {
        public TemplateData(IEnumerable<EmailBlockDto> blocks, bool socialShare, Language language)
        {
            Blocks = blocks.Select(s => new Block()
            {
                Body = s.Body,
                Cta = s.Cta,
                Subtitle = s.Subtitle,
                Title = s.Title,
                Url = s.Url
            });
            if (socialShare)
            {
                Referral = new Referral(language);
            }
        }

        [JsonProperty("blocks")]
        public IEnumerable<Block> Blocks { get; set; }
        [JsonProperty("referral")]
        public Referral Referral { get; set; }
    }

    public class Referral
    {
        public Referral(Language language)
        {
            Lang = new Lang(language);
        }

        [JsonProperty("lang")]
        public Lang Lang { get; set; }
    }

    public class Lang
    {
        public Lang(Language language)
        {
            if (language == Language.English)
            {
                English = true;
            }
            else
            {
                Hebrew = true;
            }
        }

        [JsonProperty("english")]
        public bool English { get; private set; }
        [JsonProperty("hebrew")]
        public bool Hebrew { get; private set; }
    }

    public class Block
    {
        [JsonProperty("title")]

        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("cta")]
        public string Cta { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }

}
