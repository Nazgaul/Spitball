using Cloudents.Common.Attributes;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum JobRequestSort
    {
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortRelevance")]
        Relevance,
        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortDate")]
        Date,
    }
}