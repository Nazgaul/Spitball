using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class FollowRequest
    {
        [Required]
        public long Id { get; set; }
    }

    public class UnFollowRequest
    {
        [Required]
        public long Id { get; set; }
    }

    public class EnrollStudyRoomRequest
    {
        [Required]
        public Guid StudyRoomId { get; set; }
    }
}
