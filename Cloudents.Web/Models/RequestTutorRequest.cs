using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        public string Text { get; set; }
        public string Course { get; set; }

        public IFormFile File { get; set; }
    }
}