using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(string text, long quizId, long userId, Guid questionId)
        {
            Text = text;
            QuizId = quizId;
            UserId = userId;
            QuestionId = questionId;
        }
        public string Text { get; private set; }
        public long QuizId { get; private set; }
        public long UserId { get; private set; }
        public Guid QuestionId { get; private set; }

    }
}
