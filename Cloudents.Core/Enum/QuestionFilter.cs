﻿using Cloudents.Common.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Application.Enum
{
    public enum QuestionFilter
    {
        All,
        [PublicValue]
        [ResourceDescription(typeof(EnumResources), "QuestionFilterUnanswered")]
        Unanswered,
        [PublicValue]
        [ResourceDescription(typeof(EnumResources), "QuestionFilterAnswered")]
        Answered,
        [PublicValue]
        [ResourceDescription(typeof(EnumResources), "QuestionFilterSold")]
        Sold
    }
}