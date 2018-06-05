using System;

namespace Cloudents.Web.Identity
{
    [Flags]
    public enum SignInStepEnum
    {
        None = 0,
        //Email = 1,
        //Sms = 1,
        //Password = 2,
        //UntilPassword = Email | Sms,
        //Password = 4,
        All = 1 // = Sms | Password
    }

    public static class SignInStep
    {
        public const string Claim = "signInStep";
        //public const string PolicyEmail = "policyEmail";
        //public const string Sms = "PolicySms";
        //public const string Password = "PolicyPassword";
        public const string Finish = "policyAll";
    }
}