﻿using Cloudents.Core.DTOs.Tutors;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentDetailDto
    {
        public TutorCardDto Tutor { get; set; }
        public DocumentFeedDto Document{ get; set; }
        public int Pages { get; set; }
        public bool IsPurchased { get; set; }

        public long? DuplicateId { get; set; }

        public bool ShouldSerializeDuplicateId() => false;
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