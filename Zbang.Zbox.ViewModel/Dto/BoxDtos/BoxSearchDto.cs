using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    /// <summary>
    /// Used to upload data to azure search
    /// </summary>
    public class BoxSearchDto
    {

        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Professor { get; set; }
        public string CourseCode { get; set; }


        public string Url { get; set; }

        public long UniversityId { get; set; }

        public IEnumerable<long> UserIds { get; set; }
    }

    public class UsersInBoxSearchDto
    {
        public long UserId { get; set; }
        public long BoxId { get; set; }
    }

    public class BoxToUpdateSearchDto
    {
        public IEnumerable<BoxSearchDto> BoxesToUpdate { get; set; }

        public IEnumerable<long> BoxesToDelete { get; set; }
    }
}
