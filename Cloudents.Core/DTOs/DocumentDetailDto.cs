using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs
{
    public class DocumentDetailDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string University { get; set; }

        public string Course { get; set; }


        public TutorCardDto User { get; set; }

        public long UploaderId { get; set; }
        public string UploaderName { get; set; }


        public int Pages { get; set; }

        public int Views { get; set; }

        public decimal? Price { get; set; }

        public bool IsPurchased { get; set; }

        public DocumentType DocumentType { get; set; }

        //  public int PageCount { get; set; }

    }

    //public class DocumentUserDto 
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public string Image { get; set; }
    //    public int Score { get; set; }
    //    public string Courses { get; set; }
    //    public decimal Price { get; set; }

    //    public float? Rate { get; set; }

    //    public string Bio { get; set; }

    //    public int ReviewsCount { get; set; }
    //    public bool IsTutor { get; set; }
    //}
}
