using System;

namespace Cloudents.Infrastructure.Search
{
    [Serializable]
    public sealed class CustomApiKey
    {
        public string Key { get; }

        private CustomApiKey(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return Key;
        }

        public static readonly CustomApiKey Documents = new CustomApiKey("2506829495");
        public static readonly CustomApiKey Flashcard = new CustomApiKey("3768889099");
        public static readonly CustomApiKey AskQuestion = new CustomApiKey("664922931");
    }
}