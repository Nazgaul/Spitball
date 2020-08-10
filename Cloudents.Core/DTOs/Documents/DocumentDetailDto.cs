using System;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentDetailDto
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public DocumentType DocumentType { get; set; }



        public int Pages { get; set; }
        public bool IsPurchased { get; set; }
        
        [NonSerialized]
        public Money Price;

        public Money? SubscriptionPrice { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long CourseId { get; set; }
    }

   
}
