using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class SignalRQuestionDelete : IEventHandler<QuestionDeletedEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public SignalRQuestionDelete(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }


        public async Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            var user = new UserDto
            {
                Id = eventMessage.Question.User.Id,
                Name = eventMessage.Question.User.Name,
                Image = eventMessage.Question.User.Image
            };
            var dto = new QuestionFeedDto(eventMessage.Question.Id,
                eventMessage.Question.Subject,
                eventMessage.Question.Price,
                eventMessage.Question.Text,
                eventMessage.Question.Attachments,
                0,
                user,
                eventMessage.Question.Updated,
                eventMessage.Question.Color,
                false,
                eventMessage.Question.Language);
           
            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Delete, dto), token);
        }
    }
}
