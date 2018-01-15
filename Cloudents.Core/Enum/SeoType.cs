namespace Cloudents.Core.Enum
{
    public enum SeoType
    {
        //Static,
        [Parse(SeoTypeString.Item)]
        Item,
        [Parse(SeoTypeString.Quiz)]
        Quiz,
        [Parse(SeoTypeString.Flashcard)]
        Flashcard
    }

    public static class SeoTypeString
    {
        public const string Item = "Item";
        public const string Flashcard = "Flashcard";
        public const string Quiz = "Quiz";
    }
}
