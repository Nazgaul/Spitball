using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Queries
{
   public  class GetChatRoomMessagesQuery
    {
       public GetChatRoomMessagesQuery(Guid? id, IEnumerable<long> userIds)
       {
           Id = id;
           UserIds = userIds;
       }

       public Guid? Id { get; private set; }


       public IEnumerable<long> UserIds { get; private set; }
    }
}
