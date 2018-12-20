using System.ComponentModel.DataAnnotations;
using Cloudents.Application.Enum;
using Cloudents.Common;
using Cloudents.Common.Enum;

namespace Cloudents.Admin2.Models
{
    public class CreateQuestionRequest
    {
        /// <summary>
        /// Subject of the question
        /// </summary>
        [Required]
        public QuestionSubject SubjectId { get; set; }
        /// <summary>
        /// The text of the question
        /// </summary>
        [Required]
        [MinLength(15)]
        public string Text { get; set; }
        /// <summary>
        /// The price of the question
        /// </summary>
        [Required] public decimal Price { get; set; }
        /// <summary>
        /// the country of the question - can be us or il
        /// </summary>
        [Required]
        public Country Country { get; set; }

        [MaxLength(4)]
        public string[] Files { get; set; }
    }

    public enum Country
    {
        Us,
        Il
    }
}
