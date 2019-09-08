namespace Cloudents.Core.DTOs
{
    public class CalendarDto
    {
        public CalendarDto(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get;  }
        public string Name { get; }
    }
}