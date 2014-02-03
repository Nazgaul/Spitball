using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ApiViewModel.Queries
{
    public abstract class QueryPagedBase : QueryBase
    {
        public QueryPagedBase(long Id, int pageNumber, int maxResult = 50)
            : base(Id)
        {
            PageNumber = pageNumber;
            MaxResult = maxResult;
        }

        public int PageNumber { get; set; }


        public int MaxResult { get; set; }
    }
}
