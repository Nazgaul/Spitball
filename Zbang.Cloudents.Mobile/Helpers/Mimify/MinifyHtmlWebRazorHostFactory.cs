using System.Web.Mvc.Razor;
using System.Web.Razor.Generator;
using System.Web.WebPages.Razor;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers.Mimify
{
// ReSharper disable once UnusedMember.Global
    public sealed class MinifyHtmlWebRazorHostFactory : WebRazorHostFactory
    {
        public override WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            WebPageRazorHost host = base.CreateHost(virtualPath, physicalPath);
            if (host.IsSpecialPage)
            {
                return host;
            }
            return new MinifyHtmlMvcWebPageRazorHost(virtualPath, physicalPath);
        }

        public sealed class MinifyHtmlMvcWebPageRazorHost : MvcWebPageRazorHost
        {
            public MinifyHtmlMvcWebPageRazorHost(string virtualPath, string physicalPath)
                : base(virtualPath, physicalPath)
            {
            }
            public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
            {
                if (incomingCodeGenerator is CSharpRazorCodeGenerator)
                {
                    return new MinifyHtmlCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);
                }
                return base.DecorateCodeGenerator(incomingCodeGenerator);
            }
        }
    }
}