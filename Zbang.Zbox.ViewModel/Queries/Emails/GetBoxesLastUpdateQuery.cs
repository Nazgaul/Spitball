using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetBoxesLastUpdateQuery : BaseDigestLastUpdateQuery
    {
        public GetBoxesLastUpdateQuery(NotificationSetting notificationSettings, long userId)
            :base(notificationSettings)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }

    public class GetUpdatesQuery
    {
        public GetUpdatesQuery(IEnumerable<long> boxIds, 
            IEnumerable<long?> itemIds,
            IEnumerable<long?> quizIds,
            IEnumerable<Guid?> commentsIds,
            IEnumerable<Guid?> repliesIds,
            IEnumerable<Guid?> discussionIds)
        {
            BoxIds = boxIds;
            ItemIds = itemIds;
            QuizIds = quizIds;
            CommentsIds = commentsIds;
            RepliesIds = repliesIds;
            DiscussionIds = discussionIds;
        }

        public IEnumerable<long> BoxIds { get; }
        public IEnumerable<long?> ItemIds { get;  }
        public IEnumerable<long?> QuizIds { get;  }
        public IEnumerable<Guid?> CommentsIds { get;  }
        public IEnumerable<Guid?> RepliesIds { get;  }
        public IEnumerable<Guid?> DiscussionIds { get;  }

    }



}
