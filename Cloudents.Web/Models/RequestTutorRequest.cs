using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        public string Text { get; set; }
        public string Course { get; set; }

        [MaxLength(4, ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }
    }
}