using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class SuspendUserRequest
    {
        /// <summary>
        /// The User Id
        /// </summary>
        [Required]
        [FromQuery(Name = "id")]
        public IEnumerable<long> Ids { get; set; }

        /// <summary>
        /// If we want to delete all his questions
        /// </summary>
        public SuspendTime? SuspendTime { get; set; }

        /// <summary>
        /// A reason for user suspension
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Reason { get; set; }
    }

    public enum SuspendTime
    {
        Day,
        Week,
        Undecided
    }


    //public class SuspendUserResponse
    //{
    //    /// <summary>
    //    /// The User Email
    //    /// </summary>
    //    public IEnumerable<string> Email { get; set; }
    //}
}
