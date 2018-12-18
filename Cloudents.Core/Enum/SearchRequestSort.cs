using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum SearchRequestSort
    {
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortDate")]

        Date,
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortRelevance")]

        Relevance
    }
}
