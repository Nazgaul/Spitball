using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class CreateStudyRoomRequest
    {
        public IEnumerable<long> UserId { get; set; }
    }
}