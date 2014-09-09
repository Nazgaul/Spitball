using System;
using System.Linq;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand>
    {
        private readonly IRepository<Comment> m_BoxCommentRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Updates> m_Updates;

        public DeleteCommentCommandHandler(
            IRepository<Comment> boxCommentRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository,
            IRepository<Updates> updates, IUserRepository userRepository)
        {
            m_BoxCommentRepository = boxCommentRepository;
            m_BoxRepository = boxRepository;
            m_Updates = updates;
            m_UserRepository = userRepository;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(DeleteCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var question = m_BoxCommentRepository.Load(message.QuestionId);
            var user = m_UserRepository.Load(message.UserId);
            var box = question.Box;

            const int reputationNeedToDeleteItem = 1000000;

            bool isAuthorize = question.User.Id == message.UserId || box.Owner.Id == message.UserId || user.Reputation > reputationNeedToDeleteItem;
            
            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didn't ask the question");
            }


            

            foreach (var item in question.AnswersReadOnly)
            {
                m_ReputationRepository.Save(item.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteAnswer));
            }

            var updatesToThatQuiz = m_Updates.GetQuerable().Where(w => w.Comment.Id == message.QuestionId);

            foreach (var quizUpdate in updatesToThatQuiz)
            {
                m_Updates.Delete(quizUpdate);
            }

            m_ReputationRepository.Save(question.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteQuestion));

            var substract = question.AnswersReadOnly.Count + 1;
            box.UpdateCommentsCount(m_BoxRepository.QnACount(box.Id) - substract);

            m_BoxRepository.Save(box);

            m_BoxCommentRepository.Delete(question);

        }
    }
}
