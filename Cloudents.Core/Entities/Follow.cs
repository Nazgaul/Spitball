using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor",Justification = "nhibernate")]
    public class Follow : IEquatable<Follow>
    {
        public Follow(BaseUser followed, BaseUser follower)
        {
            if (followed.Id == follower.Id)
            {
                throw new ArgumentException();
            }
            Followed = followed;
            Follower = follower;
        }

        protected Follow() 
        { }
        public virtual Guid Id { get; }
        public virtual BaseUser Followed { get; protected set; }
        public virtual BaseUser Follower { get; protected set; }

        public virtual bool Equals(Follow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Followed.Id, other.Followed.Id) && Equals(Follower.Id, other.Follower.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Follow)obj);
        }

        public override int GetHashCode()
        {
            var t = (Followed.Id.GetHashCode() * 101) ^ (Follower.Id.GetHashCode() * 107);
            return t;
        }
    }
}
