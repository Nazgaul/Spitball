
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

        public IEnumerable<DocumentFeedDto> Documents { get; set; }
        public IEnumerable<FutureBroadcastStudyRoomDto> StudyRooms { get; set; }

    }


}
