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
    public class LikeReplyCommandHandler : ICommandHandlerAsync<LikeReplyCommand, LikeReplyCommandResult>
    {
        private readonly IReplyLikeRepository m_ReplyLikeRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<CommentReply> m_ReplyRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IQueueProvider m_QueueProvider;

        public LikeReplyCommandHandler(IReplyLikeRepository replyLikeRepository,
            IUserRepository userRepository,
            IRepository<CommentReply> replyRepository, IGuidIdGenerator guidGenerator, IQueueProvider queueProvider)
        {
            m_ReplyLikeRepository = replyLikeRepository;
            m_UserRepository = userRepository;
            m_ReplyRepository = replyRepository;
            m_GuidGenerator = guidGenerator;
            m_QueueProvider = queueProvider;
        }

        public async Task<LikeReplyCommandResult> ExecuteAsync(LikeReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var replyLike = m_ReplyLikeRepository.GetUserLike(message.UserId, message.ReplyId);
            var reply = m_ReplyRepository.Load(message.ReplyId);
            if (replyLike == null)
            {
                reply.LikeCount++;
                var user = m_UserRepository.Load(message.UserId);
                replyLike = new ReplyLike(reply, user, m_GuidGenerator.GetId());

                m_ReplyRepository.Save(reply);
                m_ReplyLikeRepository.Save(replyLike);
                await CreateQueuesDataAsync(message.UserId, reply.User.Id);
                return new LikeReplyCommandResult(true);
            }

            reply.LikeCount--;
            m_ReplyRepository.Save(reply);
            m_ReplyLikeRepository.Delete(replyLike);
            await CreateQueuesDataAsync(message.UserId, reply.User.Id);
            return new LikeReplyCommandResult(false);
        }

        private async Task CreateQueuesDataAsync(long userWhoMadeAction, long commentUser)
        {
            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(commentUser));
            var t2 = m_QueueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(userWhoMadeAction));
            await Task.WhenAll(t1, t2);
        }
    }
}
