using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class StudyRoomPayment : Entity<Guid>
    {
        public StudyRoomPayment(StudyRoomSessionUser studyRoomSessionUser)
        {
            PricePerHour = studyRoomSessionUser.StudyRoomSession.StudyRoom.Price;
            Tutor = studyRoomSessionUser.StudyRoomSession.StudyRoom.Tutor;
            User = studyRoomSessionUser.User;
            StudyRoomSessionUser = studyRoomSessionUser;
            Created = DateTime.UtcNow;;
        }

        public StudyRoomPayment(Tutor tutor, User user,TimeSpan duration,double price)
        {
            PricePerHour = new Money(price,tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
            Tutor = tutor;
            User = user;
            ApproveSession(duration, price);
            //StudyRoomSessionUser = studyRoomSessionUser;
            Created = DateTime.UtcNow;;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected StudyRoomPayment()
        {
        }

        public virtual void ApproveSession(TimeSpan duration, double price)
        {
            PricePerHour = PricePerHour.ChangePrice(price);
            ApproveSession(duration);
        }

        protected virtual void ApproveSession(TimeSpan duration)
        {
            TutorApproveTime = duration;
            TotalPrice = TutorApproveTime.Value.TotalHours * PricePerHour.Amount;
        }

        public virtual void NoPay()
        {
            Receipt = "No Pay";
            TutorApproveTime ??= StudyRoomSessionUser?.Duration;
            TotalPrice = 0;
        }

        public virtual void Pay(string receipt, TimeSpan duration,  double price)
        {
            Receipt = receipt;
            ApproveSession(duration);
            TotalPrice = price;
        }

        public virtual StudyRoomSessionUser? StudyRoomSessionUser { get; protected set; }

        public virtual TimeSpan? TutorApproveTime { get; protected set; }

        public virtual Money PricePerHour { get; protected set; }

        public virtual double TotalPrice { get;  set; }

        public virtual string? Receipt { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual Tutor Tutor { get;protected set; }

        public virtual DateTime Created { get;protected set; }
    }
}