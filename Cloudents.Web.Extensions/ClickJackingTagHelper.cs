using System;

namespace Cloudents.Web.Extensions
{
    public static class ClickJackingTagHelper
    {
        public const string FullScript = "<style id=\"acj\">body{display:none !important;}</style>" +
                                         "<script type=\"text/javascript\">" +
                                         "if(self===top){var acj = document.getElementById(\"acj\"); acj.parentNode.removeChild(acj)"+
                                         "}else top.location=self.location;"+
                                         "</script> ";

    }
}
