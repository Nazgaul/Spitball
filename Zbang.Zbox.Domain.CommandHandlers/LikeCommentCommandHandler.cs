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
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IGuidIdGenerator _guidGenerator;
        private readonly IQueueProvider _queueProvider;

        public LikeCommentCommandHandler(ICommentLikeRepository commentLikeRepository, IUserRepository userRepository, IRepository<Comment> commentRepository, IGuidIdGenerator guidGenerator, IQueueProvider queueProvider)
        {
            m_CommentLikeRepository = commentLikeRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _guidGenerator = guidGenerator;
            _queueProvider = queueProvider;
        }

        public async Task<LikeCommentCommandResult> ExecuteAsync(LikeCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var commentLike = m_CommentLikeRepository.GetUserLike(message.UserId, message.CommentId);
            var comment = _commentRepository.Load(message.CommentId);
            if (commentLike == null)
            {
                comment.LikeCount++;
                var user = _userRepository.Load(message.UserId);
                commentLike = new CommentLike(comment, user, _guidGenerator.GetId());

                _commentRepository.Save(comment);
                m_CommentLikeRepository.Save(commentLike);

                await CreateQueuesDataAsync(message.UserId, comment.User.Id).ConfigureAwait(true);
                return new LikeCommentCommandResult(true);
            }

            comment.LikeCount--;
            _commentRepository.Save(comment);
            m_CommentLikeRepository.Delete(commentLike);
            await CreateQueuesDataAsync(message.UserId, comment.User.Id).ConfigureAwait(true);

           await _queueProvider.InsertMessageToTransactionAsync(new ReputationData(comment.User.Id)).ConfigureAwait(true);
            return new LikeCommentCommandResult(false);
        }

        private async Task CreateQueuesDataAsync(long userWhoMadeAction, long commentUser)
        {
            var t1 =  _queueProvider.InsertMessageToTransactionAsync(new ReputationData(commentUser));
            var t2 = _queueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(userWhoMadeAction));
            await Task.WhenAll(t1, t2).ConfigureAwait(true);
        }
    }
}
