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
        private readonly IRepository<CommentReply> m_ReplyRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IUpdatesRepository m_UpdatesRepository;


        public DeleteReplyCommandHandler(
            IRepository<CommentReply> answerRepository,
            IQueueProvider queueProvider, IUserRepository userRepository, IUpdatesRepository updatesRepository)
        {
            m_ReplyRepository = answerRepository;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_UpdatesRepository = updatesRepository;
        }
        public Task HandleAsync(DeleteReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var answer = m_ReplyRepository.Load(message.AnswerId);
            var box = answer.Box;
            var user = m_UserRepository.Load(message.UserId);

            var isAuthorize = answer.User.Id == message.UserId
                || box.Owner.Id == message.UserId
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException();
            }
            answer.Question.ReplyCount--;
            m_UpdatesRepository.DeleteReplyUpdates(answer.Id);
            m_ReplyRepository.Delete(answer);
            return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(answer.User.Id));

        }
    }
}
