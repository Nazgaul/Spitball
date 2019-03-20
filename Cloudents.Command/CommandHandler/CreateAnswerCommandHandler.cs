using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Command.CommandHandler
{
    [UsedImplicitly]
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly ITextAnalysis _textAnalysis;

        private readonly IQuestionsDirectoryBlobProvider _blobProvider;


        public CreateAnswerCommandHandler(IRepository<Question> questionRepository,
            IAnswerRepository answerRepository, IRepository<RegularUser> userRepository,
            IQuestionsDirectoryBlobProvider blobProvider, ITextAnalysis textAnalysis)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _textAnalysis = textAnalysis;
        }

        public async Task ExecuteAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var question = await _questionRepository.GetAsync(message.QuestionId, token);
           
            if (question == null)
            {
                throw new ArgumentException("question doesn't exits");
            }

            if (question.Status != ItemStatus.Public)
            {
                throw new ArgumentException("question doesn't exits");
            }
            if (question.CorrectAnswer != null)
            {
                throw new QuestionAlreadyAnsweredException();

            }
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (user.Id == question.User.Id)
            {
                throw new InvalidOperationException("user cannot answer himself");
            }

            if (user.Transactions.Score < Privileges.Post)
            {
                var pendingAnswers = await _answerRepository.GetNumberOfPendingAnswer(user.Id, token);
                var pendingAnswerAfterThisInsert = pendingAnswers + 1;
                if (pendingAnswerAfterThisInsert > 5)
                {
                    throw new QuotaExceededException();
                }
            }
            var answers = question.Answers;
            if (answers.Any(a => a.User.Id == user.Id && a.Status.State != ItemState.Deleted))
            {
                throw new MoreThenOneAnswerException();
            }
            //TODO:
            //we can check if we can create sql query to check answer with regular expression
            //and we can create sql to check if its not the same user
            var regex = new Regex(@"[,`~'<>?!@#$%^&*.;_=+()\s]", RegexOptions.Compiled);
            var nakedString = Regex.Replace(message.Text, regex.ToString(), "");
            foreach (var answer in answers.Where(w =>
                w.Status.State == ItemState.Ok

            ))
            {
                var check = Regex.Replace(answer.Text, regex.ToString(), "");
                if (nakedString == check)
                {
                    throw new DuplicateRowException("Duplicate answer");
                }

            }

            var language = await _textAnalysis.DetectLanguageAsync(message.Text, token);
            var newAnswer = question.AddAnswer(message.Text, message.Files?.Count() ?? 0, user, language);
            await _answerRepository.AddAsync(newAnswer, token);
            
            var id = newAnswer.Id;
          

            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"{question.Id}/answer/{id}", token)) ?? Enumerable.Empty<Task>();

            await Task.WhenAll(l);
        }
    }
}