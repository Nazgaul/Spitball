using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> _repository;
        private readonly IRepository<Transaction> _transactionRepository;

        public DeleteAnswerCommandHandler(IRepository<Answer> repository, IRepository<Transaction> transactionRepository)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
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

            foreach (var transaction in answer.Question.Transactions)
            {
                await _transactionRepository.DeleteAsync(transaction, token);
            }
            answer.Events.Add(new AnswerDeletedEvent(answer));
            var userId = answer.User.Id;
            List<long> users = new List<long> { userId, answer.Question.User.Id };
         
            answer.Events.Add(new AnswerDeletedAdminEvent(users));
            await _repository.DeleteAsync(answer, token).ConfigureAwait(false);
        }
    }
}
