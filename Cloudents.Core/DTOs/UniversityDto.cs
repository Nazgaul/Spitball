
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
}
