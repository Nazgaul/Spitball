using Cloudents.Application.Enum.Resources;
using Cloudents.Common.Attributes;

namespace Cloudents.Application.Enum
{
    public enum TutorRequestSort
    {

        [ResourceDescription(typeof(EnumResourcesOld), "SearchRequestSortRelevance")]
        Relevance,
        [ResourceDescription(typeof(EnumResourcesOld), "TutorSortPrice")]
        Price,
    }
}