using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Cloudents.Admin2.Models
{
    public class DeleteUserRequest
    {
        /// <summary>
        /// The User Id
        /// </summary>
        [Required]
        [FromQuery(Name = "id")]
        public long Id { get; set; }

    }
}
