using Cloudents.Web.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.EventHandler;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Web.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailMarkAnswerAsCorrect : EmailEventHandler, IEventHandler<MarkAsCorrectEvent>
    {
        public const string ProtectPurpose = "MarkAnswerAsCorrect";
        
        private readonly IDataProtect _dataProtect;
        private readonly IUrlBuilder _urlBuilder;


        public EmailMarkAnswerAsCorrect(IQueueProvider serviceBusProvider, IDataProtect dataProtect, IUrlBuilder urlBuilder)
            :base(serviceBusProvider)
        {
            
            _dataProtect = dataProtect;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var answer = eventMessage.Answer;

            //if (!(answer.User is RegularUser u))
            //{
            //    return;
            //}


            var code = _dataProtect.Protect(ProtectPurpose, answer.User.Id.ToString(),
                DateTimeOffset.UtcNow.AddDays(5));
            var link = _urlBuilder.BuildWalletEndPoint(new { code });
            await SendEmail(
                new AnswerCorrectEmail(answer.User.Email, answer.Question.Text,
                    answer.Text, link,
                    answer.Question.Price, answer.User.Language), token).ConfigureAwait(false);
        }
    }
}