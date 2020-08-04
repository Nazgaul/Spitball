using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentDetailDto
    {
      // public string Course { get; set; }
        public long Id { get; set; }
     //   private TimeSpan? _duration;
        public string Title { get; set; }

     //   public DateTime? DateTime { get; set; }

      //  public decimal? Price { get; set; }
      //  public string Preview { get; set; }

      //  public PriceType? PriceType { get; set; }

        public DocumentType DocumentType { get; set; }

        //public TimeSpan? Duration
        //{
        //    get
        //    {
        //        if (_duration.HasValue)
        //        {
        //            return _duration.Value.StripMilliseconds();
        //        }

        //        return _duration;

        //    }
        //    set => _duration = value;
        //}

      //  public string? UserName { get; set; }


        public int Pages { get; set; }
        public bool IsPurchased { get; set; }
        public Money Price { get; set; }
        public Money? SubscriptionPrice { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long CourseId { get; set; }
    }

   
}
