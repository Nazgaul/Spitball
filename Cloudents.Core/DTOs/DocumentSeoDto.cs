namespace Cloudents.Application.DTOs
{
    public class DocumentSeoDto
    {
        //public DocumentSeoDto(string name, string courseName,   string country, string universityName, long id, string metaContent)
        //{
        //    Name = name;
        //    CourseName = courseName;
        //    Country = country;
        //    UniversityName = universityName;
        //    Id = id;
        //    MetaContent = metaContent;
        //}

        public string Name { get; set; }
        public string CourseName { get; set; }

        public string  MetaContent { get; set; }

        public string UniversityName { get; set; }

        public string Country { get; set; }
        public long Id { get; set; }

    }
}
