using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.DTOs
{
    public class AIDto
    {
        public AIDto(string intent, KeyValuePair<string, string>? searchType, string course, IEnumerable<string> term)
        {
            Intent = intent;
            SearchType = searchType;
            Course = course;
            Term = term;
        }

        public string Intent { get; private set; }
        public KeyValuePair<string,string>? SearchType { get;  }

        public string Course { get;  }

        public IEnumerable<string> Term { get;  }

        

        //public string Professor { get; set; }



    }
}
