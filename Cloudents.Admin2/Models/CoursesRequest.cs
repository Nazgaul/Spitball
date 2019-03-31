using Cloudents.Core.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class CoursesRequest
    {
        public string Language { get; set; }
        public ItemState? State { get; set; }
    }

    public class UniversitiesRequest
    {
        public string Country { get; set; }
        public ItemState? State { get; set; }
    }
}
