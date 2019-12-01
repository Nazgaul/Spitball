using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using shortid;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Cloudents.Core.DTOs.Email;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Cloudents.FunctionsV2
{
    public static class SmsUnread
    {
        [FunctionName("SmsUnreadQuery")]
        public static async Task SmsUnreadAsync([TimerTrigger("0 */10 5-18 * * *")]TimerInfo myTimer,
            [Blob("spitball/chat/unread.txt")]CloudBlockBlob blob,
            [TwilioSms(AccountSidSetting = "TwilioSid", AuthTokenSetting = "TwilioToken", From = "+1 203-347-4577")] IAsyncCollector<CreateMessageOptions> options,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
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
            var result = (await queryBus.QueryAsync(query, token)).ToList();
            var dataProtector = dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            foreach (var unreadMessageDto in result.Distinct(UnreadMessageDto.UserIdComparer))
            {
                if (unreadMessageDto.ChatMessagesCount == 0)
                {
                    continue;
                }
                CultureInfo.DefaultThreadCurrentCulture = unreadMessageDto.CultureInfo.ChangeCultureBaseOnCountry(unreadMessageDto.Country);

                string text = string.Format(
                    ResourceWrapper.GetString("unread_message"),
                    unreadMessageDto.IsTutor ?
                        ResourceWrapper.GetString("unread_message_student") :
                        ResourceWrapper.GetString("unread_message_tutor"));

                if (unreadMessageDto.ChatMessagesCount == 1)
                {
                    if (unreadMessageDto.IsTutor)
                    {
                        text = ResourceWrapper.GetString("unread_message_first_message_tutor");
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

                try
                {
                    await options.AddAsync(messageOptions, token);
                    await options.FlushAsync(token);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Cant send sms to {unreadMessageDto}");
                }
                var htmlBodyDirection = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";

                var message = new SendGridMessage();
                message.AddTo(unreadMessageDto.Email);
                message.AddContent("text/html", $"<html><body dir=\"{htmlBodyDirection}\">{messageOptions.Body.Replace(". ", ".<br><br>")}</body></html>");
                message.SetSubject(ResourceWrapper.GetString("you_have_unread_message"));
                await emailProvider.AddAsync(message);
                await emailProvider.FlushAsync(token);
               
            }
            if (result.Count > 0)
            {
                version = result.OrderByDescending(o => o.VersionAsLong).First().Version;
                await blob.UploadFromByteArrayAsync(version, 0, 8);
            }
        }




        //[FunctionName("RequestTutorEmail")]
        //public static async Task SendEmailAsync(
        //    [QueueTrigger("request-tutor")] RequestTutorEmailDto obj,
        //    [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
        //    [Inject] ICommandBus commandBus,
        //    [Inject] IUrlBuilder urlBuilder,
        //    CancellationToken token)
        //{

        //    if (string.IsNullOrEmpty(obj.CourseName)) 
        //    {
        //        return;
        //    }
        //    CultureInfo.DefaultThreadCurrentCulture = obj.CultureInfo;
        //    var body = ResourceWrapper.GetString("unread_message_request_email_body");
        //    var request = string.Empty;
        //    //TODO -  whatsapp link
        //    if (!string.IsNullOrEmpty(obj.Request))
        //    {
        //        request = ResourceWrapper.GetString("unread_message_request_email_body_lead_request")
        //            .InjectSingleValue("Request", obj.Request);
        //    }


        //    var whatsAppLink = new UriBuilder($"https://wa.me/{obj.FirstMessagePhoneNumber.Replace("+",string.Empty)}")
        //        .AddQuery(new
        //        {
        //            text = ResourceWrapper.GetString("unread_message_request_email_body_whatsapp_text").InjectSingleValue("CourseName", obj.CourseName)
        //        });

        //    var identifier = ShortId.Generate(true, false);
        //    var command = new CreateShortUrlCommand(identifier, whatsAppLink.ToString(), DateTime.UtcNow.AddDays(30));
        //    await commandBus.DispatchAsync(command, token);
        //    var urlShort = urlBuilder.BuildShortUrlEndpoint(identifier);
        //    body = body.InjectSingleValue("Request", request);

        //    body = body.InjectSingleValue("WhatsappLink", urlShort);

        //    var htmlBodyDirection = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft? "rtl" :"ltr";
        //    body = body.Inject(new
        //    {
        //        Request = request,
        //        WhatsappLink = whatsAppLink.ToString(),
        //        obj.UserFirstName,
        //        link = obj.Url,
        //        obj.FirstMessageStudentName,
        //        obj.CourseName
        //    });
        //    var message = new SendGridMessage()
        //    {

        //        Subject = ResourceWrapper.GetString("unread_message_request_email_subject")
        //            .Inject(obj),
        //        HtmlContent =$"<html><body dir=\"{htmlBodyDirection}\">{body.Replace(Environment.NewLine, "<br><br>")}</body></html>" 


        //    };
        //    message.AddTo(obj.To);
        //    await emailProvider.AddAsync(message, token);
        //}


        //public class RequestTutorEmailDto
        //{
        //    public CultureInfo CultureInfo { get; set; }

        //    public string Request { get; set; }

        //    public string FirstMessagePhoneNumber { get; set; }
        //    public string FirstMessageStudentName { get; set; }

        //    public string CourseName { get; set; }

        //    public string UserFirstName { get; set; }

        //    public string Url { get; set; }

        //    public string To { get; set; }
        //}
        public class UnreadEmailto
        {
            public string Email { get; set; }
            public string From { get; set; }
            public string Body { get; set; }
            public string Subject { get; set; }
        }
    }
}