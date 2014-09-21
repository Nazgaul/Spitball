using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxSeoQuery
    {
        public GetBoxSeoQuery(long boxId)
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
    }
}
