using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

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
