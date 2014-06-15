using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class UpdateAnswerCommand : ICommand
    {
        public UpdateAnswerCommand(long userId, string text,  Guid id)
        {
            UserId = userId;
            Text = text;
            Id = id;
        }
        public long UserId { get; private set; }
        public string Text { get; private set; }
        public Guid Id { get; private set; }
    }
}
