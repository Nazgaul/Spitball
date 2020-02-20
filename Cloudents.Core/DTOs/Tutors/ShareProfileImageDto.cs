
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Tutors
{
    public class ShareProfileImageDto
    {

        public string Image { get; set; }

        //public string CountryStr { get; set; }


       // public Country Country => CountryStr;
        public string Name { get; set; }
        public double Rate { get; set; }
        public string Description { get; set; }
    }

    public class ShareDocumentImageDto
    {
        public string CourseName { get; set; }
        public string Name { get; set; }
        public DocumentType Type { get; set; }
    }
}