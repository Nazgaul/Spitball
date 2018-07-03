using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link) : base(to, "register","Welcome to Spitball")
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }
    }

    [Serializable]
    public class SupportRedeemEmail: BaseEmail
    {
        public SupportRedeemEmail(decimal amount, long userId) : base("support@spitball.co", null, "Redeem Email")
        {
            Amount = amount;
            UserId = userId;
        }

        public decimal Amount { get; set; }
        public long UserId { get; set; }
       

        public override string ToString()
        {
            return $"User id: {UserId} want to redeem {Amount}";
        }
    }
}