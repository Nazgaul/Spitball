using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class Follow : Entity<Guid>, IEquatable<Follow>
    {
        public Follow(User user, User follower)
        {
            if (user.Id == follower.Id)
            {
                throw new ArgumentException();
            }
            User = user;
            Follower = follower;
            Created = DateTime.UtcNow;
        }

        public Follow(User user, User follower,bool subscriber)
        {
            if (user.Id == follower.Id)
            {
                throw new ArgumentException();
            }
            User = user;
            Follower = follower;
            Subscriber = subscriber;
            Created = DateTime.UtcNow;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected Follow()
        { }
        public virtual User User { get;  }
        public virtual User Follower { get;  }

        public virtual bool? Subscriber { get;  set; }

        public virtual DateTime Created { get; }

        public virtual bool Equals(Follow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(User.Id, other.User.Id) && Equals(Follower.Id, other.Follower.Id);
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
            return (User.Id.GetHashCode() * 101) ^ (Follower.Id.GetHashCode() * 107);
        }
    }
}
