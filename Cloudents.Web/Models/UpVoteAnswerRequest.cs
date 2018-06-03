using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UpVoteAnswerRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}