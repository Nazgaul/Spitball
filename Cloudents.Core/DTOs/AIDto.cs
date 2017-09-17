using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.DTOs
{
    public class AIDto
    {
        public AIDto(AIIntent intent, KeyValuePair<string, string>? searchType, string course, IList<string> term)
        {
            Intent = intent;
            SearchType = searchType;
            Course = course;
            Term = term;
        }

        public AIIntent Intent { get; }
        public KeyValuePair<string,string>? SearchType { get;  }

        public string Course { get;  }

        public IList<string> Term { get;  }
    }
}
