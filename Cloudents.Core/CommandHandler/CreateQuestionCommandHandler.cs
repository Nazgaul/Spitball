﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<QuestionSubject> _questionSubjectRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
       // private readonly ITransactionRepository _transactionRepository;


        public CreateQuestionCommandHandler(IRepository<Question> questionRepository, 
            IRepository<QuestionSubject> questionSubjectRepository, IRepository<User> userRepository,
            IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _questionRepository = questionRepository;
            _questionSubjectRepository = questionSubjectRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            //_transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            //if you get an exception doing debug make sure the locals window is minimized.
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(true);
            var subject = await _questionSubjectRepository.LoadAsync(message.SubjectId,token).ConfigureAwait(true);
            var question = new Question(subject, message.Text, message.Price, message.Files?.Count() ?? 0, user);
            await _questionRepository.AddAsync(question, token).ConfigureAwait(true);
            var id = question.Id;
            //var p = _blockChainProvider.InsertMessageAsync(new BlockChainSubmitQuestion(id, message.Price, _blockChain.GetAddress(user.PrivateKey)), token);


            //var t = Transaction.QuestionCreateTransaction(question);

            //await _transactionRepository.AddAsync(t, token);

            //TODO: not right
            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{id}", token)) ?? Enumerable.Empty<Task>();
            await Task.WhenAll(l/*.Union(new[] { p })*/).ConfigureAwait(true);
        }
    }
}
