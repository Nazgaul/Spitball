using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Core.DTOs
{
    public class EmailObjectDto
    {
        public EmailObjectDto()
        {
            Blocks = new List<EmailBlockDto>();
        }

        public bool SocialShare { get; set; }
        public string Event { get; set; }

        public string Subject { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public IList<EmailBlockDto> Blocks { get;private set; }
    }
}
