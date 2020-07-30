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

        public virtual void SetInitPrice()
        {

            var amountInPoints = Documents.Where(w => w.DocumentPrice?.Type == PriceType.HasPrice).Sum(d => d.DocumentPrice.Price);
            var fiatValue = new Money(amountInPoints * Tutor.User.SbCountry.ConversationRate, Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);

            var fiatStudyRoomCheck = StudyRooms.Select(s => s.Price).GroupBy(g => g.Currency).Select(s => new
            {
                c = s.Key,
                v = s.DefaultIfEmpty().Aggregate((l, r) => l + r)
            });

            if (fiatStudyRoomCheck.Count(c => c.v.Cents > 0) > 1)
            {
                throw new ArgumentException();
            }

            Description = StudyRooms.FirstOrDefault(f => f.Description != null)?.Description;
            var fiatStudyRoom = StudyRooms.Select(s => s.Price).DefaultIfEmpty().Aggregate((l, r) => l + r);

            Price = fiatValue + fiatStudyRoom;
            if (Tutor.HasSubscription())
            {
                SubscriptionPrice = new Money(0d, Price.Currency);
                if (Price.Cents == 0)
                {
                    Price = Price.ChangePrice(200);
                }
            }

            if (Create == default)
            {
                Create = DateTime.UtcNow;
            }

            if (Price.Amount > 0 && Price.Amount < 5 && Tutor.User.SbCountry == Country.Israel)
            {
                Price = new Money(5d, "ILS");
            }



            if (Tutor.User.SbCountry == Country.India)
            {
                State = ItemState.Pending;
            }
            if (Tutor.User.SbCountry == Country.UnitedStates)
            {
                State = ItemState.Ok;
            }
            if (Tutor.User.SbCountry == Country.Israel && Tutor.SellerKey == null && Price.Amount > 0)
            {
                State = ItemState.Pending;
            }

            if (Price.Cents == 0)
            {
                State = ItemState.Ok;
            }
            var purchasedDocumentsUsers = Documents.SelectMany(s => s.Transactions).Where(w => w.Type == TransactionType.Spent).Select(s => s.User);

            foreach (var purchasedDocumentsUser in purchasedDocumentsUsers.DistinctBy(x => x.Id))
            {
                EnrollUser(purchasedDocumentsUser, "Old Data", Price.ChangePrice(0));
            }

            if (Documents.Count() == 0 && StudyRooms.All(a => a.BroadcastTime < DateTime.UtcNow && a.Schedule == null))
            {
                State = ItemState.Deleted;
            }

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