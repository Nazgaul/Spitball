using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddQuizLikeCommand : ICommandAsync
    {
        public AddQuizLikeCommand(long userId, long quizId)
        {
            UserId = userId;
            QuizId = quizId;
        }

        public long UserId { get; private set; }
        public long QuizId { get; private set; }

        public Guid Id { get; set; }
    }
}