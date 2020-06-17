using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class StripePayment :Entity<Guid>, IPayment2
    {
        public StripePayment(string paymentKey)
        {
            PaymentKey = paymentKey;
            Created = DateTime.UtcNow;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected StripePayment()
        {
        }

        public virtual string PaymentKey { get; set; }

        public virtual DateTime? Created { get; set; }
        //public virtual DateTime PaymentKeyExpiration { get; set; }

        //public virtual string CreditCardMask { get; set; }
        public virtual IPayment2 AvoidProxy => (IPayment2)Actual;
        public virtual bool IsValid()
        {
            return PaymentKey != null;
        }
    }
}