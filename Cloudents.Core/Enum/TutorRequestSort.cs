using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

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