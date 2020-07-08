using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChangePriceRequest
    {
        public long Id { get; set; }

        [Range(0, 214748)] //small money restriction
        public decimal Price { get; set; }
    }
}
