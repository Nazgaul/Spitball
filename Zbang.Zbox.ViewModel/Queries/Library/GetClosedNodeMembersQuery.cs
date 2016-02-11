using System;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetClosedNodeMembersQuery : IUserQuery
    {
        public GetClosedNodeMembersQuery(long userId, Guid libraryId)
        {
            LibraryId = libraryId;
            UserId = userId;
        }

        public long UserId
        {
            get;
            private set;
        }

        public Guid LibraryId { get; private set; }
    }
}
