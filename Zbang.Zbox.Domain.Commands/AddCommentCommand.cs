using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddCommentCommand : ICommandAsync
    {
        public AddCommentCommand(long userId, long boxId, string text, Guid id, IEnumerable<long> filesIds)
        {
            UserId = userId;
            BoxId = boxId;
            Text = text;
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
