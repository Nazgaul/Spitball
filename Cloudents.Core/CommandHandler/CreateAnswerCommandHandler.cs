using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;


        public CreateAnswerCommandHandler(IRepository<Question> questionRepository,
            IAnswerRepository answerRepository, IRepository<User> userRepository,
            IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var question = await _questionRepository.GetAsync(message.QuestionId, token).ConfigureAwait(false);
            var nakeadstring = Regex.Replace(message.Text, @"[^0-9a-zA-Z]", "");
            foreach (var t in question.Answers)
            {
                var check = Regex.Replace(t.Text, @"[^0-9a-zA-Z]", "");
                if (nakeadstring == check)
                {
                    throw new DuplicateRowException("Duplicate answer");
                }
            }
            if (question == null)
            {
                throw new ArgumentException("question doesn't exits");
            }
            if (question.CorrectAnswer != null)
            {
                throw new QuestionAlreadyAnsweredException();

            }
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);

            if (user.Id == question.User.Id)
            {
                throw new InvalidOperationException("user cannot answer himself");
            }

           
            if (user.Fictive.GetValueOrDefault())
            {
                throw new InvalidOperationException("fictive user");
            }

            if (!Language.ListOfWhiteListCountries.Contains(user.Country))
            {
                var pendingAnswers = await _answerRepository.GetNumberOfPendingAnswer(user.Id, token);
                var pendingAnswerAfterThisInsert = pendingAnswers + 1;
                if (pendingAnswerAfterThisInsert > 5)
                {
                    throw new QuotaExceededException();
                }
            }
            //if (!user.Country.Contains<string>(Language.ListOfWhiteListCountries, StringComparison.OrdinalIgnoreCase))
            //{
            //    //State = QuestionState.Ok;
            //}
            //doing that instead of repository because this will only go to db once to get the collection vs 2 separate api calls.
            //I can argue about that - but for now it'll work
            if (question.Answers?.Any(a => a.User.Id == user.Id) == true)
            {
                throw new InvalidOperationException("user cannot give more then one answer");
            }

            if (question.Answers?.Any(a => string.Equals(a.Text, message.Text, StringComparison.OrdinalIgnoreCase)) ==
                true)
            {
                throw new DuplicateRowException();
            }
            var answer = question.AddAnswer(message.Text, message.Files?.Count() ?? 0, user);
            await _answerRepository.AddAsync(answer, token).ConfigureAwait(false);
            var id = answer.Id;


            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"{question.Id}/answer/{id}", token)) ?? Enumerable.Empty<Task>();

            await Task.WhenAll(l/*.Union(new[] { t })*/).ConfigureAwait(true);
        }
    }
}