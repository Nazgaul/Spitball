using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Item> m_ItemRepository;

        private const int AmoutOfPointOfAddingQuestion = 5;

        public AddQuestionCommandHandler(IUserRepository userRepository,
            IBoxRepository boxRepository,
            IRepository<Question> questionRepository
            , IRepository<Item> itemRepository)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_QuestionRepository = questionRepository;
            m_ItemRepository = itemRepository;
        }
        public void Handle(AddQuestionCommand message)
        {
            Throw.OnNull(message, "message");

            var user = m_UserRepository.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);
            //Decode the comment to html friendly
            var text = TextManipulation.EncodeComment(message.Text);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, box.Id); //user.GetUserType(box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }
            var files = message.FilesIds.Select(s => m_ItemRepository.Load(s)).ToList();

            var question = new Question(user, text, box, message.Id, files);
            m_QuestionRepository.Save(question);

            user.AddReputation(AmoutOfPointOfAddingQuestion);

            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) + 1);
            //box.UserTime.UpdateUserTime(user.Email);
            m_BoxRepository.Save(box);

        }
    }
}
