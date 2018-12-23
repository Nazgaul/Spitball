using Cloudents.Common.Attributes;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum QuestionFilter
    {
        All,
        [PublicValue]
        [ResourceDescription(typeof(EnumResourcesOld), "QuestionFilterUnanswered")]
        Unanswered,
        [PublicValue]
        [ResourceDescription(typeof(EnumResourcesOld), "QuestionFilterAnswered")]
        Answered,
        [PublicValue]
        [ResourceDescription(typeof(EnumResourcesOld), "QuestionFilterSold")]
        Sold
    }
}