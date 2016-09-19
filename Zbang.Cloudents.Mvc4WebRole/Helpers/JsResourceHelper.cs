using System.Text;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public static class JsResourceHelper
    {
        public static string BuildResourceObject()
        {
            var x = typeof(Js.Resources.JsResources);
            var sb = new StringBuilder();

            sb.Append("window.cdnPath='" + ConfigFetcher.Fetch("CdnEndpoint") + "';");

            sb.Append("window.JsResources={");
            foreach (var p in x.GetProperties())
            {
                var s = p.GetValue(null, null);
                if (s is string)
                {
                    sb.Append("\"" + p.Name + "\":\"" +
                              s.ToString().Replace("\r\n", @"\n").Replace("\n", @"\n").Replace("\"", @"\""") +
                              "\",");
                    sb.AppendLine();
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            return sb.ToString();
        }
    }
}