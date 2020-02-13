using System;

namespace Cloudents.Core.DTOs
{
    public class DashboardBlogDto
    {
        public Uri Image { get; set; }

        public Uri Url { get; set; }
        public string Title { get; set; }
        public string Uploader { get; set; }
        public DateTimeOffset Create { get; set; }
    }
}