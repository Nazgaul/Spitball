using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class CreateAnswerRequest
    {
        [Required]
        public long QuestionId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }

        [MaxLength(4)]
        public IEnumerable<string> Files { get; set; }
    }

    public class DeleteAnswerRequest
    {
        [FromRoute]
        public Guid Id { get; set; }
    }
}