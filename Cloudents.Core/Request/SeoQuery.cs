using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Application.Request
{
    public class SeoQuery
    {
        public SeoQuery(int page)
        {
            Page = page;
        }

        public int Page { get;  }

        public const int Steps = 10000;

        public IEnumerable<int> SubPage => Enumerable.Range(0, 50000).Where(i => i % Steps == 0);
    }
    //public class DocumentSeoQuery : SeoQuery
    //{
    //    public DocumentSeoQuery(int page) : base(page)
    //    {
    //    }
    //}
}
