namespace Cloudents.Infrastructure.Search
{
    public sealed class CustomApiKey
    {
        public string Key { get; }

        public CustomApiKey(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return Key;
        }

        public static readonly CustomApiKey Documents = new CustomApiKey("003398766241900814674:a0wbnwb-nyc");
        public static readonly CustomApiKey Flashcard = new CustomApiKey("005511487202570484797:dpt4lypgxoq");
        public static readonly CustomApiKey AskQuestion = new CustomApiKey("partner-pub-1215688692145777:pwibhr1onij");
    }
}