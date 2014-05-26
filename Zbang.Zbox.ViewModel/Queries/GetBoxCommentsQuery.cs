﻿namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxCommentsQuery : QueryBase
    {
        public GetBoxCommentsQuery(long boxId, long userId)
            : base(userId)
        {
            BoxId = boxId;            
        }

        
        public long BoxId { get; private set; }





      
    }
}
