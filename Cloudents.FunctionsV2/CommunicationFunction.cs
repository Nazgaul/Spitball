using System;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.FunctionsV2.System;
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

        [FunctionName("FunctionEmailTest")]
        public static async Task EmailFunctionTimerAsync(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply @spitball.co>")]
            IAsyncCollector<SendGridMessage> emailProvider,

            [Inject] ILifetimeScope lifetimeScope,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {


            var message = new AnswerAcceptedMessage(Guid.Parse("AD796D3F-734D-4987-AA1F-A9C700C88DD1"));

            var handlerType =
                typeof(ISystemOperation<>).MakeGenericType(message.GetType());
            using (var child = lifetimeScope.BeginLifetimeScope())
            {
                dynamic operation = child.Resolve(handlerType);
                await operation.DoOperationAsync((dynamic)message, binder, token);
            }
        }

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
        [JsonProperty("blocks")]
        public IEnumerable<Block> Blocks { get; set; }
        [JsonProperty("referral")]
        public Referral Referral { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
       
    }

    public class Referral
    {
        public Referral( string link)
        {
            Link = link;
        }

       
        [JsonProperty("link")]
        public string Link { get; set; }
    }

  

    public class Block
    {
        public Block(string title, string subtitle, string body, string minorTitle)
        {
            Title = title;
            Subtitle = subtitle;
            Body = body;
            MinorTitle = minorTitle;
        }

        public Block(string title, string subtitle, string body, string minorTitle, string cta, string url )
        {
            Title = title;
            Subtitle = subtitle;
            Body = body;
            Cta = cta;
            Url = url;
            MinorTitle = minorTitle;
        }

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
        [JsonProperty("minorTitle")]
        public string MinorTitle { get; set; }
    }


    public class EmailObject
    {
        public string Id { get; set; }
        public bool SocialShare { get; set; }

        public string Subject { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public IEnumerable<EmailBlock> Blocks { get; set; }
    }

    public class EmailBlock
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string Cta { get; set; }
        public string MinorTitle { get; set; }
    }

}
