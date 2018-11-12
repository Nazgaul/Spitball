﻿using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Question> _questionRepository;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository,
            IRepository<Transaction> transactionRepository, IRepository<Question> questionRepository)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(DeleteAnswerCommand message, CancellationToken token)
        {
            var answer = await _repository.GetAsync(message.Id, token).ConfigureAwait(false); //no point in load since next line will do query
            if (answer == null)
            {
                throw new ArgumentException("answer doesn't exits");
            }

            foreach (var transaction in answer.Transactions)
            {
               
                await _transactionRepository.DeleteAsync(transaction, token);
            }

            answer.Events.Add(new AnswerDeletedEvent(answer));

            answer.Question.CorrectAnswer = null;
            await _questionRepository.UpdateAsync(answer.Question, token);
            answer.Events.Add(new AnswerDeletedAdminEvent());
            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        }
    }
}
