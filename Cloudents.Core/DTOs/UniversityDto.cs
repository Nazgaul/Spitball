using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class UniversityDto
    {
        public string Image { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
    }

    //public  class UniversityDtoEquality : IEqualityComparer<UniversityDto>
    //{
    //    public bool Equals(UniversityDto x, UniversityDto y)
    //    {
    //        return x?.Id == y?.Id;
    //    }

    //    public int GetHashCode(UniversityDto obj)
    //    {
    //        return obj.Id.GetHashCode();
    //    }
    //}
}
