using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using Cloudents.Infrastructure.Framework;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Twilio;

namespace Cloudents.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CommunicationFunction
    {
        [FunctionName("FunctionEmail")]
        public static async Task EmailFunctionAsync([ServiceBusTrigger(TopicSubscription.Communication, nameof(TopicSubscription.Email))]BrokeredMessage brokeredMessage,
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
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "(203) 347-4577")] ICollector<SMSMessage> options
            //CancellationToken token
            )
        {

            var jsonContent = await req.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = JsonConvert.DeserializeObject<SmsMessage>(jsonContent);
            //var result = await BuildAndValidateSmsAsync(message).ConfigureAwait(false);
            //if (result == null)
            //{
            //return req.CreateResponse(HttpStatusCode.BadRequest);
            //}

            options.Add(new SMSMessage
            {
                To = message.PhoneNumber,
                Body = message.Message
            });
            return req.CreateResponse(HttpStatusCode.OK);

        }


        //[ItemCanBeNull]
        //private static async Task<CreateMessageOptions> BuildAndValidateSmsAsync(SmsMessage message)
        //{
        //    try
        //    {
        //        TwilioClient.Init(InjectConfiguration.GetEnvironmentVariable("TwilioSid"),
        //            InjectConfiguration.GetEnvironmentVariable("TwilioToken"));
        //        var result = await PhoneNumberResource.FetchAsync(new PhoneNumber(message.PhoneNumber)).ConfigureAwait(false);
        //        return new CreateMessageOptions(result.PhoneNumber)
        //        {
        //            Body = message.Message
        //        };
        //    }
        //    catch (Twilio.Exceptions.ApiException)
        //    {
        //        return null;
        //    }
        //}
    }
}
