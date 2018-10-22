using System;
using System.ComponentModel.DataAnnotations;

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
