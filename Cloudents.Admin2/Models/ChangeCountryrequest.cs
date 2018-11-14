using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class ChangeCountryrequest
    {
        /// <summary>
        /// The User Id
        /// </summary>
        [Required]
        [FromQuery(Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// New country
        /// </summary>
        [Required]
        [FromQuery(Name = "country")]
        [StringLength(2)]
        public string Country { get; set; }

    }
}
