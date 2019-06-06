using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class AboutTutorDto
    {
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(TutorReview.Review))]
        public string Review { get; set; }
        [EntityBind(nameof(TutorReview.Rate))]
        public decimal Rate { get; set; }
        [EntityBind(nameof(User.Image))]
        public string Image { get; set; }
    }
}
