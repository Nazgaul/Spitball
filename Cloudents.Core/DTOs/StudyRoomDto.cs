using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
                if (TutorPrice.Cents == 0)
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

        public Money TutorPrice { get; set; }
        public string? Jwt { get; set; }

        public DateTime? BroadcastTime { get; set; }

        public string Name { get; set; }

        public StudyRoomTopologyType TopologyType { get; set; }
        public StudyRoomType Type { get; set; }
    };

    public class StudyRoomSeoDto
    {
        public string Name { get; set; }
        public string TutorName { get; set; }
        public string Description { get; set; }
    }


    public class UserStudyRoomDto
    {
        public UserStudyRoomDto(string name, Guid id, DateTime dateTime,
            string conversationId, DateTime? lastSession, StudyRoomType type,
            DateTime? scheduled, IEnumerable<string> userNames, Money money)
        {
            Name = name;
            Id = id;
            DateTime = dateTime;
            ConversationId = conversationId;
            LastSession = lastSession;
            Type = type;
            Scheduled = scheduled;
            UserNames = userNames;
            Price = money;

        }

        public UserStudyRoomDto()
        {
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public string ConversationId { get; set; }
        public DateTime? LastSession { get; set; }

        public StudyRoomType Type { get; set; }

        public DateTime? Scheduled { get; set; }

        public IEnumerable<string> UserNames { get; set; }
        public Money Price { get; }

    }

    public class FutureBroadcastStudyRoomDto
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public Money Price { get; set; }

        public bool Enrolled { get; set; }

        public string? Description { get; set; }

        public bool IsFull { get; set; }
    }
}