using System.Collections.Generic;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    public class AiDto
    {
        public AiDto(AiIntent intent, KeyValuePair<string, string>? searchType,
            string university, IList<string> subject,
            string location, string course, string isbn)
        {
            Intent = intent;
            SearchType = searchType;
            University = university;
            Subject = subject;
            Location = location;
            Course = course;
            Isbn = isbn;
        }

        public AiIntent Intent { get; }
        public KeyValuePair<string, string>? SearchType { get; }

        public string University { get; }
        public string Course { get; }
        public string Isbn { get; }
        public IList<string> Subject { get; }
        public string Location { get; }
    }


   

   
}
