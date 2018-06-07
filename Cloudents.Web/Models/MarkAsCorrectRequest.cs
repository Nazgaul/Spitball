using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class MarkAsCorrectRequest
    {
        [Required]
        public Guid AnswerId { get;  set; }

        //[Required]
        //public long QuestionId { get;  set; }

        //[Required]
        //public string PrivateKey { get; set; }

    }
}