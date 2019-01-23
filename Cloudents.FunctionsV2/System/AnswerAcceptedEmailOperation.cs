using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class AnswerAcceptedEmailOperation : ISystemOperation<AnswerAcceptedMessage>
    {
        private readonly IQueryBus _queryBus;
        private readonly IDataProtectionProvider _dataProtectProvider;
        private readonly IUrlBuilder _urlBuilder;

        public AnswerAcceptedEmailOperation(IQueryBus queryBus, IDataProtectionProvider dataProtectProvider, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _dataProtectProvider = dataProtectProvider;
            _urlBuilder = urlBuilder;
        }

        public async Task DoOperationAsync(AnswerAcceptedMessage msg, IBinder binder, CancellationToken token)
        {
            var query = new GetAnswerAcceptedEmailQuery(msg.TransactionId);
            var result = await _queryBus.QueryAsync(query, token);

            var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(result.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));

            foreach (var block in result.Blocks)
            {
                block.Subtitle = block.Subtitle.InjectSingleValue("Tokens", result.Tokens.ToString("f2"));
                block.Body = block.Body.Inject(result);
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
            await DocumentPurchasedEmailOperation.BuildEmail(result, binder, templateData, "AnswerCorrect", token);
        }
    }
}