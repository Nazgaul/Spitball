using Cloudents.Application.Enum.Resources;
using Cloudents.Common.Attributes;

namespace Cloudents.Application.Enum
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