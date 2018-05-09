using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;
using Twilio.Types;

namespace Cloudents.Functions
{
    public static class CommunicationFunction
    {
        [FunctionName("FunctionEmail")]
        [return: SendGrid(ApiKey = "SendgridKey", From = "no-reply@spitball.co")]
        public static SendGridMessage EmailFunction([TimerTrigger("0 */1 * * * *")]TimerInfo myQueueItem, TraceWriter log)
        {
            var message = new SendGridMessage()
            {
                Subject = "hi",
                HtmlContent = "hi ram",


            };
            message.AddTo("ram@cloudents.com");
            return message;
        }

        [FunctionName("Temp")]
        public static async Task Temp(
            [TimerTrigger("0 * * * * *")]TimerInfo myTimer,
            [Queue(QueueName.SmsName, Connection = "TempConnection")]
            IAsyncCollector<SmsMessage> queue,
            TraceWriter log)
        {
            var message = new SmsMessage
            {
                PhoneNumber = "+972542642202",
                Message = "820909"
            };
            await queue.AddAsync(message);
            await queue.FlushAsync();
            log.Info("ok");
        }

        [FunctionName("SmsQueue")]
        public static async Task SmsQueue(
            [QueueTrigger(QueueName.SmsName, Connection = "TempConnection")] SmsMessage queueMessage,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "(203) 347-4577")] IAsyncCollector<CreateMessageOptions> options,
            CancellationToken token)
        {
            var result = await BuildAndValidateSmsAsync(queueMessage);
            if (result == null)
            {
                return;
            }
            await options.AddAsync(result, token);
        }

        [FunctionName("SmsHttp")]
        public static async Task<IActionResult> SmsHttp(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sms")]HttpRequest req,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "(203) 347-4577")] IAsyncCollector<CreateMessageOptions> options,
            CancellationToken token)
        {
            var jsonContent = await req.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<SmsMessage>(jsonContent);

            var result = await BuildAndValidateSmsAsync(message);
            if (result == null)
            {
                return new BadRequestResult();
            }
            await options.AddAsync(result, token);
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
                    //message.To = new PhoneNumber("+972542642202");
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
