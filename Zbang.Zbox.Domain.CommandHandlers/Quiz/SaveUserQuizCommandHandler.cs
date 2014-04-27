﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class SaveUserQuizCommandHandler : ICommandHandler<SaveUserQuizCommand>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Domain.SolvedQuiz> m_SolvedQuizRepository;
        private readonly IRepository<Domain.SolvedQuestion> m_SolvedQuestionRepository;
        private readonly IRepository<Domain.Answer> m_AnswerRepository;
        private readonly IIdGenerator m_IdGenerator;

        private readonly IUserRepository m_UserRepository;

        public SaveUserQuizCommandHandler(
            IRepository<Domain.Quiz> quizRepository,
            IUserRepository userRepository,
            IRepository<Domain.SolvedQuiz> solvedQuizRepository,
            IRepository<Domain.SolvedQuestion> solvedQuestionRepository,
            IRepository<Domain.Answer> answerRepository,
            IIdGenerator idGenerator
            )
        {
            m_QuizRepository = quizRepository;
            m_UserRepository = userRepository;
            m_SolvedQuizRepository = solvedQuizRepository;
            m_SolvedQuestionRepository = solvedQuestionRepository;
            m_IdGenerator = idGenerator;
            m_AnswerRepository = answerRepository;
        }

        public void Handle(SaveUserQuizCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var quiz = m_QuizRepository.Load(message.QuizId);

            var userRelationShipType = m_UserRepository.GetUserToBoxRelationShipType(user.Id, quiz.Box.Id);
            if (userRelationShipType == UserRelationshipType.Invite || userRelationShipType == UserRelationshipType.None)
            {
                user.ChangeUserRelationShipToBoxType(quiz.Box, UserRelationshipType.Subscribe);
                quiz.Box.CalculateMembers();
                m_UserRepository.Save(user);
            }
            var answerSheet = m_SolvedQuizRepository.GetQuerable().Where(w => w.User == user && w.Quiz == quiz).FirstOrDefault();
            if (answerSheet != null)
            {
                DeleteAnswers(answerSheet, user, quiz);
            }
            var solvedQuiz = new SolvedQuiz(m_IdGenerator.GetId(), quiz, user, message.TimeTaken);
            m_SolvedQuizRepository.Save(solvedQuiz);
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
            var score =  (decimal)solvedQuiz.SolvedQuestions.Where(w => w.Correct).Count() / quiz.Questions.Count();
            solvedQuiz.Score = (int)Math.Round(score * 100);
            m_SolvedQuizRepository.Save(solvedQuiz);
        }

        private void DeleteAnswers(SolvedQuiz answerSheet, User user, Domain.Quiz quiz)
        {
            m_SolvedQuizRepository.Delete(answerSheet);
        }




    }
}
