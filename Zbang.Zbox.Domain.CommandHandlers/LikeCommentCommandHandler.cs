using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class LikeCommentCommandHandler : ICommandHandlerAsync<LikeCommentCommand, LikeCommentCommandResult>
    {
        private readonly ICommentLikeRepository m_CommentLikeRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IQueueProvider m_QueueProvider;

        public LikeCommentCommandHandler(ICommentLikeRepository commentLikeRepository, IUserRepository userRepository, IRepository<Comment> commentRepository, IGuidIdGenerator guidGenerator, IQueueProvider queueProvider)
        {
            m_CommentLikeRepository = commentLikeRepository;
            m_UserRepository = userRepository;
            m_CommentRepository = commentRepository;
            m_GuidGenerator = guidGenerator;
            m_QueueProvider = queueProvider;
        }

        public async Task<LikeCommentCommandResult> ExecuteAsync(LikeCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var commentLike = m_CommentLikeRepository.GetUserLike(message.UserId, message.CommentId);
            var comment = m_CommentRepository.Load(message.CommentId);
            if (commentLike == null)
            {
                comment.LikeCount++;
                var user = m_UserRepository.Load(message.UserId);
                commentLike = new CommentLike(comment, user, m_GuidGenerator.GetId());

                m_CommentRepository.Save(comment);
                m_CommentLikeRepository.Save(commentLike);

                await CreateQueuesDataAsync(message.UserId, comment.User.Id).ConfigureAwait(true);
                return new LikeCommentCommandResult(true);
            }

            comment.LikeCount--;
            m_CommentRepository.Save(comment);
            m_CommentLikeRepository.Delete(commentLike);
            await CreateQueuesDataAsync(message.UserId, comment.User.Id).ConfigureAwait(true);

           await m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(comment.User.Id)).ConfigureAwait(true);
            return new LikeCommentCommandResult(false);
        }

        private async Task CreateQueuesDataAsync(long userWhoMadeAction, long commentUser)
        {
            var t1 =  m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(commentUser));
            var t2 = m_QueueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(userWhoMadeAction));
            await Task.WhenAll(t1, t2).ConfigureAwait(true);
        }
    }
}
