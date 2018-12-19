using Cloudents.Application.Enum.Resources;
using Cloudents.Common.Attributes;

namespace Cloudents.Application.Enum
{
    public enum SearchRequestSort
    {
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortDate")]

        Date,
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortRelevance")]

        Relevance
    }
}
