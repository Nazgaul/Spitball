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
            var dto = new QuestionFeedDto
            {
                User = new UserDto
                {
                    Id = eventMessage.Question.User.Id,
                    Name = eventMessage.Question.User.Name,
                    Image = eventMessage.Question.User.Image
                },
                Answers = 0,
                Id = eventMessage.Question.Id,
                DateTime = eventMessage.Question.Updated,
                Files = eventMessage.Question.Attachments,
                HasCorrectAnswer = false,
                Price = eventMessage.Question.Price,
                Text = eventMessage.Question.Text,
                Color = eventMessage.Question.Color,
                Subject = eventMessage.Question.Subject
            };
            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Delete, dto), token);
        }
    }
}
