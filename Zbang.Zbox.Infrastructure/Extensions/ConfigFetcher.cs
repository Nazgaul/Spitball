using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace Zbang.Zbox.Infrastructure.Extensions
{

    public static class ConfigFetcher
    {
        private static readonly bool IsRunningOnCloud;

        static ConfigFetcher()
        {
            try
            {
                IsRunningOnCloud = RoleEnvironment.IsAvailable;
            }
            catch (System.TypeInitializationException)
            {
                IsRunningOnCloud = false;
            }
        }

        public static string Fetch(string name)
        {
            string val = IsRunningOnCloud ? FromAzure(name) : FromConfig(name);
            return val;
        }

        private static string FromAzure(string name)
        {
            try
            {
                var connStr = RoleEnvironment.GetConfigurationSettingValue(name);
                return connStr;
            }
            catch
            {
                return FromConfig(name);
            }

        }

        private static string FromConfig(string name)
        {


            var setting = ConfigurationManager.AppSettings[name];

            if (setting != null)
            {
                return setting;
            }

            var connStr = ConfigurationManager.ConnectionStrings[name];

            if (connStr != null)
            {
                return connStr.ConnectionString;
            }

            return null;
            //throw new ConfigurationErrorsException(name);
        }

    }
}
