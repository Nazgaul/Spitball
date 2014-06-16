using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public static class LoadMailTempate
    {
        private static readonly Dictionary<string, string> Resources = new Dictionary<string, string>();
        public static string LoadMailFromContent(CultureInfo culture, string resourceName)
        {
            while (!culture.Equals(CultureInfo.InvariantCulture))
            {
                var assemblyResourceName = resourceName + culture.Name.UppercaseFirst() + ".html";
                var resource = LoadResource(assemblyResourceName);
                if (!string.IsNullOrEmpty(resource))
                {
                    return resource;
                }
                culture = culture.Parent;

            }
            string content;
            if (Resources.TryGetValue(resourceName, out content))
            {
                return content;
            }
            content =  LoadResource(resourceName + ".html");
            Resources.Add(resourceName, content);
            return content;

        }

        private static string LoadResource(string resourceName)
        {

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
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
}
