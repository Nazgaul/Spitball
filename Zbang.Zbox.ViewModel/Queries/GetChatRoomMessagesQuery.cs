using System;

namespace Zbang.Zbox.ViewModel.Queries
{
   public  class GetChatRoomMessagesQuery
    {
       public GetChatRoomMessagesQuery(Guid id)
       {
           Id = id;
       }

       public Guid Id { get; private set; }
    }
}
