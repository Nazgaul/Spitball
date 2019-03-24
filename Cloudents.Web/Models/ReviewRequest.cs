using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class ReviewRequest
    {
        public ReviewRequest(string review, float rate, long tutorId)
        {
            Review = review;
            Rate = rate;
            Tutor = tutorId;
        }
        public virtual string Review { get; set; }
        public virtual float Rate { get; set; }
        public virtual long Tutor { get; set; }
    }
}
