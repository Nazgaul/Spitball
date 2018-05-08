using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using Twilio.Rest.Api.V2010.Account;
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



        [FunctionName("FunctionSms")]
        [return: TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "(203) 347-4577")]
        public static CreateMessageOptions SmsFunction(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            TraceWriter log)
        {

            var message = new CreateMessageOptions(new PhoneNumber("+972542642202"));
            //message.To = new PhoneNumber("+972542642202");
            message.Body = "Hello Ram, thanks for your order!";

            return message;
        }
    }
}
