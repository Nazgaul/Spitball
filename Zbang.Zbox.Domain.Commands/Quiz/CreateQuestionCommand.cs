using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(long quizId, long userId, Question question)
        {
            QuizId = quizId;
            UserId = userId;
            Question = question;
        }

        public Question Question { get; private set; }
        public long QuizId { get; private set; }
        public long UserId { get; private set; }

    }

    public class Question
    {
        public Question(Guid id, string text, IEnumerable<Answer> answers)
        {
            Id = id;
            Text = text;
            Answers = answers;
        }

        public Guid Id { get; private set; }

        public string Text { get; private set; }

        public IEnumerable<Answer> Answers { get; private set; }

    }

    public class Answer
    {
        public Answer(Guid id, string text, bool isCorrect)
        {
            Id = id;
            Text = text;
            IsCorrect = isCorrect;
        }

        public Guid Id { get; private set; }

        public string Text { get; private set; }

        public bool IsCorrect { get; private set; }
    }
}
