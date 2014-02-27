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
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IBoxRepository m_BoxRepository;


        public DeleteQuestionCommandHandler(
            IRepository<Question> questionRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository)
        {
            m_QuestionRepository = questionRepository;
            m_BoxRepository = boxRepository;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(DeleteQuestionCommand message)
        {
            var question = m_QuestionRepository.Load(message.QuestionId);
            var box = question.Box;


            bool isAuthorize = question.User.Id != message.UserId || box.Owner.Id != message.UserId;
            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didnt ask the question");
            }


            var substract = question.AnswersReadOnly.Count + 1;

            foreach (var item in question.AnswersReadOnly)
            {
                m_ReputationRepository.Save(item.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteAnswer));
            }

            m_ReputationRepository.Save(question.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteQuestion));
            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) - substract);

            m_BoxRepository.Save(box);

            m_QuestionRepository.Delete(question);

        }
    }
}
