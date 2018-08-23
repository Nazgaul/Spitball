using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IEventPublisher _eventPublisher;


        public CreateAnswerCommandHandler(IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository, IRepository<User> userRepository,
            IBlobProvider<QuestionAnswerContainer> blobProvider, IEventPublisher eventPublisher)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _eventPublisher = eventPublisher;
        }

        public async Task ExecuteAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var question = await _questionRepository.GetAsync(message.QuestionId, token).ConfigureAwait(false);
            if (question == null)
            {
                throw new ArgumentException("question doesn't exits");
            }
            if (user.Id == question.User.Id)
            {
                throw new InvalidOperationException("user cannot answer himself");
            }

            if (question.CorrectAnswer != null)
            {
                throw new InvalidOperationException("already answer with question");

            }
            if (user.Fictive)
            {
                throw new InvalidOperationException("fictive user");
            }

            if (question.Answers?.Any(a => a.User.Id == user.Id) == true)
            {
                throw new InvalidOperationException("user cannot give more the one answer");
            }
            var answer = new Answer(question, message.Text, message.Files?.Count() ?? 0, user);
            await _answerRepository.AddAsync(answer, token).ConfigureAwait(false);

            var id = answer.Id;

            if (answer.Created.Subtract(question.Created).Minutes < 5)
            {
                user.FraudScore++;
                if (answer.Created.Subtract(question.Created).Minutes < 2)
                    user.FraudScore++;
                await _userRepository.UpdateAsync(user, default);
            }
            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{question.Id}/answer/{id}", token)) ?? Enumerable.Empty<Task>();
           // var t = _eventPublisher.PublishAsync(new AnswerCreatedEvent(question.Id, id), token);


            //var t = _serviceBusProvider.InsertMessageAsync(
            //        new GotAnswerEmail(question.Text, question.User.Email, message.Text, message.QuestionLink), token);

            await Task.WhenAll(l/*.Union(new[] { t })*/).ConfigureAwait(true);
        }
    }
}