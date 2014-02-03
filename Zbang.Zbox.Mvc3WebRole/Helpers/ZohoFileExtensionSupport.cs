using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.Mvc3WebRole.Helpers
{
    //ToDo need to remove it
    public static class ZohoFileExtensionSupport
    {
        //public static readonly string[] extension = new string[] { "doc", "docx", "xls", "xlsx", "ppt", "pptx", "pps", "odt", "ods", "odp", "sxw", "sxc", "sxi", "wpd", "pdf", "rtf", "html", "txt", "csv", "tsv" };
       // public static readonly string[] extension = new string[] { "xls", "xlsx", "xlsm", "xltx", "ods", "csv", "rtf", "docx", "doc", "ppt", "pot", "pps", "pptx", "potx", "ppxs", "pdf" };

        public static string ToDelimitedString()
        {
            return string.Join("|", Validation.Extension);

        }
    }
}