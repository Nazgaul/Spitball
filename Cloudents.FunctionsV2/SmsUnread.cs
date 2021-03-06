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
using Cloudents.FunctionsV2.Operations;
using SendGrid.Helpers.Mail;
using shortid.Configuration;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class SmsUnread
    {
        [FunctionName("SmsUnreadQuery")]
        public static async Task SmsUnreadAsync([TimerTrigger("0 */10 * * * *")] TimerInfo myTimer,
            [Blob("spitball/chat/unread.txt")] CloudBlockBlob blob,
            [SendGrid(ApiKey = "SendgridKey")] IAsyncCollector<SendGridMessage> emailProvider,
            [Inject] IQueryBus queryBus,
            [Inject] ICommandBus commandBus,
            [Inject] IDataProtectionProvider dataProtectProvider,
            [Inject] IUrlBuilder urlBuilder,
            ILogger log,
            CancellationToken token)
        {
            byte[]? version = null;
            if (await blob.ExistsAsync())
            {
                version = new byte[8];
                await blob.DownloadToByteArrayAsync(version, 0);
                log.LogInformation($"processing version {BitConverter.ToString(version).Replace("-", "")}");
            }

            var query = new UserUnreadMessageQuery(version);
            var result = (await queryBus.QueryAsync(query, token)).ToList();
            var dataProtector = dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            foreach (var unreadMessageDto in result.Distinct(UnreadMessageDto.UserIdComparer))
            {
                try
                {
                    if (unreadMessageDto.ChatMessagesCount == 0)
                    {
                        continue;
                    }

                    if (unreadMessageDto.Country is null)
                    {
                        continue;

                    }
                    log.LogInformation($"processing {unreadMessageDto}");
                    CultureInfo.DefaultThreadCurrentCulture = unreadMessageDto.CultureInfo.ChangeCultureBaseOnCountry(unreadMessageDto.Country);

                    var code = dataProtector.Protect(unreadMessageDto.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));
                    var identifier = ShortId.Generate(new GenerationOptions() {
                        UseNumbers = true,
                        UseSpecialCharacters = false}
                    );
                    var url = urlBuilder.BuildChatEndpoint(code, unreadMessageDto.Identifier, new { utm_source = "SMS-auto" });
                    var command = new CreateShortUrlCommand(identifier, url.PathAndQuery, DateTime.UtcNow.AddDays(5));
                    await commandBus.DispatchAsync(command, token);
                    var urlShort = urlBuilder.BuildShortUrlEndpoint(identifier, unreadMessageDto.Country);

                    var htmlBodyDirection = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";

                    var message = new SendGridMessage();
                    message.AddTo(unreadMessageDto.Email);

                    var body = GetText(unreadMessageDto.IsTutor, unreadMessageDto.ChatMessagesCount, true).Inject(new { link = urlShort });


                    message.AddContent("text/html", $"<html>" +
                                                    $"<body dir=\"{htmlBodyDirection}\">{body}</body></html>");
                    message.SetSubject(ResourceWrapper.GetString("you_have_unread_message"));
                    message.AddFromResource(CultureInfo.DefaultThreadCurrentCulture);
                    await emailProvider.AddAsync(message, token);
                    await emailProvider.FlushAsync(token);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Cant send sms/email to {unreadMessageDto}");
                }

            }
            if (result.Count > 0)
            {
                version = result.OrderByDescending(o => o.VersionAsLong).First().Version;
                await blob.UploadFromByteArrayAsync(version, 0, 8);
            }
        }

        private static string GetText(bool isTutor, int count, bool email)
        {
            var suffix = email ? "_email" : "";
            if (isTutor && count == 1)
            {
                return ResourceWrapper.GetString($"unread_message_first_message_tutor{suffix}");

            }

            return string.Format(
                ResourceWrapper.GetString($"unread_message{suffix}"),
                isTutor ?
                    ResourceWrapper.GetString("unread_message_student") :
                    ResourceWrapper.GetString("unread_message_tutor"));
        }
    }
}