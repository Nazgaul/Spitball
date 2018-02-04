using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteReplyCommandHandler : ICommandHandlerAsync<DeleteReplyCommand>
    {
        private readonly IRepository<CommentReply> _replyRepository;
        private readonly IQueueProvider _queueProvider;
        private readonly IUserRepository _userRepository;
        private readonly IUpdatesRepository _updatesRepository;

        public DeleteReplyCommandHandler(
            IRepository<CommentReply> answerRepository,
            IQueueProvider queueProvider, IUserRepository userRepository, IUpdatesRepository updatesRepository)
        {
            _replyRepository = answerRepository;
            _queueProvider = queueProvider;
            _userRepository = userRepository;
            _updatesRepository = updatesRepository;
        }

        public Task HandleAsync(DeleteReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var answer = _replyRepository.Load(message.AnswerId);
            var box = answer.Box;
            var user = _userRepository.Load(message.UserId);

            var isAuthorize = answer.User.Id == message.UserId
                || box.Owner.Id == message.UserId
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException();
            }
            answer.Question.ReplyCount--;
            _updatesRepository.DeleteReplyUpdates(answer.Id);
            _replyRepository.Delete(answer);
            var task = Task.CompletedTask;
            if (answer.LikeCount > 0)
            {
                task = _queueProvider.InsertMessageToTransactionAsync(new ReputationData(answer.User.Id));
            }
            var t2 = _queueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));

            return Task.WhenAll(task, t2);
        }
    }
}
