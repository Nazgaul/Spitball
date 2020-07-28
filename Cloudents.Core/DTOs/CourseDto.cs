
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Reflection")]
    public class CourseNameDto
    {

        public string Name { get;  set; }
    }


    public class CourseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public Money? Price { get; set; }
        public Money? SubscriptionPrice { get; set; }

        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public int StudyRoomCount { get; set; }

        public string Image { get; set; }
       // public IEnumerable<DocumentFeedDto> Documents { get; set; }
        //public IEnumerable<FutureBroadcastStudyRoomDto> StudyRooms { get; set; }

    }

    public class CourseDetailDto
    {
        public IEnumerable<DocumentFeedDto> Documents { get; set; }
        public long Id { get; set; }
        public IEnumerable<FutureBroadcastStudyRoomDto> StudyRooms { get; set; }
        public string Description { get; set; }
        public Country TutorCountry { get; set; }
        public string? TutorImage { get; set; }
        public string TutorName { get; set; }
        public long TutorId { get; set; }
        public string Name { get; set; }
        public string? TutorBio { get; set; }
        public Money Price { get; set; }
        public string Image { get; set; }
        public bool Enrolled { get; set; }
        public bool Full { get; set; }

        [NonSerialized]
        public Money? SubscriptionPrice;
    }


}
