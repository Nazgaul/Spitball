using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class UnSuspendUserRequest
    {
        /// <summary>
        /// The User Id
        /// </summary>
        [Required]
        [FromQuery(Name = "id")]
        public IEnumerable<long> Ids { get; set; }

    }


    public class UnSuspendUserResponse
    {
        /// <summary>
        /// The User Email
        /// </summary>
        public IEnumerable<string> Email { get; set; }
    }
}

