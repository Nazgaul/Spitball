using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class ShareBoxInvitation : Invitation
    {
        ShareBoxInvitation()
        {
        }

        public ShareBoxInvitation(User sender, Box box, Friend friend)
            : base(sender, friend.FriendEmailAddress)
        {
            this.Friend = friend;
            this.Box = box;
        }

        public Friend Friend { get; protected set; }
        public Box Box { get; protected set; }

        public override bool Equals(object obj)
        {
            ShareBoxInvitation other = obj as ShareBoxInvitation;
            if (other == null)
                return false;

            return Sender.Equals(other.Sender) && Box.Equals(other.Box) && Friend.Equals(other.Friend) && base.Equals(other);
        }

        public override int GetHashCode()
        {
            int result = 1;
            const int prime = 31;

            result = prime * result + base.GetHashCode();
            result += prime * result + Sender.GetHashCode();
            result += prime * result + Box.GetHashCode();
            result += prime * result + Friend.GetHashCode();

            return result;
        }
    }
}
