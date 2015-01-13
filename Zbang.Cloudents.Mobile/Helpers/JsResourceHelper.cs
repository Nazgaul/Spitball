using System.Globalization;
using System.Text;
using System.Threading;

namespace Zbang.Cloudents.Mobile.Helpers
{
    public static class JsResourceHelper
    {
        public static string BuildResourceObject()
        {
            var x = typeof(Js.Resources.JsResources);
            var sb = new StringBuilder();
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