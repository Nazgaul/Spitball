using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class SignalRQuestionCreated : IEventHandler<QuestionCreatedEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public SignalRQuestionCreated(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }


        public async Task HandleAsync(QuestionCreatedEvent eventMessage, CancellationToken token)
        {
            var dto = new QuestionDto
            {
                User = new UserDto
                {
                    Id = eventMessage.Question.User.Id,
                    Name = eventMessage.Question.User.Name,
                    Image = eventMessage.Question.User.Image
                },
                Answers = 0,
                Id = eventMessage.Question.Id,
                DateTime = DateTime.UtcNow,
                Files = eventMessage.Question.Attachments,
                HasCorrectAnswer = false,
                Price = eventMessage.Question.Price,
                Text = eventMessage.Question.Text,
                Color = eventMessage.Question.Color,
                Subject = eventMessage.Question.Subject
            };
            await _queueProvider.InsertMessageAsync(new SignalRMessage(SignalRType.Question, SignalRAction.Add, dto), token);
        }
    }
}
