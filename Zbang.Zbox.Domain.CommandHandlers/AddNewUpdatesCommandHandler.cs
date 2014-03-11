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
        private readonly IRepository<Answer> m_AnswerRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Updates> m_UpdatesRepository;
        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository,
            IRepository<Item> itemRepository,
            IRepository<Answer> answerRepository,
            IRepository<Question> questionRepository,
            IRepository<Updates> updatesRepository
             )
        {
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
            m_UpdatesRepository = updatesRepository;

        }
        public void Handle(AddNewUpdatesCommand message)
        {
            var box = m_BoxRepository.Load(message.BoxId);
            foreach (var userBoxRel in box.UserBoxRel.Where(w => w.User.Id != message.UserId))
            {

                var newUpdate = new Updates(userBoxRel.User, box,
                    GetQuestion(message.QuestionId),
                    GetAnswer(message.AnswerId),
                    GetItem(message.ItemId));
                m_UpdatesRepository.Save(newUpdate);
            }

        }

        private Item GetItem(long? itemId)
        {
            if (!itemId.HasValue)
            {
                return null;
            }
            return m_ItemRepository.Load(itemId.Value);
        }

        private Answer GetAnswer(Guid? answerId)
        {
            if (!answerId.HasValue)
            {
                return null;
            }
            return m_AnswerRepository.Load(answerId.Value);
        }

        private Question GetQuestion(Guid? questionId)
        {
            if (!questionId.HasValue)
            {
                return null;
            }
            return m_QuestionRepository.Load(questionId.Value);
        }

    }
}
