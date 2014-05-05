﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class DeleteQuizCommandHandler : ICommandHandler<DeleteQuizCommand>
    {
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Updates> m_Updates;


        public DeleteQuizCommandHandler(IRepository<Zbang.Zbox.Domain.Quiz> quizRepository,
            IRepository<Updates> updates)
        {
            m_QuizRepository = quizRepository;
            m_Updates = updates;
        }
        public void Handle(DeleteQuizCommand message)
        {
            var quiz = m_QuizRepository.Load(message.QuizId);

            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }
            var updatesToThatQuiz = m_Updates.GetQuerable().Where(w => w.Quiz.Id == message.QuizId);

            foreach (var quizUpdate in updatesToThatQuiz)
            {
                m_Updates.Delete(quizUpdate);
            }

            m_QuizRepository.Delete(quiz);
        }
    }
}
