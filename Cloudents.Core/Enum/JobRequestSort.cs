using Cloudents.Application.Enum.Resources;
using Cloudents.Common.Attributes;

namespace Cloudents.Application.Enum
{
    public enum JobRequestSort
    {
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortRelevance")]
        Relevance,
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortDate")]
        Date,
    }
}