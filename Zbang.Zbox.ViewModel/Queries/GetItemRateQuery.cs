using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetItemRateQuery : QueryBase
    {
        public GetItemRateQuery(long userId, long itemId)
            : base(userId)
        {
            ItemId = itemId;
        }
        public long ItemId { get; private set; }
    }
}
