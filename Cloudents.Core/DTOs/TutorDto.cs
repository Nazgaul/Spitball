using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.DTOs
{
    public class TutorDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public double Fee { get; set; }

        public bool Online { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
