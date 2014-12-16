using System;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
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
        private readonly IRepository<Domain.Quiz> m_QuizRepository;

        public CreateQuizCommandHandler(IUserRepository userRepository,
            IBoxRepository boxRepository,
            IRepository<Domain.Quiz> quizRepository
            )
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_QuizRepository = quizRepository;
        }
        public void Handle(CreateQuizCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var user = m_UserRepository.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);
           
            var quiz = new Domain.Quiz(TextManipulation.EncodeText(message.Text), message.QuizId, box, user);
            
            m_QuizRepository.Save(quiz);
        }
    }
}
