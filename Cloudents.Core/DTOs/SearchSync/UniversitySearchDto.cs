
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class UniversitySearchDto
    {
        [DtoToEntityConnection(nameof(University.Id))]
        public string UniversityId { get; set; }
        [DtoToEntityConnection(nameof(University.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(University.Extra))]
        public string Extra { get; set; }


    }
}