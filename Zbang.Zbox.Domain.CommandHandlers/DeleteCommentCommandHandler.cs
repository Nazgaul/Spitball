using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand>
    {
        private readonly IRepository<Comment> m_BoxCommentRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteCommentCommandHandler(
            IRepository<Comment> boxCommentRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository,
            IUserRepository userRepository, IQueueProvider queueProvider)
        {
            m_BoxCommentRepository = boxCommentRepository;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(DeleteCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var comment = m_BoxCommentRepository.Load(message.CommentId);
            var user = m_UserRepository.Load(message.UserId);
            var box = comment.Box;


            bool isAuthorize = comment.User.Id == message.UserId
                || box.Owner.Id == message.UserId
                || user.Reputation > user.University.AdminScore;

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didn't ask the question");
            }
            var userIds = box.DeleteComment(comment);
            m_QueueProvider.InsertMessageToTranaction(new ReputationData(userIds.Union(new[] { user.Id })));
            m_BoxRepository.Save(box);
        }
    }
}
