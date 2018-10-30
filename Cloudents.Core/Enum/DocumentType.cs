using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum DocumentType
    {
        None,
        [ResourceDescription(typeof(EnumResources), "DocumentTypeLecture")]
        Lecture,
        [ResourceDescription(typeof(EnumResources), "DocumentTypeTextbook")]
        Textbook,
        [ResourceDescription(typeof(EnumResources), "DocumentTypeExam")]
        Exam
    }
}