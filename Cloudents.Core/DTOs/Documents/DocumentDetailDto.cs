using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentDetailDto
    {
        public string Course { get; set; }
        public long Id { get; set; }
        private TimeSpan? _duration;
        public string Title { get; set; }

        public DateTime? DateTime { get; set; }

        public decimal? Price { get; set; }
        public string Preview { get; set; }

        public PriceType? PriceType { get; set; }

        public DocumentType DocumentType { get; set; }

        public TimeSpan? Duration
        {
            get
            {
                if (_duration.HasValue)
                {
                    return _duration.Value.StripMilliseconds();
                }

                return _duration;

            }
            set => _duration = value;
        }

        public string? UserName { get; set; }

        public long? UserId { get; set; }

        public int Pages { get; set; }
        public bool IsPurchased { get; set; }

        //public long? DuplicateId { get; set; }

        //public bool ShouldSerializeDuplicateId() => false;
    }

   
}
