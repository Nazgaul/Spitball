using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Tutors
{
    public class ShareProfileImageDto
    {

        public string Image { get; set; }

        public string CountryStr { get; set; }


        public Country Country => CountryStr;
        public string Name { get; set; }
        public double Rate { get; set; }
    }
}