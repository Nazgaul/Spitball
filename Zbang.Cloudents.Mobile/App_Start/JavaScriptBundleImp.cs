
using SquishIt.Framework.JavaScript;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class JavaScriptBundleImp : JavaScriptBundle
    {
        protected override string Template
        {
            get { return "{0} {1},"; }
        }
    }
}