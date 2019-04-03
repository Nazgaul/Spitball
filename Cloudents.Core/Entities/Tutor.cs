
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public class Tutor : Entity<long>
    {
        public Tutor(string bio, RegularUser user) :this()
        {
            User = user;
            Bio = bio;
            Price = 50;
        }

        protected Tutor()
        {
        }
        public virtual string Bio { get; set; }
        public virtual decimal Price { get; protected set; }
        public virtual RegularUser User { get; protected set; }

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

        //protected internal  virtual ICollection<TutorReview> Reviews { get; protected set; }
     


        public virtual void AddReview(string review, float rate, RegularUser user)
        {
            var newReview = new TutorReview(review,rate,user,this);

            _reviews.Add(newReview);
        }
    }
}