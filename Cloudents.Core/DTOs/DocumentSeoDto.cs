namespace Cloudents.Core.DTOs
{
    public class DocumentSeoDto
    {
        public DocumentSeoDto(string name, string courseName,   string country, string universityName, long id)
        {
            Name = name;
            CourseName = courseName;
            Country = country;
            UniversityName = universityName;
            Id = id;
        }

        public string Name { get; }
        public string CourseName { get;  }


        public string UniversityName { get;}

        public string Country { get; }
        public long Id { get;  }

    }
}
