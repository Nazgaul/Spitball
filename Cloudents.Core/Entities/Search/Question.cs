using System;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    public class Question : ISearchObject
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }


        public string Id { get; set; } //key

        public int AnswerCount { get; set; }
        public DateTime DateTime { get; set; }
        public int FilesCount { get; set; }
        public bool HasCorrectAnswer { get; set; }
        public double Price { get; set; }
        public string Text { get; set; }

        public string Prefix { get; set; }

        public string Country { get; set; }

        public QuestionColor Color { get; set; }

        public int Subject { get; set; } // facetable

       // public string SubjectText { get; set; } // facetable

    }
}