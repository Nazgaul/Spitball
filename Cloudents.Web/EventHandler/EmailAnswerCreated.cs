using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.EventHandler;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Web.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;

namespace Cloudents.Web.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailAnswerCreated : EmailEventHandler, IEventHandler<AnswerCreatedEvent>
    {

        private readonly IDataProtect _dataProtect;
        private readonly IUrlBuilder _urlBuilder;


        public EmailAnswerCreated(IQueueProvider serviceBusProvider,
             IDataProtect dataProtect,
             IUrlBuilder urlBuilder)
                : base(serviceBusProvider)
        {
            _dataProtect = dataProtect;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            if (_dataProtect == null)
            {
                //This can happen if we not in the scope of the website
                return;
            }

            if (!(eventMessage.Answer.Question.User.Actual is User _))
            {
                return;
            }
            var question = eventMessage.Answer.Question;
            var code = _dataProtect.Protect(question.User.Id.ToString(),
                DateTimeOffset.UtcNow.AddDays(2));

            var culture = question.User.Language.ChangeCultureBaseOnCountry(question.User.Country);
            var link = _urlBuilder.BuildQuestionEndPoint(question.Id, new { token = code });
            await SendEmail(
                   new GotAnswerEmail(question.Text, question.User.Email, eventMessage.Answer.Text, link,
                       culture)

                   , token);
        }
    }
}