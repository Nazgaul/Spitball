using System;

using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class UpdateQuizCommandHandler : ICommandHandler<UpdateQuizCommand>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;


        public UpdateQuizCommandHandler(

            IRepository<Domain.Quiz> quizRepository)
        {
            m_QuizRepository = quizRepository;
        }

        public void Handle(UpdateQuizCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var quiz = m_QuizRepository.Load(message.QuizId);

            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }
            quiz.UpdateText(TextManipulation.EncodeText(message.Text));
            m_QuizRepository.Save(quiz);

        }
    }
}
