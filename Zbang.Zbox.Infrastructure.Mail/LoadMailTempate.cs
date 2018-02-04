using System.Globalization;
using System.Reflection;
using System.Text;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public static class LoadMailTempate
    {
        //private static readonly Dictionary<string, string> Resources = new Dictionary<string, string>();
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
            //if (Resources.TryGetValue(resourceName, out content))
            //{
            //    return content;
            //}
            return LoadResource(resourceName + ".html");
            //Resources.Add(resourceName, content);
            //return content;

        }

        public static StringBuilder LoadMailFromContentWithDot(CultureInfo culture, string resourceName)
        {
            while (!culture.Equals(CultureInfo.InvariantCulture))
            {
                var assemblyResourceName = $"{resourceName}_{culture.Name.ToLower()}.html";
                var resource = LoadResource(assemblyResourceName);
                if (!string.IsNullOrEmpty(resource))
                {
                    return new StringBuilder(resource);
                }
                culture = culture.Parent;
            }
            //if (Resources.TryGetValue(resourceName, out content))
            //{
            //    return content;
            //}
            return new StringBuilder(LoadResource(resourceName + ".html"));
            //Resources.Add(resourceName, content);
            //return content;

        }

        private static string LoadResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;
                var content = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(content, 0, (int)stream.Length);
                return Encoding.UTF8.GetString(content);
            }
        }
    }
}
