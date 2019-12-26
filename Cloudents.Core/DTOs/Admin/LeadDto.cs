using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class LeadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Text { get; set; }
        public string Course { get; set; }
        public string University { get; set; }

        public DateTime? DateTime { get; set; }

       // public string Referer { get; set; }
       // public ItemState? Status { get; set; }
    }
}
