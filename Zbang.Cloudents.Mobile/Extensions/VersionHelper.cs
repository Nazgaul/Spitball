using System;
using System.Reflection;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class VersionHelper
    {
        public static string CurrentVersion(bool withRevision)
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version.ToString(withRevision ? 4 : 3);
            }
            catch (Exception)
            {
                return "?.?.?";
            }
        }
    }
}



