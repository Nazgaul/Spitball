using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.Models
{
    /// <summary>
    /// Course request object
    /// </summary>
    public class CourseRequest
    {
        /// <summary>
        /// User input
        /// </summary>
        [StringLength(int.MaxValue,MinimumLength = 3)]
        public string Term { get; set; }

        /// <summary>
        /// university of the user
        /// </summary>
        [Required]
        public long? UniversityId { get; set; }
    }
}