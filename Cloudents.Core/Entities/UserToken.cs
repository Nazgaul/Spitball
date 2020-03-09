﻿using System;

namespace Cloudents.Core.Entities
{
    public class UserPayPalToken : Entity<Guid>
    {
        public UserPayPalToken(string tokenId)
        {
            //User = user;
            TokenId = tokenId ?? throw new ArgumentNullException(nameof(tokenId));
            Created = DateTime.UtcNow;
            State = UserTokenState.NotUsed;
        }
        protected UserPayPalToken()
        { 
        }

       // public virtual User User { get; protected set; }
        public virtual string TokenId { get; protected set; }
        public virtual DateTime Created { get; }

        //public virtual StudyRoom StudyRoomId { get; set; }

        public virtual UserTokenState State { get; set; }

        


    }

    public enum UserTokenState
    {
        NotUsed,
        Reserved,
        Used
    }
}
