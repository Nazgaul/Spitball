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
    class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Question> m_QuestionRepository;

        public CreateQuestionCommandHandler(
            IRepository<Zbang.Zbox.Domain.Quiz> quizRepository,
            IRepository<Question> questionRepository)
        {
            m_QuizRepository = quizRepository;
            m_QuestionRepository = questionRepository;
        }
        public void Handle(CreateQuestionCommand message)
        {
            var quiz = m_QuizRepository.Load(message.QuizId);
            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not quiz owner");
            }
            var question = new Question(message.QuestionId, quiz, TextManipulation.EncodeText(message.Text));

            m_QuestionRepository.Save(question);
        }
    }
}
