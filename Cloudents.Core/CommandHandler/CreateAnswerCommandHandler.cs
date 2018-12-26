using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IEventStore _eventStore;


        public CreateAnswerCommandHandler(IRepository<Question> questionRepository,
            IAnswerRepository answerRepository, IRepository<RegularUser> userRepository,
            IBlobProvider<QuestionAnswerContainer> blobProvider, IEventStore eventStore)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var question = await _questionRepository.GetAsync(message.QuestionId, token).ConfigureAwait(false);
           
            if (question == null)
            {
                throw new ArgumentException("question doesn't exits");
            }

            if (question.Item.State != ItemState.Ok)
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

            if (user.Score < Privileges.Post)
            {
                var pendingAnswers = await _answerRepository.GetNumberOfPendingAnswer(user.Id, token);
                var pendingAnswerAfterThisInsert = pendingAnswers + 1;
                if (pendingAnswerAfterThisInsert > 5)
                {
                    throw new QuotaExceededException();
                }
            }
            var answers = question.Answers;
            if (answers.Any(a => a.User.Id == user.Id && a.Item.State != ItemState.Deleted))
            {
                throw new MoreThenOneAnswerException();
            }
            //TODO:
            //we can check if we can create sql query to check answer with regular expression
            //and we can create sql to check if its not the same user
            var regex = new Regex(@"[,`~'<>?!@#$%^&*.;_=+()\s]", RegexOptions.Compiled);
            var nakedString = Regex.Replace(message.Text, regex.ToString(), "");
            if (answers != null)
                foreach (var answer in answers.Where(w=> 
                    w.Item.State == ItemState.Ok

                    ))
                {
                    //if (answer.User.Id == user.Id)
                    //{
                    //    throw new MoreThenOneAnswerException();
                    //    //throw new InvalidOperationException("user cannot give more then one answer");
                    //}
                    var check = Regex.Replace(answer.Text, regex.ToString(), "");
                    if (nakedString == check)
                    {
                        throw new DuplicateRowException("Duplicate answer");
                    }
                }
            var newAnswer = question.AddAnswer(message.Text, message.Files?.Count() ?? 0, user);
            await _answerRepository.AddAsync(newAnswer, token).ConfigureAwait(false);
            
            var id = newAnswer.Id;

            if (newAnswer.Item.State == ItemState.Ok)
            {
                _eventStore.Add(new AnswerCreatedEvent(newAnswer));
                //question.AnswerCount++;
                await _questionRepository.UpdateAsync(question, token);
            }

            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"{question.Id}/answer/{id}", token)) ?? Enumerable.Empty<Task>();

            await Task.WhenAll(l/*.Union(new[] { t })*/).ConfigureAwait(true);
        }
    }
}