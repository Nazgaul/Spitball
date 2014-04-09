using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class SaveQuizCommandHandler : ICommandHandler<SaveQuizCommand>
    {
        private readonly IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;
        private readonly IQueueProvider m_QueueProvider;
        // private readonly IRepository<Zbang.Zbox.Domain.Question> m_QuestionRepository;
        // private readonly IRepository<Zbang.Zbox.Domain.Answer> m_AnswerRepository;

        public SaveQuizCommandHandler(
            IRepository<Zbang.Zbox.Domain.Quiz> quizRepository,
            IQueueProvider queueProvider
            // IRepository<Zbang.Zbox.Domain.Question> questionRepository,
            // IRepository<Zbang.Zbox.Domain.Answer> answerRepository
            )
        {
            m_QuizRepository = quizRepository;
            m_QueueProvider = queueProvider;
            // m_QuestionRepository = questionRepository;
            // m_AnswerRepository = answerRepository;
        }
        public void Handle(SaveQuizCommand message)
        {
            var quiz = m_QuizRepository.Load(message.QuizId);
            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }
            //var questions = m_QuestionRepository.GetQuerable().Where(w => w.Quiz == quiz);
            //var answers = m_AnswerRepository.GetQuerable().Where(w => w.Quiz == quiz);

            if (string.IsNullOrEmpty(quiz.Name))
            {
                throw new ArgumentException("quiz name is empty");
            }
            foreach (var question in quiz.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Text))
                {
                    throw new ArgumentException("question is not right");
                }
                if (question.RightAnswer == null)
                {
                    throw new ArgumentException("question is not right");
                }
                if (question.Answers.Count() < 2)
                {
                    throw new ArgumentException("question is not right");
                }
                if (question.Answers.Where(w => w.Text == null).Count() > 0)
                {
                    throw new ArgumentException("question is not right");
                }

            }
            //var wrongQuestions = questions.Where(w => w.Text == null || w.RightAnswer == null).ToList();

            //if (wrongQuestions.Count > 0)
            //{
            //    throw new ArgumentException("question is not right");
            //}

            //var wrongAnswers = answers.Where(w => w.Text == null).ToList();
            //if (wrongAnswers.Count > 0)
            //{
            //    throw new ArgumentException("answers is not right");
            //}

            quiz.Publish = true;
            var sb = new StringBuilder();
            foreach (var question in quiz.Questions)
            {

                sb.AppendFormat("{0} ", question.Text);
                if (sb.Length > 400)
                {
                    sb.Remove(400, sb.Length);
                    break;
                }
            }
            quiz.Content = sb.ToString();
            m_QueueProvider.InsertMessageToTranaction(new UpdateData(quiz.Owner.Id, quiz.Box.Id, null, null, null, quiz.Id));
            m_QuizRepository.Save(quiz);
        }
    }
}
