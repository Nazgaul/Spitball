using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddNewUpdatesCommandHandler : ICommandHandler<AddNewUpdatesCommand>
    {
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IRepository<Comment> m_QuestionRepository;
        private readonly IRepository<Updates> m_UpdatesRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;

        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReplies> answerRepository,
            IRepository<Comment> questionRepository,
            IRepository<Updates> updatesRepository,
            IRepository<Domain.Quiz> quizRepository
             )
        {
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
            m_UpdatesRepository = updatesRepository;
            m_QuizRepository = quizRepository;

        }
        public void Handle(AddNewUpdatesCommand message)
        {
            var box = m_BoxRepository.Load(message.BoxId);
            foreach (var userBoxRel in box.UserBoxRel.Where(w => w.User.Id != message.UserId))
            {

                var newUpdate = new Updates(userBoxRel.User, box,
                    GetQuestion(message.QuestionId),
                    GetAnswer(message.AnswerId),
                    GetItem(message.ItemId),
                    GetQuiz(message.QuizId)
                   );
                m_UpdatesRepository.Save(newUpdate);
            }

        }
        private Zbang.Zbox.Domain.Quiz GetQuiz(long? quizId)
        {
            if (!quizId.HasValue)
            {
                return null;
            }
            return m_QuizRepository.Load(quizId.Value);
        }

        private Item GetItem(long? itemId)
        {
            if (!itemId.HasValue)
            {
                return null;
            }
            return m_ItemRepository.Load(itemId.Value);
        }

        private CommentReplies GetAnswer(Guid? answerId)
        {
            if (!answerId.HasValue)
            {
                return null;
            }
            return m_AnswerRepository.Load(answerId.Value);
        }

        private Comment GetQuestion(Guid? questionId)
        {
            if (!questionId.HasValue)
            {
                return null;
            }
            return m_QuestionRepository.Load(questionId.Value);
        }

    }
}
