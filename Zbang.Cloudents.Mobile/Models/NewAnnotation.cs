using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class NewAnnotation
    {
        [Required(AllowEmptyStrings=false)]
        public string Comment { get; set; }
       
        [Required]
        public long ItemId { get; set; }
      
    }
}