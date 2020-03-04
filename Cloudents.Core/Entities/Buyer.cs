using System;

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

        protected BuyerPayment()
        {
        }

        public string? PaymentKey { get; set; }
        public DateTime PaymentKeyExpiration { get; set; }

        public string CreditCardMask { get; set; }

        public bool IsValid()
        {
            return PaymentKey != null && PaymentKeyExpiration > DateTime.UtcNow;
        }
    }
}