using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class InviteSystemFromFacebook
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string UserName { get; set; }
        //[Required]
        //public string Name { get; set; }


        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public bool Sex { get; set; }
    }
}