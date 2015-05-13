using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Extensions
{

    public static class ConfigFetcher
    {
        private static readonly bool IsRunningOnCloud;
        private static readonly Dictionary<string, string> ConfigurationValues = new Dictionary<string, string>();

        static ConfigFetcher()
        {
            try
            {
                IsRunningOnCloud = RoleEnvironment.IsAvailable;
            }
            catch (TypeInitializationException)
            {
                IsRunningOnCloud = false;
            }
            catch(Exception ex)
            {
                TraceLog.WriteError(ex);
                IsRunningOnCloud = false;
            }
        }

        public static string Fetch(string name)
        {
            string retVal;
            if (ConfigurationValues.TryGetValue(name, out retVal))
            {
                return retVal;
            }
            retVal = IsRunningOnCloud ? FromAzure(name) : FromConfig(name);
            ConfigurationValues.Add(name, retVal);
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
