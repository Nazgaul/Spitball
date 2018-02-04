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
        private readonly IUserRepository _userRepository;
        private readonly IBoxRepository _boxRepository;
        private readonly IQueueProvider _queueProvider;
        private readonly IUpdatesRepository _updatesRepository;

        public DeleteCommentCommandHandler(
            IRepository<Comment> boxCommentRepository,
            IBoxRepository boxRepository,
            IUserRepository userRepository, IQueueProvider queueProvider, IUpdatesRepository updatesRepository)
        {
            m_BoxCommentRepository = boxCommentRepository;
            _boxRepository = boxRepository;
            _userRepository = userRepository;
            _queueProvider = queueProvider;
            _updatesRepository = updatesRepository;
        }

        public Task HandleAsync(DeleteCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var comment = m_BoxCommentRepository.Load(message.CommentId);
            var user = _userRepository.Load(message.UserId);
            var box = comment.Box;

            var isAuthorize = comment.User.Id == message.UserId
                || box.Owner.Id == message.UserId
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException();
            }
            _updatesRepository.DeleteCommentUpdates(comment.Id);
            var userIds = box.DeleteComment(comment);

            m_BoxCommentRepository.Delete(comment);
            _boxRepository.Save(box);
            var t2 = _queueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));

            var t1 =  _queueProvider.InsertMessageToTransactionAsync(new ReputationData(userIds.Union(new[] { user.Id })));
            return Task.WhenAll(t1, t2);
        }
    }
}
