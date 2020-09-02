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
        [NonSerialized] public bool EnrolledStudyRoom;
        [NonSerialized] public bool EnrolledCourse;

        [NonSerialized] public int AmountOfPayments;
        [NonSerialized] public int AmountOfSessions;


        public bool Enrolled => EnrolledStudyRoom || EnrolledCourse;


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

                if (EnrolledCourse)
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

        public Money TutorPrice { get; set; }
        public string? Jwt { get; set; }

        public DateTime? BroadcastTime { get; set; }

        public string Name { get; set; }

        public StudyRoomTopologyType TopologyType { get; set; }
        public StudyRoomType Type { get; set; }
    };

    

    public class FutureBroadcastStudyRoomDto
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public bool OnGoing { get; set; }
      
    }

    public class CourseEditStudyRoomDto
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}