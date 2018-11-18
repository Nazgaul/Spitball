using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum DocumentType
    {
        None,
        [ResourceDescription(typeof(EnumResources), "DocumentTypeLecture")]
        [PublicValue]
        Lecture,
        [ResourceDescription(typeof(EnumResources), "DocumentTypeTextbook")]
        [PublicValue]
        Textbook,
        [ResourceDescription(typeof(EnumResources), "DocumentTypeExam")]
        [PublicValue]
        Exam
    }
}