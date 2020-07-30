using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateCourseRequest
    {
        [Required]
        public string Name { get; set; }

        [Range(0,int.MaxValue)]
        public int Price { get; set; }

        public int? SubscriptionPrice { get; set; }

        public string Description { get; set; }

        public string? Image { get; set; }

        public IEnumerable<CreateLiveStudyRoomRequest> StudyRooms { get; set; }
        public IEnumerable<CreateDocumentRequest> Documents { get; set; }
        public bool IsPublish { get; set; }
    }
}