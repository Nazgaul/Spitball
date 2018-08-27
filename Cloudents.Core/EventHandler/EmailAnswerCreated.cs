using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class EmailAnswerCreated : IEventHandler<AnswerCreatedEvent>
    {
        public const string CreateAnswer = "CreateAnswer";
        private readonly IServiceBusProvider _serviceBusProvider;
        private readonly IDataProtect _dataProtect;
        private readonly IUrlBuilder _urlBuilder;


        public EmailAnswerCreated(IServiceBusProvider serviceBusProvider, IDataProtect dataProtect,
             IUrlBuilder urlBuilder)
        {
            _serviceBusProvider = serviceBusProvider;
            _dataProtect = dataProtect;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            //var question = await _questionRepository.LoadAsync(eventMessage.QuestionId, token);
            //var answer = await _answerRepository.LoadAsync(eventMessage.AnswerId, token);
            var question = eventMessage.Answer.Question;
            var code = _dataProtect.Protect(CreateAnswer, question.User.Id.ToString(),
                DateTimeOffset.UtcNow.AddDays(2));
            var link = _urlBuilder.BuildQuestionEndPoint(question.Id, new { code });
            await _serviceBusProvider.InsertMessageAsync(
                   new GotAnswerEmail(question.Text, question.User.Email, eventMessage.Answer.Text, link), token);
        }
    }
}