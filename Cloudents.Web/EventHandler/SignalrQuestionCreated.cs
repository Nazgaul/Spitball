using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Cloudents.Web.Models;

namespace Cloudents.Web.Services
{
    public class SignalrQuestionCreated : IEventHandler<QuestionCreatedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public SignalrQuestionCreated(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(QuestionCreatedEvent eventMessage, CancellationToken token)
        {
            QuestionDto dto = new QuestionDto()
            {
                User = new UserDto()
                {
                    Id = eventMessage.Question.User.Id,
                    Name = eventMessage.Question.User.Name,
                    Image = null
                },
                Answers = 0,
                Id = eventMessage.Question.Id,
                DateTime = DateTime.UtcNow,
                Files = eventMessage.Question.Attachments,
                HasCorrectAnswer = false,
                Price = eventMessage.Question.Price,
                Text = eventMessage.Question.Text,
                Color = eventMessage.Question.Color,
                Subject = eventMessage.Question.Subject.Text
            };
            await _hubContext.Clients.All.SendAsync(SbHub.MethodName, new SignalRTransportType<QuestionDto>("question", SignalRAction.Add, dto), token);
        }
    }
}
