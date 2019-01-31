﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    [AdminCommandHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRegularUserRepository _userRepository;



        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository, IRegularUserRepository userRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);
            if (!(question.User.Actual is RegularUser t))
            {
                return;
            }
            question.DeleteQuestionAdmin();
            if (question.CorrectAnswer == null)
            { 
                t.MakeTransaction(QuestionTransaction.Deleted(question));
                //t.MakeTransaction(TransactionType2.UnStakeMoney(question.Price,
                //    TransactionActionType.DeleteQuestion));
                await _userRepository.UpdateAsync(t, token);
            }
            
            await _questionRepository.DeleteAsync(question, token);
        }
    }
}