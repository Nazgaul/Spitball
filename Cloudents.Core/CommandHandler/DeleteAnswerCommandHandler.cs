﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(DeleteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.GetAsync(message.Id, token).ConfigureAwait(false); //no point in load since next line will do query
            if (answer == null)
            {
                throw new ArgumentException("answer doesn't exits");
            }

            if (answer.State.State != ItemState.Ok)
            {
                throw new ArgumentException("answer doesn't exits");

            }



            if (answer.User.Id != message.UserId)
            {
                throw new InvalidOperationException("user is not the one who wrote the answer");
            }
            if (answer.Question.CorrectAnswer?.Id == message.Id)
            {
                throw new ArgumentException("this is answer is correct answer");
            }

            answer.Events.Add(new AnswerDeletedEvent(answer));
            
            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        }
    }
}