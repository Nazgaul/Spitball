using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ApiViewModel.Queries
{
    public abstract class GetBoxesQueryBase : QueryBase
    {
        protected const string PageQueryPrefix = "Page";
        protected string m_QueryName;

        public GetBoxesQueryBase(long Id, string boxesQueryName)
            : base(Id)
        {
            m_QueryName = boxesQueryName;
        }
        public abstract string QueryName { get; }
    }
}
