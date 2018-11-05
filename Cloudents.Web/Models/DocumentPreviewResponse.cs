using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cloudents.Core.DTOs;

namespace Cloudents.Web.Models
{
    public class DocumentPreviewResponse
    {
        public DocumentPreviewResponse(DocumentDetailDto details, IEnumerable<Uri> preview)
        {
            Details = details;
            Preview = preview.OrderBy(o=>o,new OrderPreviewComparer());
        }

        public DocumentDetailDto Details { get; set; }
        public IEnumerable<Uri> Preview { get; set; }
    }


    public class OrderPreviewComparer : IComparer<Uri>
    {
        public int Compare(Uri s1, Uri s2)
        {
            var z = Regex.Replace(s1.Segments.Last(), "[^\\d]", string.Empty);
            var z2 = Regex.Replace(s2.Segments.Last(), "[^\\d]", string.Empty);

            if (int.TryParse(z, out var i1) && int.TryParse(z2, out var i2))
            {
                if (i1 > i2) return 1;
                if (i1 < i2) return -1;
                if (i1 == i2) return 0;
            }

            return 0;
            //return string.Compare(s1, s2, true);
        }

    }
}