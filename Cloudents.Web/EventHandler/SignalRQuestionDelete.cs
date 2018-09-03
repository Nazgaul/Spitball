using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.EventHandler
{
    public class SignalRQuestionDelete : IEventHandler<QuestionDeletedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public SignalRQuestionDelete(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
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
                DateTime = eventMessage.Question.Updated,
                Files = eventMessage.Question.Attachments,
                HasCorrectAnswer = false,
                Price = eventMessage.Question.Price,
                Text = eventMessage.Question.Text,
                Color = eventMessage.Question.Color,
                Subject = eventMessage.Question.Subject.Text
            };
            await _hubContext.Clients.All.SendAsync(SbHub.MethodName, new SignalRTransportType<QuestionDto>("question", SignalRAction.Delete, dto), token);
        }
    }
}
