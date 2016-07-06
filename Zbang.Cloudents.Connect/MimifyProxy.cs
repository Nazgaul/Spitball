using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR.Hubs;

namespace Zbang.Cloudents.Connect
{
    public class MimifyProxy : IJavaScriptMinifier
    {
        public string Minify(string source)
        {
            var minifier = new Minifier();
            return minifier.MinifyJavaScript(source);
        }
    }
}