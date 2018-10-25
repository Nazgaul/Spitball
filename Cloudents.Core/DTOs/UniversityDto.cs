
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.DTOs
{
    public class UniversityDto
    {
        public UniversityDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UniversitySearchDto
    {
        public UniversitySearchDto(IEnumerable<UniversityDto> universities)
        {
            Universities = universities;
        }

        private UniversitySearchDto()
        {

        }

        public static UniversitySearchDto StopWordResponse()
        {
            return new UniversitySearchDto
            {
                Universities = Enumerable.Empty<UniversityDto>(),
                StopWord = true
            };
        }


        /// <summary>
        /// A list of universities or empty if no university found
        /// </summary>
        public IEnumerable<UniversityDto> Universities { get; set; }

        /// <summary>
        /// an indicator is the search is stop word
        /// </summary>
        public bool? StopWord { get; set; }
    }
}
