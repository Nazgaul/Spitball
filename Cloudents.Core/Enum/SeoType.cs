using Cloudents.Core.Attributes;

namespace Cloudents.Core.Enum
{
    public enum SeoType
    {
        [Parse(SeoTypeString.Tutor)]
        Static,
        [Parse(SeoTypeString.Document)]
        Document,
        [Parse(SeoTypeString.Question)]
        Question,
        [Parse(SeoTypeString.Tutor)]
        Tutor,
        TutorList
    }

    public static class SeoTypeString
    {
        public const string Document = "Document";
        public const string Question = "Question";
        public const string Tutor = "Tutor";
        public const string Static = "Static";
        public const string TutorList = "TutorList";
    }
}
