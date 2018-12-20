using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Application.Enum
{
    public enum TutorRequestSort
    {

        [ResourceDescription(typeof(EnumResources), "SearchRequestSortRelevance")]
        Relevance,
        [ResourceDescription(typeof(EnumResources), "TutorSortPrice")]
        Price,
    }
}