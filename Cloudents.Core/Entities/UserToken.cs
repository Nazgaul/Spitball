using System;

namespace Cloudents.Core.Entities
{
    public class UserToken : Entity<Guid>
    {
        public UserToken(User user, string tokenId)
        {
            User = user;
            TokenId = tokenId;
            Created = DateTime.UtcNow;
        }
        protected UserToken()
        { 
        
        }
        public virtual User User { get; protected set; }
        public virtual string TokenId { get; protected set; }
        public virtual DateTime Created { get; }
    }
}
