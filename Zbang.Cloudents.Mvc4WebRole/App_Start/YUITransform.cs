using System.Web.Optimization;
using Yahoo.Yui.Compressor;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class YUITransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var compressor = new CssCompressor();
            response.Content = compressor.Compress(response.Content);
            //response.Content = dotless.Core.Less.Parse(response.Content);
            response.ContentType = "text/css";
        }
    }
}