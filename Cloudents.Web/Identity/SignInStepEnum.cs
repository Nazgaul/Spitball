using System;

namespace Cloudents.Web.Identity
{
    [Flags]
    public enum SignInStepEnum
    {
        None = 0,
        Email = 1,
        Sms = 2,
        UntilPassword = Email | Sms,
        Password = 4,
        All = Email | Sms | Password
    }

    public static class SignInStep
    {
        public const string Claim = "signInStep";
        public const string PolicyEmail = "policyEmail";
        public const string PolicySms = "PolicySms";
        public const string PolicyPassword = "PolicyPassword";
        public const string PolicyAll = "policyAll";
    }
}