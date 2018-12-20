using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Application.Enum
{
    public enum JobRequestSort
    {
        [ResourceDescription(typeof(EnumResources), "SearchRequestSortRelevance")]
        Relevance,
        [ResourceDescription(typeof(EnumResources), "SearchRequestSortDate")]
        Date,
    }
}