using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class UniversitySynonymDto
    {
        public IEnumerable<string> Name { get; set; }
    }

    public class UniversityWriteDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public long Version { get; set; }
    }
}