
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
