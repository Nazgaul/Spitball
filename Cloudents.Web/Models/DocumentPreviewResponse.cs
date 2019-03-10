using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cloudents.Core.DTOs;

namespace Cloudents.Web.Models
{
    public class DocumentPreviewResponse
    {
        public DocumentPreviewResponse(DocumentDetailDto details, IEnumerable<Uri> preview, string content)
        {
            Details = details;
            Content = content;
            Preview = preview;//.OrderBy(o=>o,new OrderPreviewComparer());
        }

        public DocumentDetailDto Details { get;  }
        public IEnumerable<Uri> Preview { get;  }

        public string Content { get; }
    }


    //public class OrderPreviewComparer : IComparer<Uri>
    //{
    //    public int Compare(Uri s1, Uri s2)
    //    {

    //        string GetNumberStr(Uri x)
    //        {
    //            return Regex.Replace(x?.Segments.Last() ?? string.Empty, "[^\\d]", string.Empty);
    //        }

    //        var z = GetNumberStr(s1);
    //        var z2 = GetNumberStr(s2); 

    //        if (int.TryParse(z, out var i1) && int.TryParse(z2, out var i2))
    //        {
    //            if (i1 > i2) return 1;
    //            if (i1 < i2) return -1;
    //            if (i1 == i2) return 0;
    //        }

    //        return 0;
    //        //return string.Compare(s1, s2, true);
    //    }

    //}
}