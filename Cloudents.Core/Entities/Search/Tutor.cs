using System;

namespace Cloudents.Core.Entities.Search
{
    public class Tutor
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Fee { get; set; }

        public bool Online { get; set; }

        public bool InPerson { get; set; }
        public string[] Subjects { get; set; }

        public double Rank { get; set; }

        public DateTime? InsertDate { get; set; }

        public string[] Extra { get; set; }
    }
}