using System;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetFeedLikesQuery
    {
        public GetFeedLikesQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
