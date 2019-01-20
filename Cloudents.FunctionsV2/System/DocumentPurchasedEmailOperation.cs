using System;
using System.Collections.Generic;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Cloudents.FunctionsV2.System
{
    public class DocumentPurchasedEmailOperation : ISystemOperation<DocumentPurchasedMessage>
    {
        private readonly IQueryBus _queryBus;
        private readonly IDataProtectionProvider _dataProtectProvider;
        private readonly IUrlBuilder _urlBuilder;

        public DocumentPurchasedEmailOperation(IQueryBus queryBus, IDataProtectionProvider dataProtectProvider, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _dataProtectProvider = dataProtectProvider;
            _urlBuilder = urlBuilder;
        }

        //DocumentPurchasedMessage
        public async Task DoOperationAsync(DocumentPurchasedMessage msg, IBinder binder, CancellationToken token)
        {
            var query = new GetDocumentPurchasedEmail(msg.TransactionId);
            var result = await _queryBus.QueryAsync(query, token);

            var dataProtector = _dataProtectProvider.CreateProtector("MarkAnswerAsCorrect")
                .ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(result.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));


            foreach (var block in result.Blocks)
            {
                block.Body = block.Body.Inject(new
                {
                    result.CourseName,
                    result.DocumentName
                });
                if (block.Cta != null)
                {
                    block.Url = _urlBuilder.BuildWalletEndPoint(new { code });
                }
                

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