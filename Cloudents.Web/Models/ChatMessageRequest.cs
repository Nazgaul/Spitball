using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class ChatMessageRequest
    {
        //public Guid? ChatId { get; set; }

        [Required]
        public string Message { get; set; }

        public long OtherUser { get; set; }
    }
}
