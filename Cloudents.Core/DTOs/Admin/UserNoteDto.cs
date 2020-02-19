using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserNoteDto
    {
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string AdminUser { get; set; }
    }
}
