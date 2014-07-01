using System.IO;
using System.Linq;
using SquishIt.Framework.JavaScript;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class JavaScriptBundleImp : JavaScriptBundle
    {
        protected override string Template
        {
            get { return "{0} {1},"; }
        }

        protected override string AppendFileClosure(string content)
        {
            return string.Empty;
            return base.AppendFileClosure(content);
        }
    }
}