using System;

using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IRepository<Answer> m_AnswerRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        public DeleteAnswerCommandHandler(IRepository<Answer> answerRepository, IRepository<Question> questionRepository)
        {
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
        }
        public void Handle(DeleteAnswerCommand message)
        {
            var answer = m_AnswerRepository.Load(message.Id);
            if (answer.Quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not owner of quiz");
            }
            if (answer == answer.Question.RightAnswer)
            {
                answer.Question.UpdateCorrectAnswer(null);
                m_QuestionRepository.Save(answer.Question);
            }
            m_AnswerRepository.Delete(answer);
        }
    }
}
