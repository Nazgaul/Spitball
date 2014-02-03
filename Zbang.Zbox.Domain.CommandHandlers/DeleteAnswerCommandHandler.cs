using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> m_AnswerRepository;
        private readonly IBoxRepository m_BoxRepository;


        public DeleteAnswerCommandHandler(IRepository<Answer> answerRepository, IBoxRepository boxRepository)
        {
            m_AnswerRepository = answerRepository;
            m_BoxRepository = boxRepository;
        }
        public void Handle(DeleteAnswerCommand message)
        {
            var answer = m_AnswerRepository.Load(message.AnswerId);
            if (answer.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User didnt ask the question");
            }
            var box = answer.Box;
            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) - 1);
            m_BoxRepository.Save(box);
            m_AnswerRepository.Delete(answer);

        }
    }
}
