﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Search.Interfaces;
using Microsoft.Azure.Search;
using QuestionSubject = Cloudents.Common.QuestionSubject;

namespace Cloudents.Search.Entities
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global",Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]
    public class Question : ISearchObject
    {
        public Question(long id, DateTime dateTime, string text, string country, string language,
            QuestionSubject subject, QuestionFilter state)
        {
            Id = id.ToString();
            DateTime = dateTime;
            Text = text;
            Prefix = new[] { text }.Union(subject.GetEnumLocalizationAllValues()).ToArray();
            Country = country.ToUpperInvariant();
            Language = language.ToLowerInvariant();
            Subject = subject;
            State = state;
        }

        public Question()
        {
            
        }

        [Key]
        public string Id { get; set; } //key readonly

        [IsSortable,IsFilterable]
        public DateTime? DateTime { get;  set; } //readonly

        [IsSearchable]
        public string Text { get;  set; } //search readonly

        public string[] Prefix { get;  set; } //search



        public string Country { get;  set; }
        public string Language { get;  set; }


        public QuestionSubject? Subject { get;  set; } // facetable readonly
        public QuestionFilter? State { get; set; }


        //[Obsolete]
        //public string UserName { get; set; }

        //[Obsolete]
        //public string UserName { get; set; }
        //[Obsolete]
        //public string UserName { get; set; }
        //[Obsolete]
        //public string UserName { get; set; }
        //[Obsolete]
        //public string UserName { get; set; }
        //[Obsolete]
        //public string UserName { get; set; }
        //[Obsolete]
        //public string UserName { get; set; }
        //[Obsolete]
        //public string UserName { get; set; }
    }
}