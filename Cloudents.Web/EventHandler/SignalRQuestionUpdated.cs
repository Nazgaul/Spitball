using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.EventHandler
{
    public class SignalRQuestionUpdated : IEventHandler<MarkAsCorrectEvent>, IEventHandler<AnswerCreatedEvent>, IEventHandler<AnswerDeletedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public SignalRQuestionUpdated(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        private async Task Handle(Question question, CancellationToken token)
        {
            var dto = new QuestionDto
            {
                User = new UserDto
                {
                    Id = question.User.Id,
                    Name = question.User.Name,
                    Image = null
                },
                Answers = question.Answers.Count,
                Id = question.Id,
                DateTime = question.Updated,
                Files = question.Attachments,
                HasCorrectAnswer = question.CorrectAnswer != null,
                Price = question.Price,
                Text = question.Text,
                Color = question.Color,
                Subject = question.Subject.Text
            };
            await _hubContext.Clients.All.SendAsync(SbHub.MethodName, new SignalRTransportType<QuestionDto>("question", SignalRAction.Update, dto), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            return Handle(eventMessage.Answer.Question, token);
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            return Handle(eventMessage.Answer.Question, token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            return Handle(eventMessage.Answer.Question, token);
        }
    }
}
