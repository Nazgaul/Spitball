using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteReplyCommandHandler : ICommandHandler<DeleteReplyCommand>
    {
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;

        public DeleteReplyCommandHandler(
            IRepository<CommentReplies> answerRepository,
            IRepository<Reputation> reputationRepository
            )
        {
            m_AnswerRepository = answerRepository;
            m_ReputationRepository = reputationRepository;
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

            m_ReputationRepository.Save(answer.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteAnswer));
            m_AnswerRepository.Delete(answer);

        }
    }
}
