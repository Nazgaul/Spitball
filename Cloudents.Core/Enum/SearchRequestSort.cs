using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Application.Enum
{
    public enum SearchRequestSort
    {
        [ResourceDescription(typeof(EnumResources), "SearchRequestSortDate")]

        Date,
        [ResourceDescription(typeof(EnumResources), "SearchRequestSortRelevance")]

        Relevance
    }
}
