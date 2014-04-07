using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteReplyCommand : ICommand
    {
        public DeleteReplyCommand(Guid answerId, long userId)
        {
            AnswerId = answerId;
            UserId = userId;
        }
        public Guid AnswerId { get; set; }

        public long UserId { get; set; }
    }
}
