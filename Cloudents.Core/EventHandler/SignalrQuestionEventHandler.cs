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
                score, eventMessage.Question.User.Image);

            var dto = new QuestionFeedDto
            {
                CultureInfo = eventMessage.Question.Language,
                User = user,
                Price = eventMessage.Question.Price,
                Id = eventMessage.Question.Id,
                Course = eventMessage.Question.Course?.Id, //TODO: this is because we already have question without courses
                Subject = eventMessage.Question.Subject,
                Text = eventMessage.Question.Text,
                Answers = 0,
                DateTime = DateTime.UtcNow,
                Files = eventMessage.Question.Attachments,
                HasCorrectAnswer = false,
                Vote = new VoteDto()
            };

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

                eventMessage.Answer.User.Transactions.Score, eventMessage.Answer.User.Image);

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


        //public  Task HandleAsync(ChatMessageEvent eventMessage, CancellationToken token)
        //{
        //    //Need to support blob
        //    var messages = eventMessage.ChatMessage;
        //    var message = new SignalRTransportType(SignalRType.Chat,
        //        SignalRAction.Add, new { messages = messages });

        //   var result =  eventMessage.ChatMessage.ChatRoom.Users.Select(s =>
        //       _queueProvider.InsertMessageAsync(message, s.User.Id, token));
        //   return Task.WhenAll(result);
        //}
    }
}
