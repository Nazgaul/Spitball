using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetBoxQuestionsQuery : QueryBase
    {
        public GetBoxQuestionsQuery(long boxId, long userId)
            : base(userId)
        {
            BoxId = boxId;
        }


        public long BoxId { get; private set; }

    }
}
