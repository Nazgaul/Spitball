﻿namespace Cloudents.Core.DTOs
{
    public class DocumentSeoDto
    {
        public DocumentSeoDto(string name, string courseName,   string country, string universityName)
        {
            Name = name;
            CourseName = courseName;
            Country = country;
            UniversityName = universityName;
        }

        public string Name { get; }
        public string CourseName { get;  }


        public string UniversityName { get;}

        public string Country { get; }
    }
}
