﻿using System;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentCommand : ICommand
    {
        public PaymentCommand(long userId, long tutorId, decimal studentPay, decimal spitballPay,
            Guid studyRoomSessionId, string spitballBuyerKey, int adminDuration)
        {
            UserId = userId;
            TutorId = tutorId;
            StudentPay = studentPay;
            SpitballPay = spitballPay;
            StudyRoomSessionId = studyRoomSessionId;
            SpitballBuyerKey = spitballBuyerKey;
            AdminDuration = adminDuration;
        }

        public long UserId { get; }
        public long TutorId { get; }
        public decimal StudentPay { get; }
        public decimal SpitballPay { get; }

        public Guid StudyRoomSessionId { get; }
        public string SpitballBuyerKey { get; }
        public int AdminDuration { get; }

    }
}
