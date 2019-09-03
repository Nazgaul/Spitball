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
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });

            if (topicMessage == null)
            {
                log.LogError("error with parsing message");
                return;
            }

            await ProcessEmail(emailProvider, log, topicMessage, token);

            log.LogInformation("finish sending email");
        }
      

        private static async Task ProcessEmail(IAsyncCollector<SendGridMessage> emailProvider, ILogger log,
            BaseEmail topicMessage, CancellationToken token)
        {
            var message = new SendGridMessage();
            var personalization = new Personalization();


            if (topicMessage.TemplateId != null)
            {
                message.Asm = new ASM
                {
                    GroupId = topicMessage.UnsubscribeGroup
                };
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

            var messageOptions = new CreateMessageOptions(new PhoneNumber(msg.PhoneNumber))
            {
                Body = msg.Message
            };
            if (msg.PhoneNumber.StartsWith("+972"))
            {
                messageOptions.From = "Spitball";
            }
            await options.AddAsync(messageOptions, token);
        }

        [FunctionName("FunctionPhoneServiceBus")]
        public static async Task CallServiceBusAsync(
            [ServiceBusTrigger("communication", "call", Connection = "AzureWebJobsServiceBus")] SmsMessage msg,
            [TwilioCall(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateCallOptions> options)
        {
            var from = new PhoneNumber("+1 203-347-4577");
            var to = new PhoneNumber(msg.PhoneNumber);

            var hostName2 = GetHostUri();

            var uriBuilder = new UriBuilder(hostName2)
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

        public static Uri GetHostUri()
        {
            var hostName2 = Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%");
            //var hostName2 = string.Format("http://{0}.azurewebsites.net",
            //    Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%"));
            if (hostName2.Contains("localhost", StringComparison.OrdinalIgnoreCase))
            {
                hostName2 = "spitball-function-dev2.azurewebsites.net";
            }

            var uri = new UriBuilder("https", hostName2.TrimEnd('/'));
            return uri.Uri;


            //return hostName2;
        }

        [FunctionName("TwilioMessage")]
        public static IActionResult RunTwilioResult(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "twilio")]
            HttpRequest req, ILogger log)
        {
            string name = req.Query["code"];
            var twiml = new VoiceResponse();
            twiml.Say($"Your code to spitball is, {string.Join(". ", name.ToCharArray())}", loop: 3, voice: "alice");
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

}
