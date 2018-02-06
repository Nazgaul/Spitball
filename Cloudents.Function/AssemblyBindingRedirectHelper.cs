using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Function
{
    class AssemblyBindingRedirectHelper
    {
        public static void ConfigureBindingRedirects()
        {
            var redirects = GetBindingRedirects();
            redirects.ForEach(RedirectAssembly);
        }

        private static List<BindingRedirect> GetBindingRedirects()
        {
            var assemblies =
                AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assem in assemblies.OrderBy(o => o.FullName))
            {
                Debug.WriteLine(assem.FullName);
            }
            return new List<BindingRedirect>
            {
                new BindingRedirect("Autofac", "17863AF14B0044DA", "4.6.2.0"),
                new BindingRedirect("Autofac.Extras.CommonServiceLocator","","5.0.0.0")
            };
        }

        private class BindingRedirect
        {
            public BindingRedirect(string shortName, string publicKeyToken, string redirectToVersion)
            {
                ShortName = shortName;
                PublicKeyToken = publicKeyToken;
                RedirectToVersion = redirectToVersion;
            }

            public string ShortName { get; set; }
            public string PublicKeyToken { get; set; }
            public string RedirectToVersion { get; set; }
        }

        private static void RedirectAssembly(BindingRedirect bindingRedirect)
        {
            ResolveEventHandler handler = null;
            handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != bindingRedirect.ShortName)
                {
                    return null;
                }
                var targetPublicKeyToken = new AssemblyName("x, PublicKeyToken=" + bindingRedirect.PublicKeyToken).GetPublicKeyToken();
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);
                requestedAssembly.Version = new Version(bindingRedirect.RedirectToVersion);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;
                AppDomain.CurrentDomain.AssemblyResolve -= handler;
                return Assembly.Load(requestedAssembly);
            };
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }
    }
}
