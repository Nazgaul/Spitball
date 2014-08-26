using System;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetLibraryNodeQuery : QueryBase
    {
        public GetLibraryNodeQuery(long universityId, Guid? parentNode, long userId)
            : base(userId)
        {
            UniversityId = universityId;
            ParentNode = parentNode;
        }
        public long UniversityId { get; private set; }
        public Guid? ParentNode { get; private set; }

        public string LibraryQuery
        {
            get
            {
                return ParentNode.HasValue ? "GetLibraryNodeWithId" : "GetLibraryNode";
            }
        }

    }
}
