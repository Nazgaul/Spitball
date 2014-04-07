using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class DeleteQuizCommand : ICommand
    {
        public DeleteQuizCommand(long quizId, long userId)
        {
            QuizId = quizId;
            UserId = userId;
        }
        public long QuizId { get; private set; }
        public long UserId { get; private set; }
    }
}
