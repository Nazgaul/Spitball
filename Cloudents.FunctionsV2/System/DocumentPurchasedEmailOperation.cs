﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            var query = new GetDocumentPurchasedEmailQuery(msg.TransactionId);
            var data = await _queryBus.QueryAsync(query, token);

            var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(data.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));

            //foreach (var block in result.Blocks)
            //{
            //    block.Subtitle = block.Subtitle.InjectSingleValue("Tokens", data.Tokens.ToString("f2"));
            //    block.Body = block.Body.Inject(new
            //    {
            //        result.CourseName,
            //        result.DocumentName
            //    });
            //}

            var templateData = new TemplateData()
            {
                //Blocks = result.Blocks
                //    .Select(s => new Block(s.Title, s.Subtitle, s.Body, s.MinorTitle, s.Cta,
                //        _urlBuilder.BuildWalletEndPoint(code))),
                //Referral = new Referral(_urlBuilder.BuildShareEndPoint(code)),
                //Subject = result.Subject.InjectSingleValue("Tokens", data.Tokens.ToString("f2")),
                //To = data.ToEmailAddress,
            };
            await BuildEmail(data.ToEmailAddress, data.Language, binder, templateData, "DocumentPurchased", token);

        }
        public static async Task BuildEmail(string toAddress, Language language, IBinder binder,
            TemplateData templateData,
            string category,
            CancellationToken token)
        {
            var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            {
                ApiKey = "SendgridKey",
                From = "Spitball <no-reply @spitball.co>"
            }, token);


            var message = new SendGridMessage
            {
                Asm = new ASM { GroupId = 10926 },
                TemplateId = language == Language.English ? "d-91a839096c8547f9a028134744e78ecb" : "d-a9cd8623ad034007bb397f59477d81d2"
            };
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