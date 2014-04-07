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
    class DeleteQuizCommandHandler : ICommandHandler<DeleteQuizCommand>
    {
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;


        public DeleteQuizCommandHandler(IRepository<Zbang.Zbox.Domain.Quiz> quizRepository)
        {
            m_QuizRepository = quizRepository;
        }
        public void Handle(DeleteQuizCommand message)
        {
            var quiz = m_QuizRepository.Load(message.QuizId);

            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }
           
            m_QuizRepository.Delete(quiz);
        }
    }
}
