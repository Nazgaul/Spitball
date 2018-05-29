using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;
using Twilio.Types;

namespace Cloudents.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CommunicationFunction
    {
        [FunctionName("FunctionEmail")]
        public static async Task EmailFunctionAsync(
            [QueueTrigger(QueueName.EmailName)] CloudQueueMessage queueMessage,
            [SendGrid(ApiKey = "SendgridKey", From = "no-reply@spitball.co")] IAsyncCollector<SendGridMessage> emailProvider,
            IBinder binder,
            TraceWriter log,
            CancellationToken token)
        {
            var emailParams =  JsonConvert.DeserializeObject<RegistrationEmail>(queueMessage.AsString, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            if (emailParams == null)
            {
                log.Error("error deSerializing" );
                return;
            }
            //TODO: dynamic binding somehow doesn't work skip that for now
            var dynamicBlobAttribute = new BlobAttribute("mailcontainer/Spitball/register-mail.html");

            var htmlTemplate = await binder.BindAsync<string>(dynamicBlobAttribute, token).ConfigureAwait(false);
            if (htmlTemplate == null)
            {
                log.Error("error with template name" + emailParams.Template);
                return;
            }

            var content = htmlTemplate.Inject(emailParams);

            var message = new SendGridMessage
            {
                Subject = "welcome to spitball",
                HtmlContent = content,
            };
            message.AddTo(emailParams.To);
            await emailProvider.AddAsync(message, token).ConfigureAwait(false);
        }

        //[FunctionName("Temp")]
        //public static async Task Temp(
        //    [TimerTrigger("0 * * * * *")]TimerInfo myTimer,
        //    [Queue(QueueName.SmsName, Connection = "TempConnection")]
        //    IAsyncCollector<A> queue,
        //    TraceWriter log)
        //{
        //    var b = new B()
        //    {
        //        a = 1,
        //        aa = 2
        //    };
        //    await queue.AddAsync(b).ConfigureAwait(false);
        //    await queue.FlushAsync().ConfigureAwait(false);
        //    log.Info("ok");
        //}


      





        //[FunctionName("SmsQueue")]
        //[return: TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "(203) 347-4577")]
        //public static CreateMessageOptions SmsQueue(
        //    [QueueTrigger(QueueName.SmsName, Connection = "TempConnection")] SmsMessage queueMessage)
        //{
        //    return new CreateMessageOptions(new PhoneNumber(queueMessage.PhoneNumber))
        //    {
        //        Body = queueMessage.Message
        //    };
        //}

        [FunctionName("SmsHttp")]
        public static async Task<IActionResult> SmsHttpAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sms")]HttpRequest req,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "(203) 347-4577")] IAsyncCollector<CreateMessageOptions> options,
            CancellationToken token)
        {
            var jsonContent = await req.ReadAsStringAsync().ConfigureAwait(false);
            var message = JsonConvert.DeserializeObject<SmsMessage>(jsonContent);

            var result = await BuildAndValidateSmsAsync(message).ConfigureAwait(false);
            if (result == null)
            {
                return new BadRequestResult();
            }
            await options.AddAsync(result, token).ConfigureAwait(false);
            return new OkResult();

        }


        [ItemCanBeNull]
        private static async Task<CreateMessageOptions> BuildAndValidateSmsAsync(SmsMessage message)
        {
            try
            {
                TwilioClient.Init(InjectConfiguration.GetEnvironmentVariable("TwilioSid"),
                    InjectConfiguration.GetEnvironmentVariable("TwilioToken"));
                var result = await PhoneNumberResource.FetchAsync(new PhoneNumber(message.PhoneNumber)).ConfigureAwait(false);
                return new CreateMessageOptions(result.PhoneNumber)
                {
                    Body = message.Message
                };
            }
            catch (Twilio.Exceptions.ApiException)
            {
                return null;
            }

        }

        

    }

}
