using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateQuestionRequest
    {
        /// <summary>
        /// Subject of the question
        /// </summary>
        [Required]
        public string Course { get; set; }
        /// <summary>
        /// The text of the question
        /// </summary>
        [Required]
        [MinLength(15)]
        public string Text { get; set; }

        /// <summary>
        /// the country of the question - can be us or il
        /// </summary>
        [Required]
        public string Country { get; set; }

      


    }

    //public enum Country
    //{
    //    Us,
    //    Il,
    //    In
    //}
}
