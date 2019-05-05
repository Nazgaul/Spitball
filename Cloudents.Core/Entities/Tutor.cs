using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public class Tutor : Entity<long>
    {
        public const int MaximumPrice = 214748;
        public Tutor(string bio, RegularUser user, decimal price) :this()
        {
            if (price <= 0 || price > MaximumPrice) throw new ArgumentOutOfRangeException(nameof(price));
            
            User = user;
            Bio = bio;
            Price = price;
        }

        protected Tutor()
        {
        }
        public virtual string Bio { get;protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual RegularUser User { get; protected set; }

        public virtual void UpdateSettings(string bio, decimal price)
        {
            if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));
            Price = price;
            Bio = bio;
        }

        //public virtual bool Equals(Tutor other)
        //{
        //    if (ReferenceEquals(null, other)) return false;
        //    if (ReferenceEquals(this, other)) return true;
        //    return User.Equals(other.User);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((Tutor) obj);
        //}

        //public override int GetHashCode()
        //{
        //    var t =  User.GetHashCode();
        //    return t;
        //}

        //public static bool operator ==(Tutor left, Tutor right)
        //{
        //    return Equals(left, right);
        //}

        //public static bool operator !=(Tutor left, Tutor right)
        //{
        //    return !Equals(left, right);
        //}
        private readonly ICollection<TutorReview> _reviews = new HashSet<TutorReview>();

        public virtual IReadOnlyCollection<TutorReview> Reviews => _reviews.ToList();
        public virtual string SellerKey { get; set; }

        //protected internal  virtual ICollection<TutorReview> Reviews { get; protected set; }
     


        public virtual void AddReview(string review, float rate, RegularUser user, StudyRoom room)
        {
            var newReview = new TutorReview(review,rate,user,this, room);

            _reviews.Add(newReview);
        }
    }
}