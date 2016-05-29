using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Queries
{
   public  class GetChatRoomMessagesQuery: IPagedQuery2
    {
       public GetChatRoomMessagesQuery(Guid? id, IEnumerable<long> userIds, Guid? fromId, int top, int skip)
       {
           Id = id;
           UserIds = userIds;
           FromId = fromId;
           Top = top;
           Skip = skip;
       }

       public Guid? Id { get; private set; }


       public IEnumerable<long> UserIds { get; private set; }

       public Guid? FromId { get; private set; }


       public int Top { get; }
       public int Skip { get; }
    }
}
