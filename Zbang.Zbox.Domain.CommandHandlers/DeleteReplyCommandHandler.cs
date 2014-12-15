using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteReplyCommandHandler : ICommandHandler<DeleteReplyCommand>
    {
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteReplyCommandHandler(
            IRepository<CommentReplies> answerRepository,
            IQueueProvider queueProvider)
        {
            m_AnswerRepository = answerRepository;
            m_QueueProvider = queueProvider;
        }
        public void Handle(DeleteReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var answer = m_AnswerRepository.Load(message.AnswerId);
            var box = answer.Box;

            bool isAuthorize = answer.User.Id == message.UserId
                || box.Owner.Id == message.UserId;
            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didn't ask the answer");
            }

            m_QueueProvider.InsertMessageToTranaction(new ReputationData(answer.User.Id));
            m_AnswerRepository.Delete(answer);

        }
    }
}
