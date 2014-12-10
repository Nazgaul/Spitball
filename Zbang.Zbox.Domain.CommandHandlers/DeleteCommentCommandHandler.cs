using System;
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

        public DeleteCommentCommandHandler(
            IRepository<Comment> boxCommentRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository,
            IUserRepository userRepository)
        {
            m_BoxCommentRepository = boxCommentRepository;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
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

            m_ReputationRepository.Save(box.DeleteComment(comment));
            m_BoxRepository.Save(box);
        }
    }
}
