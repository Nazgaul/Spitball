using System;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentCommand : ICommand
    {
        public PaymentCommand(long userId, long tutorId, double studentPay, double spitballPay,
            Guid studyRoomSessionId,  TimeSpan adminDuration)
        {
            UserId = userId;
            TutorId = tutorId;
            StudentPay = studentPay;
            SpitballPay = spitballPay;
            StudyRoomSessionId = studyRoomSessionId;
            AdminDuration = adminDuration;
        }

        public long UserId { get; }
        public long TutorId { get; }
        public double StudentPay { get; }
        public double SpitballPay { get; }

        public Guid StudyRoomSessionId { get; }
        
        public TimeSpan AdminDuration { get; }

    }
}
