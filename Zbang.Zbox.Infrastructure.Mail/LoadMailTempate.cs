using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public static class LoadMailTempate
    {
        public static string LoadMailFromContent(CultureInfo culture, string resourceName)
        {
           

            while (!culture.Equals(CultureInfo.InvariantCulture))
            {
                var assemblyResourceName = resourceName + culture.Name.UppercaseFirst() + ".html";
                var resource = loadResource(assemblyResourceName);
                if (!string.IsNullOrEmpty(resource))
                {
                    return resource;
                }
                culture = culture.Parent;

            }
            return loadResource(resourceName + ".html");

        }

        private static string loadResource(string resourceName)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                var content = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(content, 0, (int)stream.Length);
                return Encoding.UTF8.GetString(content);
            }
            return string.Empty;
        }
    }
}
