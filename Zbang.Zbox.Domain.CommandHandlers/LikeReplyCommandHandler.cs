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
        private readonly IReplyLikeRepository _replyLikeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<CommentReply> _replyRepository;
        private readonly IGuidIdGenerator _guidGenerator;
        private readonly IQueueProvider _queueProvider;

        public LikeReplyCommandHandler(IReplyLikeRepository replyLikeRepository,
            IUserRepository userRepository,
            IRepository<CommentReply> replyRepository, IGuidIdGenerator guidGenerator, IQueueProvider queueProvider)
        {
            _replyLikeRepository = replyLikeRepository;
            _userRepository = userRepository;
            _replyRepository = replyRepository;
            _guidGenerator = guidGenerator;
            _queueProvider = queueProvider;
        }

        public async Task<LikeReplyCommandResult> ExecuteAsync(LikeReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var replyLike = _replyLikeRepository.GetUserLike(message.UserId, message.ReplyId);
            var reply = _replyRepository.Load(message.ReplyId);
            if (replyLike == null)
            {
                reply.LikeCount++;
                var user = _userRepository.Load(message.UserId);
                replyLike = new ReplyLike(reply, user, _guidGenerator.GetId());

                _replyRepository.Save(reply);
                _replyLikeRepository.Save(replyLike);
                await CreateQueuesDataAsync(message.UserId, reply.User.Id);
                return new LikeReplyCommandResult(true);
            }

            reply.LikeCount--;
            _replyRepository.Save(reply);
            _replyLikeRepository.Delete(replyLike);
            await CreateQueuesDataAsync(message.UserId, reply.User.Id);
            return new LikeReplyCommandResult(false);
        }

        private async Task CreateQueuesDataAsync(long userWhoMadeAction, long commentUser)
        {
            var t1 = _queueProvider.InsertMessageToTransactionAsync(new ReputationData(commentUser));
            var t2 = _queueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(userWhoMadeAction));
            await Task.WhenAll(t1, t2);
        }
    }
}
