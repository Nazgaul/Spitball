using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteReplyCommandHandler : ICommandHandler<DeleteReplyCommand>
    {
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IRepository<Updates> m_Updates;

        public DeleteReplyCommandHandler(IRepository<CommentReplies> answerRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository,
            IRepository<Updates> updates)
        {
            m_AnswerRepository = answerRepository;
            m_BoxRepository = boxRepository;
            m_ReputationRepository = reputationRepository;
            m_Updates = updates;
        }
        public void Handle(DeleteReplyCommand message)
        {
            var answer = m_AnswerRepository.Load(message.AnswerId);
            var box = answer.Box;

            bool isAuthorize = answer.User.Id == message.UserId || box.Owner.Id == message.UserId;
            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didnt ask the answer");
            }

            var updatesToThatQuiz = m_Updates.GetQuerable().Where(w => w.Reply.Id == message.AnswerId);

            foreach (var quizUpdate in updatesToThatQuiz)
            {
                m_Updates.Delete(quizUpdate);
            }
            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) - 1);

            m_ReputationRepository.Save(answer.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteAnswer));
            m_BoxRepository.Save(box);
            m_AnswerRepository.Delete(answer);

        }
    }
}
