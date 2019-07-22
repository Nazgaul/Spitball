﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class AddShortUrlRequest
    {
        [Required]
        public string Destination { get; set; }
        [Required]
        [StringLength(6)]
        public string Identifier { get; set; }
        public DateTime? Expiration { get; set; }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
    }
}
