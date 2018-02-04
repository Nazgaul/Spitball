using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class ConfigFetcher
    {
        public static readonly bool IsRunningOnCloud;
        public static readonly bool IsEmulated;
        private static readonly Dictionary<string, string> ConfigurationValues;
        private static readonly object LockObj = new object();

        static ConfigFetcher()
        {
            ConfigurationValues = new Dictionary<string, string>();
            try
            {
                IsRunningOnCloud = RoleEnvironment.IsAvailable;
                if (IsRunningOnCloud)
                {
                    IsEmulated = RoleEnvironment.IsEmulated;
                }
            }
            catch (TypeInitializationException)
            {
                IsRunningOnCloud = false;
            }
            //catch(Exception ex)
            //{
            //    //TraceLog.WriteError(ex);
            //    IsRunningOnCloud = false;
            //}
        }

        public static string Fetch(string name)
        {
            lock (LockObj)
            {
                if (ConfigurationValues.TryGetValue(name, out string retVal))
                {
                    return retVal;
                }
                retVal = IsRunningOnCloud ? FromAzure(name) : FromConfig(name);

                ConfigurationValues.Add(name, retVal);
                return retVal;
            }
        }

        private static string FromAzure(string name)
        {
            try
            {
                var connStr = RoleEnvironment.GetConfigurationSettingValue(name);

                return connStr;
            }
            catch(RoleEnvironmentException)
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

            return connStr?.ConnectionString;

            //throw new ConfigurationErrorsException(name);
        }
    }
}
