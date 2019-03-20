using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum JobRequestSort
    {
        [ResourceDescription(typeof(EnumResources), "SearchRequestSortRelevance")]
        Relevance,
        [ResourceDescription(typeof(EnumResources), "SearchRequestSortDate")]
        Date
    }
}