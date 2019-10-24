using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class Tutor : Entity<long>
    {
        public const int MaximumPrice = 214748;
        public const int MinimumPrice = 50;
        public Tutor(string bio, User user, decimal price) : this()
        {

            User = user;
            UpdateSettings(bio, price);
            State = ItemState.Pending;
            Created = DateTime.UtcNow;

        }


        protected Tutor()
        {
        }
        public virtual string Bio { get; protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual User User { get; protected set; }

        public virtual void UpdateSettings(string bio, decimal price)
        {
            if (price < MinimumPrice || price > MaximumPrice) throw new ArgumentOutOfRangeException(nameof(price));
            Price = price;
            Bio = bio;
            AddEvent(new UpdateTutorSettingsEvent(Id));
        }

        public virtual void Approve()
        {
            //TODO: maybe put an event to that
            if (State == ItemState.Pending)
            {
                State = ItemState.Ok;
                AddEvent(new TutorApprovedEvent(Id));
            }

        }

        public virtual void Delete()
        {
            AddEvent(new TutorDeletedEvent(Id));
        }


        public static decimal PriceAfterDiscount(decimal price)
        {
            //var price2 = price - 70;

            //Maybe we can do it with min max
            if (price < 55)
            {
                return price;
            }

            var subsidizingPrice = price - 70;
            if (subsidizingPrice < 55)
            {
                return 55;
            }

            return subsidizingPrice;
        }

        private readonly ICollection<TutorReview> _reviews = new List<TutorReview>();

        public virtual IEnumerable<TutorReview> Reviews => _reviews;

        private readonly ISet<StudyRoom> _studyRooms = new HashSet<StudyRoom>();
        public virtual IEnumerable<StudyRoom> StudyRooms => _studyRooms;

        private readonly ISet<Lead> _leads = new HashSet<Lead>();
        public virtual IEnumerable<Lead> Leads => _leads;
        public virtual string SellerKey { get; set; }
        public virtual ItemState State { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        //protected internal  virtual ICollection<TutorReview> Reviews { get; protected set; }

        public virtual DateTime ManualBoost { get; protected set; }
        public virtual byte[] Version { get; protected set; }
        public virtual void AddReview(string review, float rate, User user)
        {
            var newReview = new TutorReview(review, rate, user, this);

            _reviews.Add(newReview);
          
        }

        public virtual void AddTutorHours(DayOfWeek weekDay, TimeSpan from, TimeSpan to)
        {
            _tutorHours.Add(new TutorHours(this, weekDay, from, to));
        }


        // ReSharper disable once InconsistentNaming Need to have due to mapping
        private readonly ICollection<TutorCalendar> _calendars = new List<TutorCalendar>();
        public virtual IEnumerable<TutorCalendar> Calendars => _calendars;


        private readonly ISet<TutorHours> _tutorHours = new HashSet<TutorHours>();
        public virtual IEnumerable<TutorHours> TutorHours => _tutorHours;


        public virtual void AddCalendar(string id, string name)
        {
            var calendar = new TutorCalendar(id, name, this);
            _calendars.Add(calendar);
        }
    }

    public class TutorCalendar : Entity<Guid>
    {
        public TutorCalendar([NotNull] string googleId, [NotNull] string name, Tutor tutor)
        {
            GoogleId = googleId ?? throw new ArgumentNullException(nameof(googleId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tutor = tutor;
        }

        protected TutorCalendar()
        {
        }


        public virtual string GoogleId { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual Tutor Tutor { get; protected set; }


        //protected bool Equals(TutorCalendar other)
        //{
        //    return base.Equals(other) && GoogleId == other.GoogleId && Tutor.Id.Equals(other.Tutor.Id);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((TutorCalendar)obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int hashCode = base.GetHashCode();
        //        hashCode = (hashCode * 397) ^ GoogleId.GetHashCode();
        //        hashCode = (hashCode * 397) ^ Tutor.Id.GetHashCode();
        //        return hashCode;
        //    }
        //}
        protected bool Equals(TutorCalendar other)
        {
            return string.Equals(GoogleId, other.GoogleId, StringComparison.OrdinalIgnoreCase) && Tutor.Id.Equals(other.Tutor.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TutorCalendar)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {

                var hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(GoogleId);
                hashCode = (hashCode * 397) ^ Tutor.Id.GetHashCode();
                return hashCode;
            }
        }
    }
}