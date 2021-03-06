﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Entities
{
    public class Course : Entity<long>
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhiberate proxy")]
        public Course(string name, Tutor tutor, double price,
            double? subscriptionPrice, string description, DateTime? startTime, bool isPublish)
        {
            Name = name;
            Tutor = tutor ?? throw new ArgumentNullException(nameof(tutor));
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative");
            }

            if (subscriptionPrice.HasValue && subscriptionPrice.Value < 0)
            {
                throw new ArgumentException("Subscription price cannot be negative");
            }
            State = isPublish ? ItemState.Ok : ItemState.Pending;

            if (tutor.User.SbCountry == Country.Israel)
            {
                if (price > 0 && price < 5)
                {
                    throw new ArgumentException("Cant have course which costs less then 5 shekel");
                }
                if (tutor.SellerKey == null && price > 0)
                {
                    State = ItemState.Pending;
                }

            }

            if (tutor.HasSubscription())
            {
                SubscriptionPrice = new Money(subscriptionPrice.GetValueOrDefault(), Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
            }
            DomainTime = new DomainTimeStamp();
            Description = description;
            StartTime = startTime ?? DateTime.UtcNow;
            Price = new Money(price, Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
            Details = new CourseDetails();

        }

        protected Course()
        {
        }

        public virtual string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                _name = value;
            }
        }

        public virtual Tutor Tutor { get; }

        public virtual int Position { get; }

        public virtual Money Price { get; protected set; }
        public virtual Money? SubscriptionPrice { get; protected set; }

        public virtual void ChangeSubscriptionPrice(double? subscriptionPrice)
        {
            if (Tutor.HasSubscription())
            {
                SubscriptionPrice = new Money(subscriptionPrice.GetValueOrDefault(), Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
            }
        }

        public virtual string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                _description = value;
            }
        }

        public virtual DateTime? StartTime
        {
            get => _startTime;
            set =>
                //if (value == null)
                //{
                //   // _startTime = DateTime.UtcNow;
                //    return;
                //}
                //if (value < DateTime.UtcNow)
                //{
                //    _startTime = DateTime.UtcNow;
                //    return;
                //}
                _startTime = value;
        }


        public virtual ItemState State { get; protected internal set; }

        public virtual void UpdateCourse(bool isPublish, double price)
        {
            State = isPublish ? ItemState.Ok : ItemState.Pending;

            if (Tutor.User.SbCountry == Country.Israel)
            {
                if (price > 0 && price < 5)
                {
                    throw new ArgumentException("Cant have course which costs less then 5 shekel");
                }
                if (Tutor.SellerKey == null && price > 0)
                {
                    State = ItemState.Pending;
                }

            }
            AddEvent(new UpdateCourseEvent(this));
            Price = Price.ChangePrice(price);
        }

        public virtual void AddStudyRoom(BroadCastStudyRoom studyRoom)
        {
            if (_studyRooms.Any(a => a.BroadcastTime == studyRoom.BroadcastTime))
            {
                throw new ArgumentException("Already have a broadcast on that time");
            }
            _studyRooms.Add(studyRoom);
        }

        public virtual void AddDocument(Document document)
        {
            _documents.Add(document);
        }




        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly IList<Document> _documents = new List<Document>();

        public virtual IEnumerable<Document> Documents => _documents;


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ISet<BroadCastStudyRoom> _studyRooms = new HashSet<BroadCastStudyRoom>();

        public virtual IEnumerable<BroadCastStudyRoom> StudyRooms => _studyRooms;

        public virtual DomainTimeStamp DomainTime { get; protected set; }

        public virtual CourseDetails Details { get; protected set; }

        private readonly ISet<CourseEnrollment> _courseEnrollments = new HashSet<CourseEnrollment>();
        private DateTime? _startTime;
        private string _name;
        private string _description;

        public virtual IEnumerable<CourseEnrollment> CourseEnrollments => _courseEnrollments;

        public virtual void EnrollUser(User user, string? receipt, Money price)
        {
            var courseEnrollment = new CourseEnrollment(user, this, receipt, price);

            if (_courseEnrollments.Add(courseEnrollment))
            {
                AddEvent(new CourseEnrollmentEvent(courseEnrollment));
            }

            foreach (var broadCastStudyRoom in _studyRooms)
            {
                broadCastStudyRoom.AddUserToStudyRoom(user);
            }
        }


        public virtual void UpdateStudyRoom(IEnumerable<BroadCastStudyRoom> broadCastStudyRooms)
        {
            var newSet = new HashSet<BroadCastStudyRoom>(broadCastStudyRooms);
            _studyRooms.IntersectWith(newSet);

            foreach (var studyRoom in _studyRooms)
            {
                var updateData = newSet.Single(w => w.BroadcastTime == studyRoom.BroadcastTime);
                studyRoom.Description = updateData.Description;
            }

            foreach (var hours in newSet)
            {
                _studyRooms.Add(hours);
            }

            var studyRoomEarlyBroadcastTime = StudyRooms.DefaultIfEmpty().Min(m => m?.BroadcastTime);
            if (studyRoomEarlyBroadcastTime != null)
            {
                StartTime = studyRoomEarlyBroadcastTime.Value;
            }



        }

        public virtual void RemoveDocument(Document document)
        {
            _documents.Remove(document);
        }

        public virtual int Version { get; protected set; }




        public virtual void UpdateDocument(long? id, string name, bool visible, int index)
        {
            var oldIndex = _documents.FindIndex(f => f.Id == id, out var d);

            d.Status = visible ? ItemStatus.Public : ItemStatus.Pending;
            d.Rename(name);
            _documents.Move(oldIndex, index);
            //throw new NotImplementedException();
        }
    }
}