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
        private readonly IRepository<Reputation> m_ReputationRepository;


        public DeleteAnswerCommandHandler(IRepository<Answer> answerRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository)
        {
            m_AnswerRepository = answerRepository;
            m_BoxRepository = boxRepository;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(DeleteAnswerCommand message)
        {
            var answer = m_AnswerRepository.Load(message.AnswerId);
            var box = answer.Box;

            bool isAuthorize = answer.User.Id != message.UserId || box.Owner.Id != message.UserId;
            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didnt ask the answer");
            }
           
            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) - 1);

            m_ReputationRepository.Save(answer.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteAnswer));
            m_BoxRepository.Save(box);
            m_AnswerRepository.Delete(answer);

        }
    }
}
