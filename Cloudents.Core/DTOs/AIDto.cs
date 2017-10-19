using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class AIDto
    {
        public AIDto(AiIntent intent, KeyValuePair<string, string>? searchType, string course, IList<string> term)
        {
            Intent = intent;
            SearchType = searchType;
            Course = course;
            Term = term;
        }

        public AiIntent Intent { get; }
        public KeyValuePair<string,string>? SearchType { get;  }

        public string Course { get;  }

        public IList<string> Term { get;  }
    }
}
