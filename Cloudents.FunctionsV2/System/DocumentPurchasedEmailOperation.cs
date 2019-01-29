using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Email;
using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.System
{
    
    public class DocumentPurchasedEmailOperation :  ISystemOperation<DocumentPurchasedMessage>
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
            var result = await _queryBus.QueryAsync(query, token);

            var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(result.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));

            foreach (var block in result.Blocks)
            {
                block.Subtitle = block.Subtitle.InjectSingleValue("Tokens", result.Tokens.ToString("f2"));
                block.Body = block.Body.Inject(new
                {
                    result.CourseName,
                    result.DocumentName
                });
            }

            var templateData = new TemplateData()
            {
                Blocks = result.Blocks
                    .Select(s => new Block(s.Title, s.Subtitle, s.Body, s.MinorTitle, s.Cta,
                        _urlBuilder.BuildWalletEndPoint(code))),
                Referral = new Referral(_urlBuilder.BuildShareEndPoint(code)),
                Subject = result.Subject.InjectSingleValue("Tokens", result.Tokens.ToString("f2")),
                To = result.ToEmailAddress,
            };
            await BuildEmail(result, binder, templateData, "DocumentPurchased", token);
            //var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            //{
            //    ApiKey = "SendgridKey",
            //    From = "Spitball <no-reply @spitball.co>"
            //}, token);


            //var message = new SendGridMessage
            //{
            //    Asm = new ASM { GroupId = 10926 },
            //    TemplateId = result.Language == Language.English ? "d-91a839096c8547f9a028134744e78ecb" : "d-a9cd8623ad034007bb397f59477d81d2"
            //};
            //var personalization = new Personalization
            //{
            //    TemplateData = new TemplateData()
            //    {
            //        Blocks = result.Blocks
            //            .Select(s => new Block(s.Title, s.Subtitle, s.Body, s.MinorTitle, s.Cta,
            //                _urlBuilder.BuildWalletEndPoint(code))),
            //        Referral = new Referral(_urlBuilder.BuildShareEndPoint(code)),
            //        Subject = result.Subject.InjectSingleValue("Tokens", result.Tokens.ToString("f2")),
            //        To = result.ToEmailAddress,
            //    }
            //};


            //message.Personalizations = new List<Personalization>()
            //{
            //    personalization
            //};
            ////message.Subject = result.Subject.InjectSingleValue("Tokens",result.Tokens);
            //message.AddCategory("DocumentPurchased");
            //message.TrackingSettings = new TrackingSettings
            //{
            //    Ganalytics = new Ganalytics
            //    {
            //        UtmCampaign = "DocumentPurchased",
            //        UtmSource = "SendGrid",
            //        UtmMedium = "Email",
            //        Enable = true
            //    }
            //};
            //message.AddTo(result.ToEmailAddress);
            //await emailProvider.AddAsync(message, token);

        }
        public static async Task BuildEmail(EmailDto result, IBinder binder,
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
                TemplateId = result.Language == Language.English ? "d-91a839096c8547f9a028134744e78ecb" : "d-a9cd8623ad034007bb397f59477d81d2"
            };
            templateData.To = result.ToEmailAddress;
            var personalization = new Personalization
            {
                TemplateData = templateData
                //TemplateData = new TemplateData()
                //{
                //    Blocks = result.Blocks
                //        .Select(s => new Block(s.Title, s.Subtitle, s.Body, s.MinorTitle, s.Cta,
                //            _urlBuilder.BuildWalletEndPoint(code))),
                //    Referral = new Referral(_urlBuilder.BuildShareEndPoint(code)),
                //    Subject = result.Subject.InjectSingleValue("Tokens", result.Tokens.ToString("f2")),
                //    To = result.ToEmailAddress,
                //    //Direction = ((CultureInfo)result.Language).TextInfo.IsRightToLeft ? "rtl" : "ltr"
                //}
            };


            message.Personalizations = new List<Personalization>()
            {
                personalization
            };
            //message.Subject = result.Subject.InjectSingleValue("Tokens",result.Tokens);
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
            message.AddTo(result.ToEmailAddress);
            await emailProvider.AddAsync(message, token);
        }



    }
   
}