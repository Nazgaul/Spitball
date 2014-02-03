using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddQuestionCommand : ICommand
    {
        public AddQuestionCommand(long userId, long boxId, string text, Guid id, IEnumerable<long> filesIds)
        {
            UserId = userId;
            BoxId = boxId;
            Text = text.Trim();
            Id = id;
            FilesIds = filesIds;
        }
        public long UserId { get; private set; }

        public long BoxId { get; private set; }
        public IEnumerable<long> FilesIds { get; private set; }

        public string Text { get; private set; }
        public Guid Id { get; private set; }
    }
}
