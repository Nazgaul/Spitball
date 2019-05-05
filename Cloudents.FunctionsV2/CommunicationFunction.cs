using Cloudents.Core.Extension;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.Binders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;
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


        //    var message = new DocumentPurchasedMessage(Guid.Parse("9D4463F7-CD2C-4091-87AA-A9C8008D922E"));

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
                message.Personalizations = new List<Personalization>()
                {
                    personalization
                };
            }
            else
            {
                message.AddContent("text/html", topicMessage.ToString());
                message.Subject = topicMessage.Subject;
                if (topicMessage.Bcc != null)
                {
                    foreach (var bcc in topicMessage.Bcc)
                    {
                        message.AddBcc(bcc);
                    }
                }
                log.LogWarning("error with template name" + topicMessage.TemplateId);
            }

            
            message.AddTo(topicMessage.To);

            await emailProvider.AddAsync(message, token);
        }




        [FunctionName("FunctionSmsServiceBus")]
        public static async Task SmsServiceBusAsync(
            [ServiceBusTrigger("communication", "sms", Connection = "AzureWebJobsServiceBus")] SmsMessage msg,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateMessageOptions> options,
            ILogger log,
            CancellationToken token
        )
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

        [FunctionName("FunctionPhoneServiceBus")]
        public static async Task CallServiceBusAsync(
            [ServiceBusTrigger("communication", "call", Connection = "AzureWebJobsServiceBus")] SmsMessage msg,
            [TwilioCall(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateCallOptions> options,
            ILogger log
            )
        {
            var from = new PhoneNumber("+1 203-347-4577");
            var to = new PhoneNumber(msg.PhoneNumber);


            var hostName2 = string.Format("http://{0}.azurewebsites.net", Environment.ExpandEnvironmentVariables("%WEBSITE_SITE_NAME%"));
            if (hostName2.Contains("localhost", StringComparison.OrdinalIgnoreCase))
            {
                hostName2 = "https://spitball-function-dev2.azurewebsites.net";
            }

            hostName2 = hostName2.TrimEnd('/');

            var uriBuilder = new UriBuilder(new Uri(hostName2))
            {
                Path = "/api/twilio",
            };
            uriBuilder.AddQuery(new NameValueCollection()
            {
                ["code"] = msg.Message
            });
            var call = new CreateCallOptions(to, from)
            {
                Url = uriBuilder.Uri,
                MachineDetection = "Enable"
            };
            await options.AddAsync(call);

        }





        [FunctionName("TwilioMessage")]
        public static IActionResult RunTwilioResult(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "twilio")]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["code"];
            var twiml = new VoiceResponse();
            twiml.Say($"Your code to spitball is, {string.Join(". ",name.ToCharArray())}", loop: 3, voice: "alice");
            return new ContentResult()
            {
                Content = twiml.ToString(),
                ContentType = "application/xml"
            };
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
        public Referral(string link)
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

        public Block(string title, string subtitle, string body, string minorTitle, string cta, string url)
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


    //public class EmailObject
    //{
    //    public string Id { get; set; }
    //    public bool SocialShare { get; set; }
    //    public string Event { get; set; }

    //    public string Subject { get; set; }

    //    public CultureInfo CultureInfo { get; set; }

    //    public IEnumerable<EmailBlock> Blocks { get; set; }

    //}



    //public class EmailBlock
    //{
    //    public string Title { get; set; }
    //    public string Subtitle { get; set; }
    //    public string Body { get; set; }
    //    public string Cta { get; set; }
    //    public string MinorTitle { get; set; }
    //}

}
