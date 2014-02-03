using System;
using System.Reflection;
using System.Web.Mvc;


public static class VersionHelper
{

    /// <summary>
    /// Return the Current Version from the AssemblyInfo.cs file.
    /// </summary>
    public static string CurrentVersion(this HtmlHelper helper)
    {
        try
        {
            System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString();
        }
        catch (Exception)
        {
            return "?.?.?";
        }
    }

   

}



