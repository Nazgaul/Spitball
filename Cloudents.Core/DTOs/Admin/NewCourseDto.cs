using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Admin
{
   
    public class PendingCoursesDto
    {
        [EntityBind(nameof(Course.Id))]
        public string Name { get; set; }
    }
   

}
