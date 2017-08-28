using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Queries
{
   public  class GetChatRoomMessagesQuery: IPagedQuery2
    {
       public GetChatRoomMessagesQuery(Guid? id, IEnumerable<long> userIds, DateTime? fromTime, int top)
       {
           Id = id;
           UserIds = userIds;
           FromTime = fromTime;
           Top = top;
       }

       public Guid? Id { get; private set; }

       public IEnumerable<long> UserIds { get; private set; }

       public DateTime? FromTime { get; private set; }

       public int Top { get; }
       public int Skip { get; }
    }
}
