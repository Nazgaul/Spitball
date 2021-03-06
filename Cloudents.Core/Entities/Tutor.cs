﻿using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Exceptions;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class Tutor : Entity<long>
    {
        public Tutor(User user) : this()
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            State = ItemState.Pending;
            Created = DateTime.UtcNow;
            AddEvent(new TutorCreatedEvent(this));
            Courses = new List<Course>();
        }


        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected Tutor()
        {
        }

        public virtual string? Paragraph2 { get; protected set; }

        public virtual string? Title { get; protected set; }

        public virtual string? Paragraph3 { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual Money? SubscriptionPrice { get; protected set; }

        public virtual IList<Course> Courses { get; protected set; }

        private readonly ICollection<UserCoupon> _userCoupons =new List<UserCoupon>();
       
        public virtual IEnumerable<UserCoupon> UserCoupons => _userCoupons;
        //private readonly ISet<UserCoupon> _userCoupons = new HashSet<UserCoupon>();

        //public virtual IEnumerable<UserCoupon> UserCoupons => _userCoupons;

        
        private readonly ISet<Coupon> _coupons = new HashSet<Coupon>();

        public virtual ICollection<Coupon> Coupons => _coupons;


        protected internal virtual ICollection<ChatRoom> ChatRooms { get; set; }




        public virtual void ChangeSubscriptionPrice(double price)
        {
            if (User.SbCountry == null)
            {
                throw new ArgumentException("Tutor cannot have null country");
            }
            var currency = User.SbCountry.RegionInfo.ISOCurrencySymbol;
            var money = new Money(price, currency);
            SubscriptionPrice = money;
            AddEvent(new TutorSubscriptionEvent(Id));
        }

        public virtual bool HasSubscription()
        {
            return SubscriptionPrice != null;
        }

        public virtual void UpdateSettings(string? shortParagraph, string? title, string? paragraph)
        {

            Paragraph2 = shortParagraph;
            Paragraph3 = paragraph;
            Title = title;
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
        public virtual string? SellerKey { get; protected set; }

        public virtual void SetSellerKey(string key)
        {
            SellerKey = key ?? throw new ArgumentNullException(nameof(key));
            foreach (var course in Courses.Where(w => w.State == ItemState.Pending))
            {

                course.State = ItemState.Ok;
            }
        }

        public virtual ItemState State { get; protected set; }
        public virtual DateTime Created { get; protected set; }

        protected internal virtual byte[] Version { get; set; }
        public virtual void AddReview(string? review, float rate, User user)
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


        public virtual Course AddCourse(Course course)
        {
            Courses.Insert(0, course);
            AddEvent(new NewCourseEvent(course));
            return course;
        }


        public virtual void RemoveCourse(long courseId)
        {
            var course = Courses.First(f => f.Id == courseId);
            Courses.Remove(course);
            AddEvent(new DeleteCourseEvent(course));
        }

        public virtual void AddCoupon(string code, CouponType couponType, decimal value, DateTime? expiration)
        {
            var coupon = new Coupon(code, couponType, this, value, expiration);

            var result = _coupons.Add(coupon);
            if (!result)
            {
                throw new DuplicateRowException();
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