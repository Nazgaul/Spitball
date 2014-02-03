using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchCommentQuery :SearchQueryBase
    {
        public SearchCommentQuery(long Id, string searchText, int pageNumber)
            : base(Id, searchText, pageNumber)
        {
        }
        public override string QueryName
        {
            get { return "SearchComment"; }
        }
    }
}
