using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public class Course : Entity<long>
    {
        public Course(string name, Tutor tutor)
        {
            Name = name;
            Tutor = tutor;
            State = ItemState.Ok;
            Price = new Money(0d, Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
        }

        protected Course()
        {
        }

        public virtual string Name { get; set; }

        public virtual Tutor Tutor { get; set; }

        public virtual int Position { get; }

        public virtual Money Price { get; set; }
        public virtual Money? SubscriptionPrice { get; protected set; }

        public virtual string? Description { get; set; }

        public virtual ItemState State { get; set; }

        public virtual void SetInitPrice()
        {

            var amountInPoints = Documents.Where(w => w.DocumentPrice.Type == PriceType.HasPrice).Sum(d => d.DocumentPrice.Price);
            var fiatValue = new Money(amountInPoints * Tutor.User.SbCountry.ConversationRate, Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);

            var fiatStudyRoomCheck = StudyRooms.Select(s => s.Price).GroupBy(g => g.Currency).Select(s => new
            {
                c = s.Key,
                v = s.DefaultIfEmpty().Aggregate((l, r) => l + r)
            });

            if (fiatStudyRoomCheck.Count() > 1)
            {
                throw new ArgumentException();
            }

            Description = StudyRooms.FirstOrDefault(f => f.Description != null)?.Description;
            var fiatStudyRoom = StudyRooms.Select(s => s.Price).DefaultIfEmpty().Aggregate((l, r) => l + r);

            Price = fiatValue + fiatStudyRoom;
            if (Tutor.HasSubscription())
            {
                SubscriptionPrice = new Money(0d, Price.Currency);
            }
        }


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<Document> _documents = new List<Document>();

        public virtual IEnumerable<Document> Documents => _documents;


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<BroadCastStudyRoom> _studyRooms = new List<BroadCastStudyRoom>();

        public virtual IEnumerable<BroadCastStudyRoom> StudyRooms => _studyRooms;


        private readonly ISet<CourseEnrollment> _courseEnrollments = new HashSet<CourseEnrollment>();

        public virtual IEnumerable<CourseEnrollment> CourseEnrollments => _courseEnrollments;

        public virtual void EnrollUser(User user, string? receipt, Money price)
        {
            var courseEnrollment = new CourseEnrollment(user, this, receipt, price);

            _courseEnrollments.Add(courseEnrollment);
        }

    }
}