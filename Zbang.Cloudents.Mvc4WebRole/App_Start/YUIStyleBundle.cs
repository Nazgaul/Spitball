using System.Collections.Generic;
using System.Web.Optimization;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class YUIStyleBundle : StyleBundle
    {
        public YUIStyleBundle(string virtualPath) : base(virtualPath)
        {
            this.Transforms.Clear();
            
            this.Transforms.Add(new YUITransform());
        }

        public YUIStyleBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath)
        {
            this.Transforms.Clear();
            this.Transforms.Add(new YUITransform());
        }
        public override IEnumerable<BundleFile> EnumerateFiles(BundleContext context)
        {
            return base.EnumerateFiles(context);
        }
    }
}