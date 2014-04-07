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
    class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> m_AnswerRepository;
        public DeleteAnswerCommandHandler(IRepository<Answer> answerRepository)
        {
            m_AnswerRepository = answerRepository;
        }
        public void Handle(DeleteAnswerCommand message)
        {
            var answer = m_AnswerRepository.Load(message.Id);
            if (answer.Quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not owner of quiz");
            }

            m_AnswerRepository.Delete(answer);
        }
    }
}
