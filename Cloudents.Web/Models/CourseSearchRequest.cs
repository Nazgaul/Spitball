namespace Cloudents.Web.Models
{
    /// <summary>
    /// Course request object
    /// </summary>
    public class CourseSearchRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "StringLength")]
        public string? Term { get; set; }

        public int Page { get; set; }
    }
}