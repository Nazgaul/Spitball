using Cloudents.Core.Attributes;

namespace Cloudents.Core.Enum
{
    public enum SeoType
    {
        Static,
        [Parse(SeoTypeString.Item)]
        Item,
        [Parse(SeoTypeString.Question)]
        Question,
        //[Parse(SeoTypeString.Quiz)]
        //Quiz,
        [Parse(SeoTypeString.Flashcard)]
        Flashcard
    }

    public static class SeoTypeString
    {
        public const string Item = "Item";
        public const string Document = "Document";
        public const string Question = "Question";
        public const string Flashcard = "Flashcard";
    }
}
