using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class LikeReplyCommandHandler : ICommandHandler<LikeReplyCommand, LikeReplyCommandResult>
    {
        private readonly IReplyLikeRepository m_ReplyLikeRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<CommentReplies> m_ReplyRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;

        public LikeReplyCommandHandler(IReplyLikeRepository replyLikeRepository,
            IUserRepository userRepository,
            IRepository<CommentReplies> replyRepository, IGuidIdGenerator guidGenerator)
        {
            m_ReplyLikeRepository = replyLikeRepository;
            m_UserRepository = userRepository;
            m_ReplyRepository = replyRepository;
            m_GuidGenerator = guidGenerator;
        }

        public LikeReplyCommandResult Execute(LikeReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var replyLike = m_ReplyLikeRepository.GetUserLike(message.UserId, message.ReplyId);
            var reply = m_ReplyRepository.Load(message.ReplyId);
            if (replyLike == null)
            {

                reply.LikeCount++;
                var user = m_UserRepository.Load(message.UserId);
                replyLike = new ReplyLike(reply, user, m_GuidGenerator.GetId());

                m_ReplyRepository.Save(reply);
                m_ReplyLikeRepository.Save(replyLike);
                return new LikeReplyCommandResult(true);
            }

            reply.LikeCount--;
            m_ReplyRepository.Save(reply);
            m_ReplyLikeRepository.Delete(replyLike);
            return new LikeReplyCommandResult(false);

        }
    }
}
