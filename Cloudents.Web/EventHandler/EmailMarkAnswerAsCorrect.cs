using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message.Email;

namespace Cloudents.Web.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailMarkAnswerAsCorrect : IEventHandler<MarkAsCorrectEvent>
    {
        public const string ProtectPurpose = "MarkAnswerAsCorrect";
        private readonly IQueueProvider _serviceBusProvider;
        private readonly IDataProtect _dataProtect;
        private readonly IUrlBuilder _urlBuilder;


        public EmailMarkAnswerAsCorrect(IQueueProvider serviceBusProvider, IDataProtect dataProtect, IUrlBuilder urlBuilder)
        {
            _serviceBusProvider = serviceBusProvider;
            _dataProtect = dataProtect;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var answer = eventMessage.Answer;

            var code = _dataProtect.Protect(ProtectPurpose, answer.User.Id.ToString(),
                DateTimeOffset.UtcNow.AddDays(5));
            var link = _urlBuilder.BuildWalletEndPoint(new { code });
            await _serviceBusProvider.InsertMessageAsync(
                new AnswerCorrectEmail(answer.User.Email, answer.Question.Text,
                    answer.Text, link,
                    answer.Question.Price, answer.User.Culture), token).ConfigureAwait(false);
        }
    }
}