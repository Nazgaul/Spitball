using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class RateAnswerCommand : ICommand
    {
        public RateAnswerCommand(long userId, Guid answerId, Guid id)
        {
            UserId = userId;
            AnswerId = answerId;
            Id = id;
        }
        public long UserId { get; private set; }

        public Guid AnswerId { get; private set; }

        public Guid Id { get; private set; }
    }
}
