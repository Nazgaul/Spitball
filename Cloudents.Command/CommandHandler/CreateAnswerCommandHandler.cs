using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IRepository<User> _userRepository;



        public CreateAnswerCommandHandler(IRepository<Question> questionRepository,
            IAnswerRepository answerRepository, IRepository<User> userRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
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

            var user = await _userRepository.LoadAsync(message.UserId, token);
            Country country = user.Country;
            if (country == Country.India)
            {
                var pendingAnswers = await _answerRepository.GetNumberOfPendingAnswer(user.Id, token);
                var pendingAnswerAfterThisInsert = pendingAnswers + 1;
                if (pendingAnswerAfterThisInsert > 5)
                {
                    throw new QuotaExceededException();
                }
            }
            //var answers = question.Answers;
           
            //we can check if we can create sql query to check answer with regular expression
            //and we can create sql to check if its not the same user
            //var regex = new Regex(@"[,`~'<>?!@#$%^&*.;_=+()\s]", RegexOptions.Compiled);
            //var nakedString = Regex.Replace(message.Text, regex.ToString(), "");
            //foreach (var answer in answers.Where(w =>
            //    w.Status.State == ItemState.Ok

            //))
            //{
            //    var check = Regex.Replace(answer.Text, regex.ToString(), "");
            //    if (nakedString == check)
            //    {
            //        throw new DuplicateRowException("Duplicate answer");
            //    }

            //}

            var newAnswer = question.AddAnswer(message.Text, user);
            await _answerRepository.AddAsync(newAnswer, token);
        }
    }
}