using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class UpdateAnswerCommandHandler : ICommandHandler<UpdateAnswerCommand>
    {
        private readonly IRepository<Answer> m_AnswerRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        public UpdateAnswerCommandHandler(IRepository<Answer> answerRepository, IRepository<Question> questionRepository)
        {
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
        }
        public void Handle(UpdateAnswerCommand message)
        {
            var answer = m_AnswerRepository.Load(message.Id);

            if (answer.Quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not owner of quiz");
            }

            answer.UpdateText(message.Text);
            if (message.IsCorrect)
            {
                answer.UpdateCorrectAnswer();
                m_QuestionRepository.Save(answer.Question);

            }
            m_AnswerRepository.Save(answer);
        }
    }
}
