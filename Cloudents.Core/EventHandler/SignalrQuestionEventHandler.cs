﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.EventHandler
{
    public class SignalrQuestionEventHandler
        : IEventHandler<QuestionCreatedEvent>,
            IEventHandler<QuestionDeletedEvent>,
            IEventHandler<QuestionDeletedAdminEvent>,
            IEventHandler<MarkAsCorrectEvent>,
            IEventHandler<AnswerCreatedEvent>, IEventHandler<AnswerDeletedEvent>,
            IEventHandler<TransactionEvent>
    {
        private readonly IServiceBusProvider _queueProvider;

        public SignalrQuestionEventHandler(IServiceBusProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }


        public async Task HandleAsync(QuestionCreatedEvent eventMessage, CancellationToken token)
        {
            var score = 0;
            if (eventMessage.Question.User.Actual is RegularUser p)
            {
                score = p.Transactions.Score;
            }
            var user = new UserDto(eventMessage.Question.User.Id, eventMessage.Question.User.Name,
                score);

            var dto = new QuestionFeedDto(eventMessage.Question.Id,
                eventMessage.Question.Subject,
                eventMessage.Question.Price,
                eventMessage.Question.Text,
                eventMessage.Question.Attachments,
                0,
                user,
                DateTime.UtcNow,
                false,
                eventMessage.Question.Language,
                0, eventMessage.Question.Course.Name);
            if (eventMessage.Question.Language.Name.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                await _queueProvider.InsertMessageAsync(
                    new SignalRTransportType(SignalRType.Question, SignalRAction.Add, dto), token);
            }
            else
            {
                await _queueProvider.InsertMessageAsync(
                    new SignalRTransportType(SignalRType.Question, SignalRAction.Add, dto), $"country_{eventMessage.Question.User.Country.ToLowerInvariant()}", token);
            }
        }


        public async Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            var dto = new
            {
                id = eventMessage.Question.Id
            };

            await _queueProvider.InsertMessageAsync(
                new SignalRTransportType(SignalRType.Question, SignalRAction.Delete, dto), token);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            var message = new SignalRTransportType(SignalRType.Question, SignalREventAction.MarkAsCorrect,
                new
                {
                    questionId = eventMessage.Answer.Question.Id,
                    answerId = eventMessage.Answer.Id
                });


            return _queueProvider.InsertMessageAsync(message, token);
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
           
            var user = new UserDto(eventMessage.Answer.User.Id, eventMessage.Answer.User.Name,
                eventMessage.Answer.User.Transactions.Score);

            var answerDto = new QuestionDetailAnswerDto
            (
                eventMessage.Answer.Id,
                eventMessage.Answer.Text,
                user,
                eventMessage.Answer.Created,

                new VoteDto
                {
                    Votes = 0
                },
                eventMessage.Answer.Language
            );
            var dto = new
            {
                QuestionId = eventMessage.Answer.Question.Id,
                Answer = answerDto
            };

            return _queueProvider.InsertMessageAsync(new SignalRTransportType(SignalRType.Answer, SignalRAction.Add, dto), token);
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            var dto = new
            {
                questionId = eventMessage.Answer.Question.Id,
                answer = new { id = eventMessage.Answer.Id }
            };

            return _queueProvider.InsertMessageAsync(new SignalRTransportType(SignalRType.Answer, SignalRAction.Delete, dto), token);
        }

        public Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            var message = new SignalRTransportType(SignalRType.User,
                SignalRAction.Update, new { balance = eventMessage.User.Transactions.Balance });

            return _queueProvider.InsertMessageAsync
                (message, eventMessage.User.Id, token);
        }

        public Task HandleAsync(QuestionDeletedAdminEvent eventMessage, CancellationToken token)
        {
            var dto = new
            {
                id = eventMessage.Question.Id
            };

            return _queueProvider.InsertMessageAsync(
                new SignalRTransportType(SignalRType.Question, SignalRAction.Delete, dto), token);
        }


    }
}
