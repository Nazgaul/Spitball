using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class BuyerPayment
    {
        public BuyerPayment(string paymentKey, DateTime paymentKeyExpiration, string creditCardMask)
        {
            PaymentKey = paymentKey;
            PaymentKeyExpiration = paymentKeyExpiration;
            CreditCardMask = creditCardMask;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected BuyerPayment()
        {
        }

        public string PaymentKey { get; set; }
        public DateTime PaymentKeyExpiration { get; set; }

        public string CreditCardMask { get; set; }

        public bool IsValid()
        {
            return PaymentKey != null && PaymentKeyExpiration > DateTime.UtcNow;
        }
    }
}