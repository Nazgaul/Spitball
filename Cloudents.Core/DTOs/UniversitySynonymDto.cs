using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    public class UniversitySynonymDto
    {
        public IEnumerable<string> Name { get; set; }
    }

    [DataContract]
    public class UniversitySearchWriteDto : UniversitySearchDeleteDto
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        public long Version { get; set; }
    }

    [DataContract]
    public class UniversitySearchDeleteDto
    {
        public UniversitySearchDeleteDto()
        {
            
        }

        public UniversitySearchDeleteDto(long id)
        {
            Id = id;
        }
        [DataMember]
        public long Id { get; set; }
    }
}