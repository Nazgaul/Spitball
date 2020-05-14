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

    public interface IPayment2
    {
        bool IsValid();
        string PaymentKey { get; }

        IPayment2 AvoidProxy { get; }
    }

    public class PaymePayment : Entity<Guid>, IPayment2
    {
        public PaymePayment(string paymentKey, DateTime paymentKeyExpiration, string creditCardMask)
        {
            PaymentKey = paymentKey;
            PaymentKeyExpiration = paymentKeyExpiration;
            CreditCardMask = creditCardMask;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected PaymePayment()
        {
        }

        public virtual string PaymentKey { get; set; }
        public virtual IPayment2 AvoidProxy => (IPayment2)Actual;
        public virtual DateTime PaymentKeyExpiration { get; set; }

        public virtual string CreditCardMask { get; set; }

        public virtual bool IsValid()
        {
            return PaymentKey != null && PaymentKeyExpiration > DateTime.UtcNow;
        }
    }

    public class StripePayment :Entity<Guid>, IPayment2
    {
        public StripePayment(string paymentKey)
        {
            PaymentKey = paymentKey;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected StripePayment()
        {
        }

        public virtual string PaymentKey { get; set; }
        //public virtual DateTime PaymentKeyExpiration { get; set; }

        //public virtual string CreditCardMask { get; set; }
        public virtual IPayment2 AvoidProxy => (IPayment2)Actual;
        public virtual bool IsValid()
        {
            return PaymentKey != null;// && PaymentKeyExpiration > DateTime.UtcNow;
        }
    }
}