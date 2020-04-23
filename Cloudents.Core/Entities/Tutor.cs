using Cloudents.Core.Enum;
using Cloudents.Core.Event;
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
        public const int MinimumPrice = 10;
        public Tutor(string bio, User user, decimal? price) : this()
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            State = ItemState.Pending;
            Created = DateTime.UtcNow;
            Bio = bio;
            //User.SetUserType(UserType.Teacher);
            //Country country = user.Country;
            if (user.SbCountry == Country.India)
            {
                Price = new TutorPrice(100, 0);
            }
            else
            {
                if (price is null)
                {
                    throw new ArgumentException("Price is null");
                }
                Price = new TutorPrice(price.Value);
            }

            //SubsidizedPrice = subsidizedPrice;


        }


        protected Tutor()
        {
            //Price = Price ?? new TutorPrice();
        }
        public virtual string Bio { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual TutorPrice Price { get; protected set; }

        protected internal virtual ICollection<UserCoupon> UserCoupons { get; set; }
        protected internal virtual ICollection<Coupon> Coupons { get; set; }

        public virtual void UpdateSettings(string bio, decimal? price)
        {
            if (price.HasValue)
            {
                if (price < MinimumPrice || price > MaximumPrice) throw new ArgumentOutOfRangeException(nameof(price));

                Price = new TutorPrice(price.Value);
            }
            Bio = bio;
            AddEvent(new UpdateTutorSettingsEvent(Id));
        }

        public virtual void AdminChangePrice(decimal newPrice)
        {
            if (newPrice < 0) throw new ArgumentOutOfRangeException(nameof(newPrice));

            Price = new TutorPrice(newPrice);
            AddEvent(new UpdateTutorSettingsEvent(Id));
        }

        public virtual void Approve()
        {
            if (State != ItemState.Pending) return;
            State = ItemState.Ok;
            AddEvent(new TutorApprovedEvent(Id));

        }

        public virtual void Delete()
        {
            User.SetUserType(UserType.UniversityStudent);
            AddEvent(new TutorDeletedEvent(Id));
        }

        public virtual void Suspend()
        {
            if (State != ItemState.Ok) return;
            State = ItemState.Flagged;
            AddEvent(new TutorSuspendedEvent(Id));
        }

        public virtual void UnSuspend()
        {
            if (State != ItemState.Flagged) return;
            State = ItemState.Ok;
            AddEvent(new TutorUnSuspendedEvent(Id));
        }
        private readonly ICollection<TutorReview> _reviews = new List<TutorReview>();

        public virtual IEnumerable<TutorReview> Reviews => _reviews;

        protected internal virtual ICollection<StudyRoom> StudyRooms { get; set; }

        protected internal virtual ICollection<Lead> Leads { get; set; }
        public virtual string? SellerKey { get; set; }
        public virtual ItemState State { get; protected set; }
        public virtual DateTime Created { get; protected set; }

        protected internal virtual byte[] Version { get; set; }
        public virtual void AddReview(string review, float rate, User user)
        {
            if (Id == user.Id)
            {
                throw new ArgumentException("user can't review it self");
            }
            var newReview = new TutorReview(review, rate, user, this);

            _reviews.Add(newReview);

        }

        public virtual void UpdateTutorHours(IEnumerable<TutorAvailabilitySlot> tutorHours)
        {
            var newSet = new HashSet<TutorHours>(tutorHours.Select(s => new TutorHours(this, s)));
            _tutorHours.IntersectWith(newSet);

            foreach (var hours in newSet)
            {
                _tutorHours.Add(hours);
            }

        }



        // ReSharper disable once InconsistentNaming Need to have due to mapping
        private readonly ISet<TutorCalendar> _calendars = new HashSet<TutorCalendar>();
        public virtual IEnumerable<TutorCalendar> Calendars => _calendars;


        private readonly ISet<TutorHours> _tutorHours = new HashSet<TutorHours>();
        public virtual IEnumerable<TutorHours> TutorHours => _tutorHours;
        public virtual bool IsShownHomePage { get; protected set; }


        protected internal virtual ICollection<AdminTutor> AdminUsers { get; set; }

        public virtual AdminTutor AdminUser
        {
            get => AdminUsers.SingleOrDefault();
            set
            {
                AdminUsers.Clear();
                AdminUsers.Add(value);
            }
        }


        public virtual void UpdateCalendar(IEnumerable<GoogleCalendar> calendars)
        {
            var newSet = new HashSet<TutorCalendar>(calendars.Select(s => new TutorCalendar(s, this)));
            _calendars.IntersectWith(newSet);
            foreach (var hours in newSet)
            {
                _calendars.Add(hours);
            }
        }
    }
}