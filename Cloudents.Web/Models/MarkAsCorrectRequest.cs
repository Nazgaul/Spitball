using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class MarkAsCorrectRequest
    {
        [Required]
        public Guid AnswerId { get;  set; }

    }
}