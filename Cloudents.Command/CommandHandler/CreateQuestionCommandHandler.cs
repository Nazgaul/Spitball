using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    [UsedImplicitly]
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly Lazy<IBlobProvider<QuestionAnswerContainer>> _blobProvider;
        private readonly ITextAnalysis _textAnalysis;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICourseRepository _courseRepository;

        public CreateQuestionCommandHandler(IQuestionRepository questionRepository,
            IRegularUserRepository userRepository, ITextAnalysis textAnalysis,
            ITransactionRepository transactionRepository,
            Lazy<IBlobProvider<QuestionAnswerContainer>> blobProvider, ICourseRepository courseRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _textAnalysis = textAnalysis;
            _transactionRepository = transactionRepository;
            _blobProvider = blobProvider;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (await _questionRepository.GetSimilarQuestionAsync(message.Text, token))
            {
                throw new DuplicateRowException();
            }

            var currentBalance = await _transactionRepository.GetBalanceAsync(message.UserId, token);
           
            if (currentBalance < message.Price)
            {
                throw new InsufficientFundException();
            }

            var course = await _courseRepository.LoadAsync(message.Course, token);
            course.Count++;
            await _courseRepository.UpdateAsync(course, token);



            var textLanguage = await _textAnalysis.DetectLanguageAsync(message.Text, token);

            var question = new Question(message.SubjectId,
                message.Text, message.Price, message.Files?.Count() ?? 0,
                user, textLanguage, course, user.University);

            user.MakeTransaction(QuestionTransaction.Asked(question));
            await _userRepository.UpdateAsync(user, token);


            await _questionRepository.AddAsync(question, token);
            var id = question.Id;

            if (_blobProvider != null)
            {
                var l = message.Files?.Select(file => _blobProvider.Value.MoveAsync(file, $"{id}", token)) ??
                        Enumerable.Empty<Task>();
                await Task.WhenAll(l);
            }
        }
    }
}
