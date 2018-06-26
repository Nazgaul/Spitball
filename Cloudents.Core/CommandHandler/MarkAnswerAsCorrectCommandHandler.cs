﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class MarkAnswerAsCorrectCommandHandler : ICommandHandler<MarkAnswerAsCorrectCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly ITransactionRepository _transactionRepository;

        public MarkAnswerAsCorrectCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository, ITransactionRepository transactionRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _transactionRepository = transactionRepository;
            // _blockChain = blockChain;
            //_blockChainProvider = blockChainProvider;
        }

        public async Task ExecuteAsync(MarkAnswerAsCorrectCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token).ConfigureAwait(true); //false will raise an exception
            var question = answer.Question;
            if (question.User.Id != message.UserId)
            {
                throw new ApplicationException("only owner can perform this task");
            }
            if (answer.Question.Id != question.Id)
            {
                throw new ApplicationException("answer is not connected to question");
            }
            question.MarkAnswerAsCorrect(answer);


            var transactions = Transaction.QuestionMarkAsCorrect(question);

            await _transactionRepository.AddAsync(transactions, token);
            await _questionRepository.UpdateAsync(question, token).ConfigureAwait(false);

            // await _blockChainProvider.InsertMessageAsync(new BlockChainMarkQuestionAsCorrect(_blockChain.GetAddress(question.User.PrivateKey), _blockChain.GetAddress(answer.User.PrivateKey), question.Id, answer.Id), token).ConfigureAwait(true);
        }
    }
}