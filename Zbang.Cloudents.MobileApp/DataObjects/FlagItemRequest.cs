using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class FlagItemRequest
    {
        [Required]
        public long ItemId { get; set; }
    }
}