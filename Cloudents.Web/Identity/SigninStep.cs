using System;

namespace Cloudents.Web.Identity
{
    [Flags]
    public enum SigninStep
    {
        None = 0,
        Email = 1,
        Sms = 2,
        All = Email | Sms
    }

    public static class SignInStep
    {
        public const string Claim = "signInStep";
        public const string PolicyEmail = "policyEmail";
        //public const string PolicySms = "policySms";
        public const string PolicyAll = "policyAll";
    }
}