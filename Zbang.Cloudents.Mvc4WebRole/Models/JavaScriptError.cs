using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class JavaScriptError
    {
        public string Cause { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorUrl { get; set; }

        public string[] StackTrace { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Cause:" + Cause);
            sb.AppendLine("ErrorMessage:" + ErrorMessage);
            sb.AppendLine("ErrorUrl:" + ErrorUrl);
            sb.AppendLine("StackTrace:" + String.Join("\n", StackTrace));
            return sb.ToString();
        }
    }
}