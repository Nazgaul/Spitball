using System;
using System.Reflection;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class VersionHelper
    {
        public static string CurrentVersion()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var retval = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
                return retval;
            }
            catch (Exception)
            {
                return "?.?.?";
            }
        }
    }
}



