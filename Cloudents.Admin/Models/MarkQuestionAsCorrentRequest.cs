using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin.Models
{
    public class MarkQuestionAsCorrentRequest
    {
        [Required]
        public long? QuestionId { get; set; }

        [Required]
        public Guid? AnswerId { get; set; }
    }
}
