using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    //public enum RegisterLogInRoutes
    //{
    //    [Parse("setPassword")]
    //    LoginSetPassword,
    //    [Parse("setEmailPassword")]
    //    RegisterSetEmailPassword,
    //    [Parse("setPassword")]
    //    RegisterEmailConfirmed,
    //    [Parse("setPassword")]
    //    RegisterVerifyPhone,
    //    [Parse("setPassword")]
    //    RegisterSetPhone
    //}

    public sealed class RegistrationStep //: Enumeration
    {
        [JsonProperty("name")]
        public string RouteName { get; }

        [JsonIgnore]
        public string RoutePath { get; }


        private RegistrationStep(string routeName, string routePath)
        {
            RouteName = routeName;
            RoutePath = routePath;
        }

        public static readonly RegistrationStep LoginSetPassword = new RegistrationStep("setPassword", "set-password");
        public static readonly RegistrationStep RegisterSetEmailPassword = new RegistrationStep("setEmailPassword", "personal-details");
        public static readonly RegistrationStep RegisterEmailConfirmed = new RegistrationStep("registerEmailConfirmed", "email-confirmed");
        public static readonly RegistrationStep RegisterVerifyPhone = new RegistrationStep("verifyPhone", "verify-phone");
        public static readonly RegistrationStep RegisterSetPhone = new RegistrationStep("setPhone", "set-phone");




        //public override string ToString()
        //{
        //    return RouteName;
        //}


        public static RegistrationStep GetStepByUrl(string step)
        {
            if (string.IsNullOrEmpty(step))
            {
                return null;
            }

            return typeof(RegistrationStep).GetFields(BindingFlags.Public | BindingFlags.Static)
                            .Where(w => !w.IsLiteral)
                            .Select(s => (RegistrationStep)s.GetValue(null))
                            .FirstOrDefault(f => f.RoutePath.Equals(step, StringComparison.OrdinalIgnoreCase));


        }

        private bool Equals(RegistrationStep other)
        {
            return RouteName == other.RouteName;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RegistrationStep other && Equals(other);
        }

        public override int GetHashCode()
        {
            return RouteName.GetHashCode();
        }
    }
}