using System.Collections.Generic;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class DocumentPurchasedEmailOperation : ISystemOperation<DocumentPurchasedMessage>
    {
        private readonly IQueryBus _queryBus;

        public DocumentPurchasedEmailOperation(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        //DocumentPurchasedMessage
        public async Task DoOperationAsync(DocumentPurchasedMessage msg, IBinder binder, CancellationToken token)
        {
            var query = new GetDocumentPurchasedEmail(msg.TransactionId);
            var result = await _queryBus.QueryAsync(query, token);

            foreach (var block in result.Blocks)
            {
                block.Body = block.Body.Inject(new
                {
                    result.CourseName,
                    result.DocumentName
                });

            }

            var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            {
                ApiKey = "SendgridKey",
                From = "Spitball <no-reply @spitball.co>"
            }, token);


            var message = new SendGridMessage();
            message.Asm = new ASM
            {
                GroupId = 10926
            };
            message.TemplateId = "d-91a839096c8547f9a028134744e78ecb";
            var personalization = new Personalization();

            personalization.TemplateData = new TemplateData(result.Blocks, result.SocialShare, result.Language);

            message.Personalizations = new List<Personalization>()
            {
                personalization
            };
            message.Subject = result.Subject;
            message.AddCategory("DocumentPurchased");
            message.TrackingSettings = new TrackingSettings
            {
                Ganalytics = new Ganalytics
                {
                    UtmCampaign = "DocumentPurchased",
                    UtmSource = "SendGrid",
                    UtmMedium = "Email",
                    Enable = true
                }
            };
            message.AddTo(result.ToEmailAddress);
            await emailProvider.AddAsync(message, token);

        }
    }
}