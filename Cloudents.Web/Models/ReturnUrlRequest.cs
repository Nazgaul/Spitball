using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    [ModelBinder(typeof(ReturnUrlEntityBinder))]
    public class ReturnUrlRequest
    {
        public string Url { get; set; }

        public override string ToString()
        {
            return Url;
        }
    }
}
