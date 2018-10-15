﻿using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RegisterRequest
    {
        [EmailAddress(ErrorMessage = "EmailAddress"), Required(ErrorMessage = "Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "StringLength", MinimumLength = Startup.PasswordRequiredLength)]

        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [Compare("Password", ErrorMessage = "Compare")]
        public string ConfirmPassword { get; set; }
    }
}