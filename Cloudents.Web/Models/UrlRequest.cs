using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UrlRequest
    {
        [Required(ErrorMessage = "Required")]
        public string Host { get; set; }

        [Required(ErrorMessage = "Required")]
        public Uri Url { get; set; }

        public int Location { get; set; }

        public override string ToString()
        {
            return $"{nameof(Host)}: {Host}, {nameof(Url)}: {Url}, {nameof(Location)}: {Location}";
        }
    }
}