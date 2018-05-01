using System;
using System.Collections.Generic;

namespace Cloudents.Core.Read
{
    [Serializable]
    public sealed class CustomApiKey
    {
        public string Key { get; }
        public string DefaultPhrase { get; }

        public IReadOnlyDictionary<string, PrioritySource> Priority { get; }

        private CustomApiKey(string key,
            string defaultPhrase,
            IReadOnlyDictionary<string, PrioritySource> priority)
        {
            Key = key;
            Priority = priority;
            DefaultPhrase = defaultPhrase;
        }

        /// <summary>
        /// Used to build cache key
        /// </summary>
        /// <returns>The key of the custom api</returns>
        public override string ToString()
        {
            return Key;
        }


        public static readonly CustomApiKey Documents = new CustomApiKey("2506829495", "biology", PrioritySource.DocumentPriority);
        public static readonly CustomApiKey Flashcard = new CustomApiKey("3768889099", "accounting", PrioritySource.FlashcardPriority);
        public static readonly CustomApiKey AskQuestion = new CustomApiKey("664922931", "world war 2", PrioritySource.AskPriority);
    }
}