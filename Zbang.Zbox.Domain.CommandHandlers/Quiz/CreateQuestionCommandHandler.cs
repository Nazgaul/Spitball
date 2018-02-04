using System;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Answer> m_AnswerRepository;

        public CreateQuestionCommandHandler(
            IRepository<Domain.Quiz> quizRepository,
            IRepository<Question> questionRepository, IRepository<Answer> answerRepository)
        {
            m_QuizRepository = quizRepository;
            m_QuestionRepository = questionRepository;
            m_AnswerRepository = answerRepository;
        }

        public void Handle(CreateQuestionCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var quiz = m_QuizRepository.Load(message.QuizId);
            if (quiz.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not quiz owner");
            }

            var question = new Question(message.Question.Id, quiz, TextManipulation.EncodeText(message.Question.Text, Question.AllowedHtmlTag));
            m_QuestionRepository.Save(question);
            foreach (var commandAnswer in message.Question.Answers)
            {
                var answer = new Answer(commandAnswer.Id, commandAnswer.Text, question);
                if (commandAnswer.IsCorrect)
                {
                    question.UpdateCorrectAnswer(answer);
                    //answer.UpdateCorrectAnswer();
                }
                m_AnswerRepository.Save(answer);
            }
            m_QuestionRepository.Save(question);
        }
    }
}
