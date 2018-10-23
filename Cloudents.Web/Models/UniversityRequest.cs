﻿using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    /// <summary>
    /// University request object
    /// </summary>
    public class UniversityRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "StringLength")]
        public string Term { get; set; }
    }
}