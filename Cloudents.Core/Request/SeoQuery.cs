using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Request
{
    public class SeoQuery
    {
        public SeoQuery(int page)
        {
            Page = page;
        }

        public int Page { get;  }
    }
    //public class DocumentSeoQuery : SeoQuery
    //{
    //    public DocumentSeoQuery(int page) : base(page)
    //    {
    //    }
    //}
}
