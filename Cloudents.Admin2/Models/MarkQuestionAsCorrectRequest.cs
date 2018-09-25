using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Models
{
    public class MarkQuestionAsCorrectRequest
    {
        /// <summary>
        /// The question id
        /// </summary>
        [Required]
        public long? QuestionId { get; set; }

        /// <summary>
        /// The answer id
        /// </summary>
        [Required]
        public Guid? AnswerId { get; set; }
    }
}
