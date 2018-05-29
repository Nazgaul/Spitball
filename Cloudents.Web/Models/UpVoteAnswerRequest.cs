using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UpVoteAnswerRequest
    {
        [Required]
        public string PrivateKey { get; set; }
        public Guid Id { get; set; }
    }
}