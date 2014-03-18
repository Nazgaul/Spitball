using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class BoxDigestDto
    {
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string BoxPicture { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }

    }
}
