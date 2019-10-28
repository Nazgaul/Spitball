//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Message.Email;
//using Cloudents.Query;
//using Cloudents.Query.Email;
//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.Azure.WebJobs;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Extension;

//namespace Cloudents.FunctionsV2.System
//{
//    public class AnswerAcceptedEmailOperation : ISystemOperation<AnswerAcceptedMessage>
//    {
//        private readonly IQueryBus _queryBus;
//        private readonly IDataProtectionProvider _dataProtectProvider;
//        private readonly IUrlBuilder _urlBuilder;

//        public AnswerAcceptedEmailOperation(IQueryBus queryBus, IDataProtectionProvider dataProtectProvider, IUrlBuilder urlBuilder)
//        {
//            _queryBus = queryBus;
//            _dataProtectProvider = dataProtectProvider;
//            _urlBuilder = urlBuilder;
//        }

//        public async Task DoOperationAsync(AnswerAcceptedMessage msg, IBinder binder, CancellationToken token)
//        {
//            //await Task.Delay(TimeSpan.FromSeconds(30), token);
//            var query = new GetAnswerAcceptedEmailQuery(msg.QuestionId);
//            var data = await _queryBus.QueryAsync(query, token);


//            var template = await DocumentPurchasedEmailOperation.GetEmail("AnswerAccepted", data.Language, _queryBus, token);
//            var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
//                .ToTimeLimitedDataProtector();
//            var code = dataProtector.Protect(data.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));

//            foreach (var block in template.Blocks)
//            {
//                // block.Subtitle = block.Subtitle.InjectSingleValue("Tokens", data.Tokens.ToString("f2"));
//                block.Body = block.Body.Replace("\n", "<br>").Inject(data);
//                block.Body = block.Body.Inject(new
//                {
//                    data.QuestionText,
//                    data.AnswerText
//                });

//            }

//            var templateData = new TemplateData()
//            {
//                Blocks = template.Blocks
//                    .Select(s => new Block(s.Title, s.Subtitle, s.Body, s.MinorTitle, s.Cta,
//                        _urlBuilder.BuildQuestionEndPoint(data.QuestionId, new { token = code }))),
//                Referral = new Referral(_urlBuilder.BuildShareEndPoint(code)),
//                Subject = template.Subject,//.InjectSingleValue("Tokens", data.Tokens.ToString("f2")),
//                To = data.ToEmailAddress
//            };
//            await DocumentPurchasedEmailOperation.BuildEmail(data.ToEmailAddress, data.Language, binder, templateData, "AnswerCorrect", token);
//        }
//    }
//}