using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class GoogleTokenRequest
    {
        [Required]
        public string Token { get; set; }

    }
}