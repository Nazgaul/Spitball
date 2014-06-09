using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand>
    {
        //private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Answer> m_AnswerRepository;


        public CreateAnswerCommandHandler(
            IRepository<Question> questionRepository,
            IRepository<Answer> answerRepository)
        {
            m_QuestionRepository = questionRepository;
            m_AnswerRepository = answerRepository;
        }
        public void Handle(CreateAnswerCommand message)
        {
            var question = m_QuestionRepository.Load(message.QuestionId);
            if (question.Quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not quiz owner");
            }
            var answer = new Answer(message.Id,TextManipulation.EncodeText( message.Text), question);
            m_QuestionRepository.Save(question);
            m_AnswerRepository.Save(answer);
        }
    }
}
