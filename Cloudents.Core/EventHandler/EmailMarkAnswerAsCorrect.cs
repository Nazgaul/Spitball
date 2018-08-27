using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class EmailMarkAnswerAsCorrect : IEventHandler<MarkAsCorrectEvent>
    {
        public const string ProtectPurpose = "MarkAnswerAsCorrect";
        private readonly IServiceBusProvider _serviceBusProvider;
        private readonly IDataProtect _dataProtect;
        private readonly IUrlBuilder _urlBuilder;


        public EmailMarkAnswerAsCorrect(IServiceBusProvider serviceBusProvider, IDataProtect dataProtect,IUrlBuilder urlBuilder)
        {
            _serviceBusProvider = serviceBusProvider;
            _dataProtect = dataProtect;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var answer = eventMessage.Answer;

            var code = _dataProtect.Protect(ProtectPurpose, answer.User.Id.ToString(),
                DateTimeOffset.UtcNow.AddDays(2));
            var link = _urlBuilder.BuildWalletEndPoint(new { code });
            await _serviceBusProvider.InsertMessageAsync(
                new AnswerCorrectEmail(answer.User.Email, answer.Question.Text, answer.Text, link, answer.Question.Price), token).ConfigureAwait(false);
        }
    }
}