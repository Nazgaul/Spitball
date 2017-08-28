using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchItemDirtyQuery
    {
        public SearchItemDirtyQuery(int index, int total, int top)
        {
            Index = index;
            Total = total;
            Top = top;
        }

        public SearchItemDirtyQuery(long itemId)
        {
            Top = 1;
            ItemId = itemId;
        }

        public long? ItemId { get;private set; }
        public int? Top { get; private set; }

        public int? Index { get; private set; }

        public int? Total { get; private set; }
    }
}
