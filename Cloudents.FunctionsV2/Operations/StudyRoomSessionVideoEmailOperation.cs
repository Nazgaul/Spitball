using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Message.Email;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;

namespace Cloudents.FunctionsV2.Operations
{
    public class StudyRoomSessionVideoEmailOperation : ISystemOperation<StudyRoomVideoMessage>
    {
        public async Task DoOperationAsync(StudyRoomVideoMessage msg, IBinder binder, CancellationToken token)
        {

            var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            {
                ApiKey = "SendgridKey",
                From = "Spitball <no-reply @spitball.co>"
            }, token);


            var message = new SendGridMessage
            {
                Asm = new ASM { GroupId = UnsubscribeGroup.Update },
                TemplateId = msg.Info.Equals(Language.English.Info) ? "d-8cb8934b45794ab999de2b5c24d145c0" : "d-f35043e77b0d462eb7f352c40590b128"
            };

            CultureInfo.CurrentUICulture = msg.Info;

            var personalization = new Personalization
            {
                TemplateData = new
                {
                    firstName = msg.UserName,
                    tutorName = msg.TutorFirstName,
                    date = msg.DateTime.ToString("d"),
                    link = msg.DownloadLink,
                    to = msg.To
                }
            };


            message.Personalizations = new List<Personalization>()
            {
                personalization
            };
            message.AddCategory("Session-Video");
            message.TrackingSettings = new TrackingSettings
            {
                Ganalytics = new Ganalytics
                {
                    UtmCampaign = "Session-Video",
                    UtmSource = "SendGrid",
                    UtmMedium = "Email",
                    Enable = true
                }
            };
            message.AddTo(msg.To);
            await emailProvider.AddAsync(message, token);
        }
    }
}
