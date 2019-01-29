using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
            var z = await binder.BindAsync<IEnumerable<EmailObject>>(new CosmosDBAttribute("Spitball", "Emails")
            {
                ConnectionStringSetting = "Cosmos",
                SqlQuery = "SELECT * FROM c where c.event = 'DocumentPurchased'"
            });
            var query = new GetAnswerAcceptedEmailQuery(msg.TransactionId);
            var data = await _queryBus.QueryAsync(query, token);

            var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            var code = dataProtector.Protect(data.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));

            //foreach (var block in result.Blocks)
            //{
            //    block.Subtitle = block.Subtitle.InjectSingleValue("Tokens", data.Tokens.ToString("f2"));
            //    block.Body = block.Body.Replace("\n","<br>").Inject(data);
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
            await DocumentPurchasedEmailOperation.BuildEmail(data.ToEmailAddress, data.Language, binder, templateData, "AnswerCorrect", token);
        }
    }
}