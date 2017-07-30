using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.Models
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public int? Page { get; set; }

        public string University { get; set; }
        public string Course { get; set; }
    }
}