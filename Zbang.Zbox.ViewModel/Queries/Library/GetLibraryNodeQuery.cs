using System;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetLibraryNodeQuery : QueryBase
    {
        public GetLibraryNodeQuery(long universityId, long? parentNode, long userId)
            : base(userId)
        {
            UniversityId = universityId;
            ParentNode = parentNode;
        }
        public long UniversityId { get; private set; }
        public long? ParentNode { get; private set; }

      

    }
}
