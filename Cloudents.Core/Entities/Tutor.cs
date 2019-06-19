using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class Tutor : Entity<long>
    {
        public const int MaximumPrice = 214748;
        public const int MinimumPrice = 50;
        public Tutor(string bio, User user, decimal price) :this()
        {
            
            User = user;
            UpdateSettings(bio, price);
            State = ItemState.Pending;
            Created = DateTime.UtcNow;
        }

        protected Tutor()
        {
        }
        public virtual string Bio { get;protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual User User { get; protected set; }

        public virtual void UpdateSettings(string bio, decimal price)
        {
            if (price < MinimumPrice || price > MaximumPrice) throw new ArgumentOutOfRangeException(nameof(price));
            Price = price;
            Bio = bio;
        }

        public virtual void Approve()
        {
            //TODO: maybe put an event to that
            if (State == ItemState.Pending)
            {
                State = ItemState.Ok;
            }
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
        public virtual ItemState State { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        //protected internal  virtual ICollection<TutorReview> Reviews { get; protected set; }


        public virtual byte[] Version { get; protected set; }
        public virtual void AddReview(string review, float rate, User user, StudyRoom room)
        {
            var newReview = new TutorReview(review,rate,user,this, room);

            _reviews.Add(newReview);
        }
    }
}