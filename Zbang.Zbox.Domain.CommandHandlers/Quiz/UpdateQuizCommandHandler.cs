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
    class UpdateQuizCommandHandler : ICommandHandler<UpdateQuizCommand>
    {
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;


        public UpdateQuizCommandHandler(

            IRepository<Zbang.Zbox.Domain.Quiz> quizRepository)
        {
            m_QuizRepository = quizRepository;
        }

        public void Handle(UpdateQuizCommand message)
        {
            var quiz = m_QuizRepository.Load(message.QuizId);

            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }
            quiz.UpdateText(message.Text);
            m_QuizRepository.Save(quiz);

        }
    }
}
