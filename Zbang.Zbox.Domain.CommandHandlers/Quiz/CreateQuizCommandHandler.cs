using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class CreateQuizCommandHandler : ICommandHandler<CreateQuizCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;

        public CreateQuizCommandHandler(IUserRepository userRepository,
            IBoxRepository boxRepository,
            IRepository<Zbang.Zbox.Domain.Quiz> quizRepository,
            IRepository<Reputation> reputationRepository)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_QuizRepository = quizRepository;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(CreateQuizCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);

            if (userType == Infrastructure.Enums.UserRelationshipType.None)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                box.CalculateMembers();
                m_UserRepository.Save(user);
            }
            var quiz = new Zbang.Zbox.Domain.Quiz(message.Text, message.QuizId, box, user);
            m_ReputationRepository.Save(user.AddReputation(ReputationAction.AddQuiz));
            m_UserRepository.Save(user);
            m_QuizRepository.Save(quiz);
        }
    }
}
