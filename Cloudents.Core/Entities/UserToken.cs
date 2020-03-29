using System;

namespace Cloudents.Core.Entities
{
    public class UserPayPalToken : Entity<Guid>
    {
        public UserPayPalToken(string orderId, string authorizationId, decimal amount, StudyRoom studyRoom)
        {
            //User = user;
            //TokenId = tokenId ?? throw new ArgumentNullException(nameof(tokenId));

            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            AuthorizationId = authorizationId ?? throw new ArgumentNullException(nameof(authorizationId));

            Created = Updated = DateTime.UtcNow;
            State = UserTokenState.NotUsed;
            Amount = amount;
            StudyRoom = studyRoom;
        }

        protected UserPayPalToken()
        { 
        }

       // public virtual User User { get; protected set; }
        public virtual string OrderId { get; protected set; }
        public virtual string AuthorizationId { get; protected set; }

        public virtual DateTime Created { get; }

        public virtual decimal Amount { get;  }

        public virtual StudyRoom StudyRoom { get; set; }

        public virtual UserTokenState State { get; protected set; }
        public virtual DateTime Updated { get; set; }

        public virtual void ChangeToUsedState()
        {
            State = UserTokenState.Used;
            Updated = DateTime.UtcNow;
            
        }
    }

    public enum UserTokenState
    {
        NotUsed,
       // Reserved,
        Used
    }
}
