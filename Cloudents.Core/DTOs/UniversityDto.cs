
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class UniversityDto
    {
        public UniversityDto(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; set; }
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
