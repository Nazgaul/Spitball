using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class CreateQuizCommandHandler : ICommandHandler<CreateQuizCommand>
    {
        private readonly IUserRepository m_UserReposiotry;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;

        public CreateQuizCommandHandler(IUserRepository userRepositoy,
            IBoxRepository boxRepository,
            IRepository<Zbang.Zbox.Domain.Quiz> quizRepository)
        {
            m_UserReposiotry = userRepositoy;
            m_BoxRepository = boxRepository;
            m_QuizRepository = quizRepository;
        }
        public void Handle(CreateQuizCommand message)
        {
            var user = m_UserReposiotry.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);

            var userType = m_UserReposiotry.GetUserToBoxRelationShipType(message.UserId, message.BoxId);

            if (userType == Infrastructure.Enums.UserRelationshipType.None)
            {
                throw new UnauthorizedAccessException("user not connected to box");
            }
            var quiz = new Zbang.Zbox.Domain.Quiz(message.Text, message.QuizId, box, user);

            m_QuizRepository.Save(quiz);
        }
    }
}
