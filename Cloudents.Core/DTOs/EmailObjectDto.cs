using System.Collections.Generic;
using System.Globalization;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

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


    public class EmailPaymentDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string TutorName { get; set; }
        [EntityBind(nameof(RegularUser.Language))]
        public string Language { get; set; }
    }
}
