﻿using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Common.Enum
{
    public enum DocumentType : int
    {
        None,
        [PublicValue]
        [ResourceDescription(typeof(EnumResources), "DocumentTypeLecture")]
        Lecture,
        [PublicValue]
        [ResourceDescription(typeof(EnumResources), "DocumentTypeTextbook")]
        Textbook,
        [PublicValue]
        [ResourceDescription(typeof(EnumResources), "DocumentTypeExam")]
        Exam
    }
}