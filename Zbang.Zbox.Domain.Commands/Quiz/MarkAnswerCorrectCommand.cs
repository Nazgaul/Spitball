using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class MarkAnswerCorrectCommand : ICommand
    {
        public MarkAnswerCorrectCommand(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }
        public Guid Id { get; private set; }

        public long UserId { get; private set; }
    }
}
