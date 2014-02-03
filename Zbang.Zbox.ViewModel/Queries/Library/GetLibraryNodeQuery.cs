using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetLibraryNodeQuery : QueryPagedBase
    {
        public GetLibraryNodeQuery(long universityId, Guid? parentNode, long userId, int pageNumber, OrderBy sort)
            : base(userId, pageNumber)
        {
            UniversityId = universityId;
            ParentNode = parentNode;
            Sort = sort;
        }
        public long UniversityId { get; private set; }
        public Guid? ParentNode { get; private set; }
        public OrderBy Sort { get; private set; }

        public string libraryQuery
        {
            get
            {
                return ParentNode.HasValue ? "GetLibraryNodeWithId" : "GetLibraryNode";
            }
        }

    }
}
