﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    [AdminCommandHandler]
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
            var answer = await _repository.GetAsync(message.Id, token); //no point in load since next line will do query
            if (answer == null)
            {
                throw new ArgumentException("answer doesn't exits");
            }

            if (answer.State == ItemState.Deleted)
            {
                throw new ArgumentException("answer doesn't exits");
            }

            answer.DeleteAnswerAdmin();
            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
            //await DeleteAnswerAsync(answer, token);
        }

        //internal async Task DeleteAnswerAsync(Answer answer, CancellationToken token)
        //{
        //        foreach (var transaction in answer.TransactionsReadOnly)
        //        {
        //            await _transactionRepository.DeleteAsync(transaction, token);
        //        }
            
            
        //    _eventStore.Add(new AnswerDeletedEvent(answer));

        //   // answer.Question.AnswerCount--;
        //    if (answer.Question.CorrectAnswer != null)
        //    {
        //        if (answer.Id == answer.Question.CorrectAnswer.Id)
        //        {
        //            answer.Question.CorrectAnswer = null;
        //        }
        //    }
        //    await _questionRepository.UpdateAsync(answer.Question, token);
        //    await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        //}
    }
}