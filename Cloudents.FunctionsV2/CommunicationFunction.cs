using Autofac;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.System;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using shortid;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
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


            var message = new UpdatesEmailMessage();
            

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


            if (topicMessage.TemplateId != null)
            {
                message.Asm = new ASM
                {
                    GroupId = 10926
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

        [FunctionName("SmsUnread")]
        public static async Task SmsUnreadAsync([TimerTrigger("0 */10 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("spitball/chat/unread.txt")]CloudBlockBlob blob,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateMessageOptions> options,
            [Inject] IQueryBus queryBus,
            [Inject] ICommandBus commandBus,
            [Inject] IDataProtectionProvider dataProtectProvider,
            [Inject] IUrlBuilder urlBuilder,
            ILogger log,
            CancellationToken token)
        {
            byte[] version = null;
            if (await blob.ExistsAsync())
            {
                version = new byte[8];
                await blob.DownloadToByteArrayAsync(version, 0);
            }

            var query = new UserUnreadMessageQuery(version);
            var result = await queryBus.QueryAsync(query, token);
            var tasks = new List<Task>();
            var dataProtector = dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            foreach (var unreadMessageDto in result.Distinct(UnreadMessageDto.UserIdComparer))
            {
                if (unreadMessageDto.ChatMessagesCount == 0)
                {
                    continue;
                }
                var text =
                    $"You have a new message from your {(unreadMessageDto.IsTutor ? "student" : "tutor")} on Spitball. Click on the link to read your message {{link}} ";
                if (unreadMessageDto.ChatMessagesCount == 1)
                {
                    if (unreadMessageDto.IsTutor)
                    {
                        text = "We found a student that wants a tutoring session with you. Click here {link} to chat and schedule a lesson.";
                    }
                    else
                    {
                        log.LogError($"this is first message and its not tutor {unreadMessageDto}");
                    }
                }

                var code = dataProtector.Protect(unreadMessageDto.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));
                var identifier = ShortId.Generate(true, false);



                var url = urlBuilder.BuildChatEndpoint(code, new { utm_source = "SMS-auto" });
                var command = new CreateShortUrlCommand(identifier, url.PathAndQuery, DateTime.UtcNow.AddDays(5));
                await commandBus.DispatchAsync(command, token);

                var urlShort = urlBuilder.BuildShortUrlEndpoint(identifier);

                var messageOptions = new CreateMessageOptions(new PhoneNumber(unreadMessageDto.PhoneNumber))
                {
                    Body = text.Inject(new { link = urlShort })

                };
                if (unreadMessageDto.PhoneNumber.StartsWith("+972"))
                {
                    messageOptions.From = "Spitball";
                }
                var t = options.AddAsync(messageOptions, token);

                tasks.Add(t);
            }

            await Task.WhenAll(tasks);
            if (result.Count > 0)
            {
                version = result.OrderByDescending(o => o.VersionAsLong).First().Version;
                await blob.UploadFromByteArrayAsync(version, 0, 8);
            }
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
                Body = /*"Your code to enter into Spitball is: " +*/ msg.Message
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
