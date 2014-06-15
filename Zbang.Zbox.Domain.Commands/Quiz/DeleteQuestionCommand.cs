using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class DeleteQuestionCommand : ICommand
    {
        public DeleteQuestionCommand(long userId, Guid questionId)
        {
            UserId = userId;
            QuestionId = questionId;
        }
        public long UserId { get; private set; }
        public Guid QuestionId { get;private set; }
    }
}
