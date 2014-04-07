using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class UpdateQuizCommand : ICommand
    {
        public UpdateQuizCommand(long userId, long quizId, string text)
        {
            UserId = userId;
            QuizId = quizId;
            Text = text;
        }
        public long UserId { get; private set; }
        public long QuizId { get; private set; }

        public string Text { get; private set; }

    }
}
