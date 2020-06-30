using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using Cloudents.Query.StudyRooms;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class StudyRoomFunctions
    {
        [FunctionName("LiveEventReminder")]
        public static async Task LiveEventReminderAsync([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer,
            [Inject] IQueryBus queryBus,
            [Inject] IDataProtectionService dataProtectService,
            [Inject] IUrlBuilder urlBuilder,
            [SendGrid(ApiKey = "SendgridKey", From = "no-reply@spitball.co")] IAsyncCollector<SendGridMessage> emailProvider,
            ILogger log, CancellationToken token)
        {
            //TODO - get the 24 hours live event
            var query = new LiveClassesReminderQuery();
            var result = await queryBus.QueryAsync(query, token);

            foreach (var reminderDto in result.tenMinuted)
            {

                var code = dataProtectService.ProtectData(reminderDto.UserId.ToString(), DateTimeOffset.UtcNow.AddHours(2));

                reminderDto.StudyRoomLink =
                    urlBuilder.BuildStudyRoomEndPoint(reminderDto.StudyRoomId, new {token = code});
                var message = new SendGridMessage();
                message.AddTo(reminderDto.StudentEmail);

                //var body = GetText(unreadMessageDto.IsTutor, unreadMessageDto.ChatMessagesCount, true).Inject(new { link = urlShort });
                message.AddContent(
                    "text/html",
                    TenMinutesTemplate.Inject(reminderDto).Replace(Environment.NewLine,"<br>")
                    );

                message.SetSubject("Your Live Class is about to start");
                await emailProvider.AddAsync(message, token);
            }


            foreach (var reminderDto in result.daysBefore)
            {

                var code = dataProtectService.ProtectData(reminderDto.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(1.5));

                reminderDto.StudyRoomLink =
                    urlBuilder.BuildStudyRoomEndPoint(reminderDto.StudyRoomId, new {token = code});
                var message = new SendGridMessage();
                message.AddTo(reminderDto.StudentEmail);

                //var body = GetText(unreadMessageDto.IsTutor, unreadMessageDto.ChatMessagesCount, true).Inject(new { link = urlShort });
                message.AddContent(
                    "text/html",
                    DayTemplate.Inject(reminderDto).Replace(Environment.NewLine,"<br>")
                );

                message.SetSubject($"Your Live Class {reminderDto.StudyRoomTitle} is coming up tomorrow");
                await emailProvider.AddAsync(message, token);
            }
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private const string DayTemplate = @"Hey {StudentFirstName},

Join {TeacherName} In 24 hours for the Live Class {StudyRoomTitle}

{StudyRoomDescription}

Link: {StudyRoomLink}

Date: {BroadCastTime:D}
Time: {BroadCastTime:T} UTC

Looking forward to see you at the Live Class
";

        private const string TenMinutesTemplate = @"Hey {StudentFirstName}

In just a few minutes, your live class {StudyRoomTitle} is about to start. Please, proceed to click on the link to access the Live Class

Link :  {StudyRoomLink}

{StudyRoomDescription}

Date: {BroadCastTime:D}

Time: {BroadCastTime:T} UTC


Looking forward to see you at the Live Class";
    }
}