using Cloudents.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChangePriceRequest
    {
        public long Id { get; set; }

        [Range(0, (int)Document.PriceLimit)]
        public decimal Price { get; set; }
    }
}
