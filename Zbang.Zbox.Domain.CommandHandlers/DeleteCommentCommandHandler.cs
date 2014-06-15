using System;
using System.Linq;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand>
    {
        private readonly IRepository<Comment> m_QuestionRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Updates> m_Updates;

        public DeleteCommentCommandHandler(
            IRepository<Comment> questionRepository,
            IBoxRepository boxRepository,
            IRepository<Reputation> reputationRepository,
            IRepository<Updates> updates)
        {
            m_QuestionRepository = questionRepository;
            m_BoxRepository = boxRepository;
            m_Updates = updates;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(DeleteCommentCommand message)
        {
            var question = m_QuestionRepository.Load(message.QuestionId);
            var box = question.Box;


            bool isAuthorize = question.User.Id == message.UserId || box.Owner.Id == message.UserId;
            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User didnt ask the question");
            }


            var substract = question.AnswersReadOnly.Count + 1;

            foreach (var item in question.AnswersReadOnly)
            {
                m_ReputationRepository.Save(item.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteAnswer));
            }

            var updatesToThatQuiz = m_Updates.GetQuerable().Where(w => w.Comment.Id == message.QuestionId);

            foreach (var quizUpdate in updatesToThatQuiz)
            {
                m_Updates.Delete(quizUpdate);
            }

            m_ReputationRepository.Save(question.User.AddReputation(Infrastructure.Enums.ReputationAction.DeleteQuestion));
            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) - substract);

            m_BoxRepository.Save(box);

            m_QuestionRepository.Delete(question);

        }
    }
}
