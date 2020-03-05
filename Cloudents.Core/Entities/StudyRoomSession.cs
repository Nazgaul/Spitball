using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class StudyRoomSession : Entity<Guid>
    {
        public StudyRoomSession(StudyRoom studyRoom, string sessionId)
        {
            StudyRoom = studyRoom;
            Created = DateTime.UtcNow;
            SessionId = sessionId;
            AddEvent(new StudyRoomSessionCreatedEvent(this));
        }
        protected StudyRoomSession()
        {

        }

        public virtual StudyRoom StudyRoom { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime? Ended { get; protected set; }
        public virtual TimeSpan? Duration { get; protected set; }

        //TODO remove this
        public virtual TimeSpan? DurationInMinutes { get; protected set; }

        public virtual int RejoinCount { get; protected set; }
        public virtual string SessionId { get; protected set; }
        public virtual string Receipt { get; protected set; }
        public virtual decimal? Price { get; protected set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nhibernate proxy")]
        public virtual byte[] Version { get; protected set; }

        private readonly IList<SessionParticipantDisconnect> _participantDisconnections = new List<SessionParticipantDisconnect>();

        public virtual IEnumerable<SessionParticipantDisconnect> ParticipantDisconnections => _participantDisconnections;

        public virtual bool VideoExists { get; protected set; }


        public virtual IPaymentProvider Payment { get; protected set; }
        public virtual DateTime? PaymentApproved { get; protected set; }
        public virtual long? AdminDuration { get; protected set; }
        //public virtual decimal? StudentPay { get; protected set; }
        //public virtual decimal? SpitballPay { get; protected set; }



        public virtual void UpdateVideo()
        {
            VideoExists = true;
        }

        protected virtual void CalculatePriceAndDuration()
        {
            Duration = DurationInMinutes = Ended - Created;

            Price = ((decimal)Math.Floor(DurationInMinutes.Value.TotalMinutes) / 60) * StudyRoom.Tutor.Price.SubsidizedPrice ??
                      ((decimal)Math.Floor(DurationInMinutes.Value.TotalMinutes) / 60) * StudyRoom.Tutor.Price.Price;
        }

        public virtual void EditDuration(int duration)
        {
            if (Ended == null)
            {
                throw new ArgumentException();
            }
            Ended = Created.AddMinutes(duration);
            CalculatePriceAndDuration();
        }

        public virtual void EndSession()
        {
            if (Ended.HasValue)
            {
                return;
            }
            Ended = DateTime.UtcNow;
            CalculatePriceAndDuration();
            AddEvent(new EndStudyRoomSessionEvent(this));
        }

        public virtual void ReJoinStudyRoom()
        {
            RejoinCount++;
            AddEvent(new StudyRoomSessionRejoinEvent(this));
        }

        public virtual void SetReceipt(string receipt)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
        }

        public virtual void SetReceiptAndAdminDate(string receipt, long adminDuration)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
            PaymentApproved = DateTime.UtcNow;
            AdminDuration = adminDuration;
            //AdminDuration = adminDuration;
            //StudentPay = studentPay;
            //SpitballPay = spitballPay;
        }

        public virtual void SetPyment(IPaymentProvider payment)
        {
            Payment = payment;
        }
    }
}
