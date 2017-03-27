using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddCommentCommand : ICommandAsync, ICommandCache
    {
        public AddCommentCommand(long userId, long boxId, string text, Guid id, IEnumerable<long> filesIds, bool postAnonymously)
        {
            PostAnonymously = postAnonymously;
            UserId = userId;
            BoxId = boxId;
            Text = text;
            Id = id;
            FilesIds = filesIds;
        }
        public long UserId { get; private set; }

        public long BoxId { get; }
        public IEnumerable<long> FilesIds { get; private set; }

        public string Text { get; private set; }
        public Guid Id { get; private set; }

        public bool PostAnonymously { get; private set; }
        public string CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
