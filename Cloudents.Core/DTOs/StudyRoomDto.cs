﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Dapper")]
    [SuppressMessage("Style", "CS8618", Justification = "Dapper")]
    public class StudyRoomDto
    {
        [NonSerialized]
        public bool _UserPaymentExists;

        [NonSerialized] public Country TutorCountry;
        [NonSerialized] public long UserId;

        public string OnlineDocument { get; set; }
        public string ConversationId { get; set; }
        public long TutorId { get; set; }
        public string? TutorImage { get; set; }
        public string TutorName { get; set; }

        public bool NeedPayment
        {
            get
            {
                if (TutorPrice == 0)
                {
                    return false;
                }

                if (_UserPaymentExists)
                {
                    return false;
                }

                if (TutorCountry == Country.India)
                {
                    return false;
                }

                if (UserId == TutorId)
                {
                    return false;
                }

                return true;
            }
        }

       // public CouponType? CouponType { get; set; }

       // public bool ShouldSerializeCouponType() => false;
      //  public bool ShouldSerializeCouponValue() => false;

      //  public decimal? CouponValue { get; set; }

        public decimal TutorPrice { get; set; }
        public string Jwt { get; set; }

        public DateTime? BroadcastTime { get; set; }

        public string Name { get; set; }


        public StudyRoomType Type { get; set; }
    };



    public class UserStudyRoomDto
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public string ConversationId { get; set; }
        public DateTime? LastSession { get; set; }

        public StudyRoomType Type { get; set; }

        public int AmountOfUsers { get; set; }
        public DateTime? Scheduled { get; set; }

    }

    public class FutureBroadcastStudyRoomDto
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public Money Price { get; set; }

        public bool Enrolled { get; set; }

        public string? Description { get; set; }
    }
}