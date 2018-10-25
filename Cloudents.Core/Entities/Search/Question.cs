using System;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    public class Question : ISearchObject
    {
        public long UserId { get; set; } //readonly
        public string UserName { get; set; } //readonly
        public string UserImage { get; set; } //readonly


        public string Id { get; set; } //key readonly

        public int AnswerCount { get; set; } //readonly
        public DateTime DateTime { get; set; } //readonly
        public int FilesCount { get; set; } //readonly 
        public bool HasCorrectAnswer { get; set; } //readonly
        public double Price { get; set; } //readonly
        public string Text { get; set; } //search readonly

        public string[] Prefix { get; set; } //search

        public string Country { get; set; }
        public string UniversityId { get; set; }
        public string Language { get; set; }

        public QuestionColor Color { get; set; } //readonly

        public QuestionSubject Subject { get; set; } // facetable readonly
        public QuestionFilter State { get; set; }

        // public string SubjectText { get; set; } // facetable

    }
}