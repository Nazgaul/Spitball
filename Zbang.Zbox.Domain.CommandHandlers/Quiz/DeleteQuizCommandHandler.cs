using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class DeleteQuizCommandHandler : ICommandHandler<DeleteQuizCommand>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Updates> m_Updates;
        private readonly IRepository<SolvedQuiz> m_SolvedQuizRepository;

        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IUserRepository m_UserRepository;

        public DeleteQuizCommandHandler(IRepository<Domain.Quiz> quizRepository,
            IRepository<Updates> updates,
            IRepository<SolvedQuiz> solvedQuizRepository,
            IRepository<Reputation> reputationRepository,
            IUserRepository userRepository)
        {
            m_QuizRepository = quizRepository;
            m_Updates = updates;
            m_SolvedQuizRepository = solvedQuizRepository;
            m_ReputationRepository = reputationRepository;
            m_UserRepository = userRepository;
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

            var solvedQuizes = m_SolvedQuizRepository.GetQuerable().Where(w => w.Quiz.Id == message.QuizId);
            foreach (var solvedQuiz in solvedQuizes)
            {
                m_SolvedQuizRepository.Delete(solvedQuiz);
            }
            m_ReputationRepository.Save(quiz.Owner.AddReputation(ReputationAction.DelteQuiz));
            m_UserRepository.Save(quiz.Owner);
            m_QuizRepository.Delete(quiz);
        }
    }
}
