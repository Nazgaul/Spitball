﻿using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    /// <summary>
    /// Course request object
    /// </summary>

    //TODO:Localize
    public class CourseRequest
    {
        /// <summary>
        /// User input
        /// </summary>
        [StringLength(int.MaxValue,MinimumLength = 3)]
        public string Term { get; set; }


        ///TODO: REMOVE THIS
        /// <summary>
        /// university of the user
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public long? UniversityId { get; set; }
    }
}