using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor",Justification = "nhibernate")]
    public class Follow : Entity<Guid>, IEquatable<Follow>
    {
        public Follow(User followed, User follower)
        {
            if (followed.Id == follower.Id)
            {
                throw new ArgumentException();
            }
            Followed = followed;
            Follower = follower;
            Created = DateTime.UtcNow;
        }

        [SuppressMessage("ReSharper", "CS8618",Justification = "Nhibernate proxy")]
        protected Follow() 
        { }
        public virtual User Followed { get; protected set; }
        public virtual User Follower { get; protected set; }
        public virtual DateTime Created { get; }

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
