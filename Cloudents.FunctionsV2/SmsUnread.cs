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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class SmsUnread
    {
        [FunctionName("SmsUnread")]
        public static async Task SmsUnreadAsync([TimerTrigger("0 */10 5-18 * * *")]TimerInfo myTimer,
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
            var dataProtector = dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            foreach (var unreadMessageDto in result.Distinct(UnreadMessageDto.UserIdComparer))
            {
                if (unreadMessageDto.ChatMessagesCount == 0)
                {
                    continue;
                }
                var text =
                    $"You have a new message from your {(unreadMessageDto.IsTutor ? "student" : "tutor")} on Spitball. Click on the link to read your message {{link}}";
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

                try
                {
                    await options.AddAsync(messageOptions, token);
                    await options.FlushAsync(token);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Cant send sms to {unreadMessageDto}");
                }

            }
            if (result.Count > 0)
            {
                version = result.OrderByDescending(o => o.VersionAsLong).First().Version;
                await blob.UploadFromByteArrayAsync(version, 0, 8);
            }
        }
    }
}