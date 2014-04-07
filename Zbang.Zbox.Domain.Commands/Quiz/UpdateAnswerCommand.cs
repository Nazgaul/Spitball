using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class UpdateAnswerCommand : ICommand
    {
        public UpdateAnswerCommand(long userId, string text, bool isCorrect, Guid id)
        {
            UserId = userId;
            Text = text;
            IsCorrect = isCorrect;
            Id = id;
        }
        public long UserId { get; private set; }
        public string Text { get; private set; }
        public bool IsCorrect { get; private set; }
        public Guid Id { get; private set; }
    }
}
