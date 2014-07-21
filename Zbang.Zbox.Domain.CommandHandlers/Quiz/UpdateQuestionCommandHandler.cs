using System;

using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class UpdateQuestionCommandHandler : ICommandHandler<UpdateQuestionCommand>
    {
        private readonly IRepository<Question> m_QuestionRepository;
        public UpdateQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            m_QuestionRepository = questionRepository;
        }
        public void Handle(UpdateQuestionCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var question = m_QuestionRepository.Load(message.QuestionId);
            if (question.Quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not owner of quiz");
            }

            question.UpdateText(TextManipulation.EncodeText(message.NewText));
            m_QuestionRepository.Save(question);


        }
    }
}
