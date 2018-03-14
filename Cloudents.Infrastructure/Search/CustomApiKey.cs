using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core;

namespace Cloudents.Infrastructure.Search
{
    [Serializable]
    public sealed class CustomApiKey
    {
        public string Key { get; }

        public IReadOnlyDictionary<string, PrioritySource> Priority { get; }

        private CustomApiKey(string key, IReadOnlyDictionary<string, PrioritySource> priority)
        {
            Key = key;
            Priority = priority;
        }

        

        public static readonly CustomApiKey Documents = new CustomApiKey("2506829495", PrioritySource.DocumentPriority);
        public static readonly CustomApiKey Flashcard = new CustomApiKey("3768889099", PrioritySource.FlashcardPriority);
        public static readonly CustomApiKey AskQuestion = new CustomApiKey("664922931", PrioritySource.AskPriority);
    }
}