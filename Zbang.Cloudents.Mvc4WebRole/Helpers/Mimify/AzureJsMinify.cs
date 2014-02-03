using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers.Mimify
{
    public class AzureJsMinify : JsMinify
    {
        public override void Process(BundleContext context, BundleResponse response)
        {
            base.Process(context, response);

        }
    }

    public class AzureJsBundle : Bundle
    {
        public AzureJsBundle(string virtualPath)
            : this(virtualPath, null)
        {
        }
        public AzureJsBundle(string virtualPath, string cdnPath)
            : base(virtualPath, cdnPath, new IBundleTransform[]
		{
			new AzureJsMinify()
		})
        {
            base.ConcatenationToken = ";";
        }
    }
}