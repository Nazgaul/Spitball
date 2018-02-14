﻿using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class AiDto
    {
        private readonly List<string> _list = new List<string>();

        public AiDto(AiIntent intent, KeyValuePair<string, string>? searchType,
            string university, IEnumerable<string> subject,
            string location, string course, string isbn, string sentence)
        {
            Intent = intent;
            SearchType = searchType;
            University = university;
            _list.AddRange(subject);
            Location = location;
            Course = course;
            Isbn = isbn;
            Sentence = sentence;
        }

        public AiIntent Intent { get; }
        public KeyValuePair<string, string>? SearchType { get; }

        public string University { get; }
        public string Course { get; }
        public string Isbn { get; }
        public IList<string> Subject => new List<string>(_list);
        public string Location { get; }

        public string Sentence { get; }
    }
}
