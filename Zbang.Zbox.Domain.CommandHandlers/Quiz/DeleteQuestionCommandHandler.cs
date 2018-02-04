﻿using System;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> m_QuestionRepository;
        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            m_QuestionRepository = questionRepository;
        }

        public void Handle(DeleteQuestionCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var question = m_QuestionRepository.Load(message.QuestionId);
            if (question.Quiz.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not owner of quiz");
            }

            m_QuestionRepository.Delete(question);
        }
    }
}
