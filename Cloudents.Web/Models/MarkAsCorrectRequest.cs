using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class MarkAsCorrectRequest
    {
        [Required(ErrorMessage = "Required")]
        public Guid AnswerId { get;  set; }

    }
}