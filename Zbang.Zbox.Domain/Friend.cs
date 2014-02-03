using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{   
    public class Friend
    {
        //Ctor
        public Friend()
        {

        }

        //Properties        
        public virtual int FriendId { get; set; }
        public virtual string FriendEmailAddress { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual bool IsActive { get; set; }

        public override int GetHashCode()
        {
            if (FriendId == 0)
                throw new InvalidOperationException("Friend is not persistent");
            
            return 17 * FriendId;
        }

        public override bool Equals(object obj)
        {
            if (FriendId == 0)
                throw new InvalidOperationException("Friend is not persistent");
            
            Friend friend = obj as Friend;

            if (friend == null)
                return false;

            return FriendId.Equals(friend.FriendId);
        }
    }
}
