using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class ResourceFileResolver
    {
        public static string UICultureContent(this UrlHelper urlHelper, string contentPath)
        {
            string neutralResourcesLanguage = GetNeutralResourcesLanguage();
            if (String.Compare(Thread.CurrentThread.CurrentUICulture.Name, neutralResourcesLanguage, true) == 0)
            {
                // the current thread's culture matches the neutral resource language culture
                // so get the fallback resources
                return urlHelper.Content(contentPath);
            }

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;
            while (!cultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                string cultureFilename = Path.GetFileNameWithoutExtension(VirtualPathUtility.GetFileName(contentPath)) + "." + cultureInfo.Name + VirtualPathUtility.GetExtension(contentPath);
                string cultureContentPath = Path.Combine(VirtualPathUtility.GetDirectory(contentPath), cultureFilename);
                string absoluteCultureContentPath = HttpContext.Current.Server.MapPath(cultureContentPath);
                if (File.Exists(absoluteCultureContentPath))
                {
                    // there is a file for this culture
                    return urlHelper.Content(cultureContentPath);
                }

                // there is no file specifically for this culture so try the culture's parent
                cultureInfo = cultureInfo.Parent;
            }
            // there is no file for the culture or any of its parents so use the fallback resources
            return urlHelper.Content(contentPath);
        }

        private static string GetNeutralResourcesLanguage()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(NeutralResourcesLanguageAttribute), false);
            if (attributes.GetLength(0) > 0)
            {
                NeutralResourcesLanguageAttribute neutralResourcesLanguageAttribute = (NeutralResourcesLanguageAttribute)attributes[0];
                return neutralResourcesLanguageAttribute.CultureName;
            }

            return null;
        }
    }
}