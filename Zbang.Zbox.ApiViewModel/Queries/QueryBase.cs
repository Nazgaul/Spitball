using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ApiViewModel.Queries
{
    public class QueryBase
    {
        public QueryBase(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
