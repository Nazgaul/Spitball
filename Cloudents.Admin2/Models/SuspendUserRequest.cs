using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class SuspendUserRequest
    {
        /// <summary>
        /// The User Id
        /// </summary>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// If we want to delete all his questions
        /// </summary>
        [Required]
        public bool DeleteUserQuestions { get; set; }
    }


    public class SuspendUserResponse
    {
        /// <summary>
        /// The User Email
        /// </summary>
        public string Email { get; set; }
    }
}
