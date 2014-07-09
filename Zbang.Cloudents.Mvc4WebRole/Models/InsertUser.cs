using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class InsertUser
    {
        [Required(ErrorMessage="שדה חובה")]
        [Display(Name = "שם פרטי:")]
        public string FirstName { get; set; }

        [Display(Name = "שם משפחה:")]
        [Required(ErrorMessage = "שדה חובה")]
        public string LastName { get; set; }

        [Display(Name = "ת.ז.")]
        [Required(ErrorMessage = "שדה חובה")]
        public string Id { get; set; }
    }
}