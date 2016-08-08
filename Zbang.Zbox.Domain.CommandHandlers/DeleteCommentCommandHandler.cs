using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteCommentCommandHandler : ICommandHandlerAsync<DeleteCommentCommand>
    {
        private readonly IRepository<Comment> m_BoxCommentRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUpdatesRepository m_UpdatesRepository;

        public DeleteCommentCommandHandler(
            IRepository<Comment> boxCommentRepository,
            IBoxRepository boxRepository,
            IUserRepository userRepository, IQueueProvider queueProvider, IUpdatesRepository updatesRepository)
        {
            m_BoxCommentRepository = boxCommentRepository;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_UpdatesRepository = updatesRepository;
        }
        public Task HandleAsync(DeleteCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var comment = m_BoxCommentRepository.Load(message.CommentId);
            var user = m_UserRepository.Load(message.UserId);
            var box = comment.Box;


            bool isAuthorize = comment.User.Id == message.UserId
                || box.Owner.Id == message.UserId
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException();
            }
            m_UpdatesRepository.DeleteCommentUpdates(comment.Id);
            var userIds = box.DeleteComment(comment);

            m_BoxCommentRepository.Delete(comment);
            m_BoxRepository.Save(box);
            return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(userIds.Union(new[] { user.Id })));
        }
    }
}
