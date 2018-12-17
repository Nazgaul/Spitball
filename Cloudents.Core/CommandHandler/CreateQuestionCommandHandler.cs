﻿using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Common.Enum;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly Lazy< IBlobProvider<QuestionAnswerContainer>> _blobProvider;
        private readonly ITextAnalysis _textAnalysis;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEventStore _eventStore;

        public CreateQuestionCommandHandler(IQuestionRepository questionRepository,
            IRegularUserRepository userRepository, ITextAnalysis textAnalysis,
            ITransactionRepository transactionRepository, IEventStore eventStore,
            Lazy<IBlobProvider<QuestionAnswerContainer>> blobProvider)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _textAnalysis = textAnalysis;
            _transactionRepository = transactionRepository;
            _eventStore = eventStore;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(true);
            var oldQuestion = await _questionRepository.GetUserLastQuestionAsync(user.Id, token);

            //if (oldQuestion?.Created.AddSeconds(20) > DateTime.UtcNow)
            //{
            //    throw new QuotaExceededException("You need to wait before asking more questions");
            //}

            if (await _questionRepository.GetSimilarQuestionAsync(message.Text, token))
            {
                //TODO: this will not work
                //user.Events.Add(new QuestionRejectEvent(user));
                throw new DuplicateRowException();
            }

            var currentBalance = await _transactionRepository.GetBalanceAsync(message.UserId, token);
            var amountForAskingQuestion = currentBalance * 3 / 10;
            if (amountForAskingQuestion < message.Price)
            {
                throw new InsufficientFundException();
            }

            var textLanguage = await _textAnalysis.DetectLanguageAsync(message.Text, token);

            var question = new Question(message.SubjectId,
                message.Text, message.Price, message.Files?.Count() ?? 0, user, message.Color, textLanguage);


            var transaction = new Transaction(TransactionActionType.Question, TransactionType.Stake, -question.Price,user)
            {
                Question = question
            };

            await _transactionRepository.AddAsync(transaction, token);
            await _questionRepository.AddAsync(question, token);
            var id = question.Id;

            if (_blobProvider != null)
            {
                var l = message.Files?.Select(file => _blobProvider.Value.MoveAsync(file, $"{id}", token)) ??
                        Enumerable.Empty<Task>();
                await Task.WhenAll(l).ConfigureAwait(true);
            }
            
            if (question.Item.State == ItemState.Ok)
            {
                _eventStore.Add(new QuestionCreatedEvent(question));
            }
        }
    }
}
