using System.Collections.Generic;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace Zbang.Zbox.Infrastructure.Extensions
{

    public static class ConfigFetcher
    {
        private static readonly bool IsRunningOnCloud;
        private static Dictionary<string, string> m_ConfigurationValues = new Dictionary<string, string>();

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
            string retVal;
            if (m_ConfigurationValues.TryGetValue(name, out retVal))
            {
                return retVal;
            }
            retVal = IsRunningOnCloud ? FromAzure(name) : FromConfig(name);
            m_ConfigurationValues.Add(name, retVal);
            return retVal;
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
