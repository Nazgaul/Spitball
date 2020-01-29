using System;
using System.Linq;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Core.Attributes;
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


        private RegistrationStep(string routeName)
        {
            RouteName = routeName;
        }

        public static readonly RegistrationStep LoginSetPassword = new RegistrationStep("setPassword");
        public static readonly RegistrationStep RegisterSetEmailPassword = new RegistrationStep("setEmailPassword");
        public static readonly RegistrationStep RegisterEmailConfirmed = new RegistrationStep("emailConfirmed");
        public static readonly RegistrationStep RegisterVerifyPhone = new RegistrationStep("verifyPhone");
        public static readonly RegistrationStep RegisterSetPhone = new RegistrationStep("setPhone");



        public override string ToString()
        {
            return RouteName;
        }


        public static RegistrationStep GetByStep(string step)
        {
            if (string.IsNullOrEmpty(step))
            {
                return null;
            }

            return typeof(RegistrationStep).GetFields(BindingFlags.Public | BindingFlags.Static)
                            .Where(w => !w.IsLiteral)
                            .Select(s => (RegistrationStep)s.GetValue(null))
                            .FirstOrDefault(f => f.ToString().Equals(step, StringComparison.OrdinalIgnoreCase));


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