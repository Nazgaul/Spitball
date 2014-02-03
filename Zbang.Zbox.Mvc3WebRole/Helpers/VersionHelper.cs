using System;
using System.Reflection;

public static class VersionHelper
{
    public static string CurrentVersion()
    {
        try
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            var retval = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            return retval;
        }
        catch (Exception)
        {
            return "?.?.?";
        }
    }
}



