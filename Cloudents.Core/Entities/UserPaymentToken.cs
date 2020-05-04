using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public class UserPaymentToken : Entity<Guid>
    {
        public UserPaymentToken(string orderId, string authorizationId, decimal amount, StudyRoom studyRoom)
        {
            //User = user;
            //TokenId = tokenId ?? throw new ArgumentNullException(nameof(tokenId));

            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            AuthorizationId = authorizationId ?? throw new ArgumentNullException(nameof(authorizationId));

            Created = Updated = DateTime.UtcNow;
            State = PaymentTokenState.NotUsed;
            Amount = amount;
            StudyRoom = studyRoom;
        }

        protected UserPaymentToken()
        { 

        }

        // public virtual User User { get; protected set; }
        public virtual string OrderId { get; protected set; }
        public virtual string AuthorizationId { get; protected set; }

        public virtual DateTime Created { get; }

        public virtual decimal Amount { get;  }

        public virtual StudyRoom StudyRoom { get; set; }

        public virtual PaymentTokenState State { get; protected set; }
        public virtual DateTime Updated { get; set; }

        public virtual void ChangeToUsedState()
        {
            State = PaymentTokenState.Used;
            Updated = DateTime.UtcNow;
            
        }
    }
}