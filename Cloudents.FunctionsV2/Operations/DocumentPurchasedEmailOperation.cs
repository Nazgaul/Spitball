using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;

namespace Cloudents.FunctionsV2.Operations
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
            //We wait for the db to persist. 
            await Task.Delay(TimeSpan.FromSeconds(30), token);
            var query = new GetDocumentPurchasedEmailQuery(msg.TransactionId);
            var data = await _queryBus.QueryAsync(query, token);
            var template = await GetEmail("DocumentPurchased", data.Language, _queryBus, token);
            var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(data.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));

            foreach (var block in template.Blocks)
            {
                block.Subtitle = block.Subtitle.InjectSingleValue("Tokens", data.Tokens.ToString("f2"));
                block.Body = block.Body.Inject(new
                {
                    data.CourseName,
                    data.DocumentName
                });
            }

            var templateData = new TemplateData()
            {
                Blocks = template.Blocks
                    .Select(s => new Block(s.Title, s.Subtitle, s.Body, s.MinorTitle, s.Cta,
                        _urlBuilder.BuildWalletEndPoint(code))),
                Referral = new Referral(_urlBuilder.BuildShareEndPoint(code)),
                Subject = template.Subject.InjectSingleValue("Tokens", data.Tokens.ToString("f2")),
                To = data.ToEmailAddress,
            };
            await BuildEmail(data.ToEmailAddress, data.Language, binder, templateData, "DocumentPurchased", token);

        }

        private static async Task<EmailObjectDto> GetEmail(string @event,
            Language language, IQueryBus queryBus, CancellationToken token)
        {

            var query = new GetEmailByEventQuery(@event);
            var template2 = await queryBus.QueryAsync(query, token);

            if (template2 == null)
            {
                return null;
            }


            CultureInfo info = language;
            var emailObjects = template2.ToList();
            while (info != null)
            {
                var template1 = emailObjects.FirstOrDefault(f => f.CultureInfo.Equals(info));
                if (template1 != null)
                {
                    return template1;
                }

                if (Equals(info, info.Parent))
                {
                    break;
                }
                info = info.Parent;
            }

            var z = (CultureInfo)Language.English;
            var template = emailObjects.FirstOrDefault(f => f.CultureInfo.Equals(z));
            return template;
        }

        private static async Task BuildEmail(string toAddress, Language language, IBinder binder,
            TemplateData templateData,
            string category,
            CancellationToken token)
        {
            var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            {
                ApiKey = "SendgridKey",
                //From = "Spitball <no-reply @spitball.co>"
            }, token);


            var message = new SendGridMessage
            {
                Asm = new ASM { GroupId = UnsubscribeGroup.Update },
                TemplateId = "d-91a839096c8547f9a028134744e78ecb" 
            };
            if (language.Info.Equals(Language.EnglishIndia.Info))
            {
                message.TemplateId = "d-91a839096c8547f9a028134744e78ecb";
            }
            message.AddFromResource(language.Info);
            templateData.To = toAddress;
            var personalization = new Personalization
            {
                TemplateData = templateData
            };


            message.Personalizations = new List<Personalization>()
            {
                personalization
            };
            message.AddCategory(category);
            message.TrackingSettings = new TrackingSettings
            {
                Ganalytics = new Ganalytics
                {
                    UtmCampaign = category,
                    UtmSource = "SendGrid",
                    UtmMedium = "Email",
                    Enable = true
                }
            };
            message.AddTo(toAddress);
            await emailProvider.AddAsync(message, token);
        }
    }
}