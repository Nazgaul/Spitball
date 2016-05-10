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
    public class DeleteReplyCommandHandler : ICommandHandlerAsync<DeleteReplyCommand>
    {
        private readonly IRepository<CommentReplies> m_ReplyRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;


        public DeleteReplyCommandHandler(
            IRepository<CommentReplies> answerRepository,
            IQueueProvider queueProvider, IUserRepository userRepository)
        {
            m_ReplyRepository = answerRepository;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
        }
        public Task HandleAsync(DeleteReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var answer = m_ReplyRepository.Load(message.AnswerId);
            var box = answer.Box;
            var user = m_UserRepository.Load(message.UserId);

            bool isAuthorize = answer.User.Id == message.UserId
                || box.Owner.Id == message.UserId
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException();
            }
            answer.Question.ReplyCount--;
            //if (answer.Question.LastReplyId == answer.Id)
            //{
            //    var lastReplyId = answer.Question.RepliesReadOnly.LastOrDefault(w => w.Id != answer.Id);
            //    if (lastReplyId == null)
            //    {
            //        answer.Question.LastReplyId = null;
            //    }
            //    else
            //    {
            //        answer.Question.LastReplyId = lastReplyId.Id;
            //    }
            //}
            m_ReplyRepository.Delete(answer);
            return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(answer.User.Id));

        }
    }
}
