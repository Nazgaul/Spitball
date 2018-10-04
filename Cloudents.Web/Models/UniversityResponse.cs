using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Web.Models
{
    public class UniversityResponse
    {
        public UniversityResponse(IEnumerable<UniversityDto> universities)
        {
            Universities = universities;
        }

        public IEnumerable<UniversityDto> Universities { get; set; }
    }
}