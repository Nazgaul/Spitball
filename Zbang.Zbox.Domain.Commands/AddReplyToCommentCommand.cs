using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddReplyToCommentCommand : ICommandAsync, ICommandCache
    {
        public AddReplyToCommentCommand(long userId, long boxId, string text, Guid answerId, Guid questionId, IEnumerable<long> filesIds)
        {
            UserId = userId;
            BoxId = boxId;
            Text = text;
            Id = answerId;
            QuestionId = questionId;
            FilesIds = filesIds;
        }

        public long UserId { get; private set; }

        public long BoxId { get; }

        public string Text { get; private set; }

        public Guid Id { get; private set; }

        public Guid QuestionId { get; private set; }
        public IEnumerable<long> FilesIds { get; private set; }
        public CacheRegions CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
