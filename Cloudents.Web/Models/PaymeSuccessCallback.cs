using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class PaymeSuccessCallback
    {
        [ModelBinder(Name = "payme_status")]
        public string Status { get; set; }
    }
}
