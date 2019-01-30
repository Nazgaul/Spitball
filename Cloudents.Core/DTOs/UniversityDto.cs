using System;
using System.Collections.Generic;

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

        /// <summary>
        /// A list of universities or empty if no university found
        /// </summary>
        public IEnumerable<UniversityDto> Universities { get; set; }

    }
}
