using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class SaveUserQuizCommandHandler : ICommandHandlerAsync<SaveUserQuizCommand>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<SolvedQuiz> m_SolvedQuizRepository;
        private readonly IRepository<SolvedQuestion> m_SolvedQuestionRepository;
        private readonly IRepository<Answer> m_AnswerRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IQueueProvider m_QueueProvider;

        private readonly IUserRepository m_UserRepository;

        public SaveUserQuizCommandHandler(
            IRepository<Domain.Quiz> quizRepository,
            IUserRepository userRepository,
            IRepository<SolvedQuiz> solvedQuizRepository,
            IRepository<SolvedQuestion> solvedQuestionRepository,
            IRepository<Answer> answerRepository,
            IGuidIdGenerator idGenerator, IQueueProvider queueProvider)
        {
            m_QuizRepository = quizRepository;
            m_UserRepository = userRepository;
            m_SolvedQuizRepository = solvedQuizRepository;
            m_SolvedQuestionRepository = solvedQuestionRepository;
            m_IdGenerator = idGenerator;
            m_QueueProvider = queueProvider;
            m_AnswerRepository = answerRepository;
        }

        public Task HandleAsync(SaveUserQuizCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            var quiz = m_QuizRepository.Load(message.QuizId);


            var answerSheet = m_SolvedQuizRepository.GetQueryable().FirstOrDefault(w =>
                // ReSharper disable once PossibleUnintendedReferenceComparison nHibernate doesn't support equals
                w.User == user && w.Quiz == quiz);
            if (answerSheet != null)
            {
                DeleteAnswers(answerSheet);
            }
            var solvedQuiz = new SolvedQuiz(m_IdGenerator.GetId(), quiz, user, message.TimeTaken);
            m_SolvedQuizRepository.Save(solvedQuiz, true);
            foreach (var question in quiz.Questions)
            {
                var userAnswer = message.Answers.FirstOrDefault(w => w.QuestionId == question.Id);
                if (userAnswer == null)
                {
                    continue;
                }
                var correct = userAnswer.AnswerId == question.RightAnswer.Id;
                var answer = m_AnswerRepository.Load(userAnswer.AnswerId);
                var solvedAnswer = new SolvedQuestion(m_IdGenerator.GetId(), user, question, answer, correct, solvedQuiz);
                solvedQuiz.AddSolvedQuestion(solvedAnswer);
                m_SolvedQuestionRepository.Save(solvedAnswer);
            }
            var score = (decimal)solvedQuiz.SolvedQuestions.Count(w => w.Correct) / quiz.Questions.Count;
            solvedQuiz.Score = (int)Math.Round(score * 100);
            // TODO: check how can remove this 
            m_SolvedQuizRepository.Save(solvedQuiz, true);
            quiz.SolveCount = quiz.SolvedQuizes.Count;
            
            m_QuizRepository.Save(quiz);
            return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(quiz.User.Id));


        }

        private void DeleteAnswers(SolvedQuiz answerSheet)
        {
            m_SolvedQuizRepository.Delete(answerSheet);
        }




    }
}
