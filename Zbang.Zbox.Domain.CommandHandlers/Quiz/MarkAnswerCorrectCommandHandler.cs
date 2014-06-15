using System;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class MarkAnswerCorrectCommandHandler : ICommandHandler<MarkAnswerCorrectCommand>
    {
        private readonly IRepository<Answer> m_AnswerRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        public MarkAnswerCorrectCommandHandler(IRepository<Answer> answerRepository, IRepository<Question> questionRepository)
        {
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
        }
        public void Handle(MarkAnswerCorrectCommand message)
        {
            var answer = m_AnswerRepository.Load(message.Id);

            if (answer.Quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not owner of quiz");
            }
            answer.UpdateCorrectAnswer();
            m_QuestionRepository.Save(answer.Question);

        }
    }
}
