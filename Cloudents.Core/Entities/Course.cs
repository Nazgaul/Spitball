using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Entities
{
    public class Course : Entity<long>
    {
        public Course(string name, Tutor tutor, double price,double? subscriptionPrice, string description)
        {
            Name = name;
            Tutor = tutor;
            State = ItemState.Ok;
            if (tutor.User.SbCountry == Country.Israel && tutor.SellerKey == null)
            {
                State = ItemState.Pending;
            }

            if (tutor.HasSubscription())
            {
                SubscriptionPrice = new Money(subscriptionPrice.GetValueOrDefault(), Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
            }
            Create = DateTime.UtcNow;
            Description = description;
            Price = new Money(price, Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
            
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

        public virtual void AddStudyRoom(BroadCastStudyRoom studyRoom)
        {
            _studyRooms.Add(studyRoom);
        }

        public virtual void AddDocument(Document document)
        {
            _documents.Add(document);
        }


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<Document> _documents = new List<Document>();

        public virtual IEnumerable<Document> Documents => _documents;


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<BroadCastStudyRoom> _studyRooms = new List<BroadCastStudyRoom>();

        public virtual IEnumerable<BroadCastStudyRoom> StudyRooms => _studyRooms;

        public virtual DateTime Create { get; protected set; }

        private readonly ISet<CourseEnrollment> _courseEnrollments = new HashSet<CourseEnrollment>();

        public virtual IEnumerable<CourseEnrollment> CourseEnrollments => _courseEnrollments;

        public virtual void EnrollUser(User user, string? receipt, Money price)
        {
            var courseEnrollment = new CourseEnrollment(user, this, receipt, price);

            var z = _courseEnrollments.Add(courseEnrollment);
        }

    }
}