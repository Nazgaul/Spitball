
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Store
{
   public  class BannerDto
    {
       public string Url { get; set; }
       public string ImageUrl { get; set; }
       public StoreBannerLocation Location { get; set; }

       public int? Order { get; set; }  
    }
}
