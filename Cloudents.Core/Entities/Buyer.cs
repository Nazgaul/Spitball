﻿using System;
using System.Globalization;

namespace Cloudents.Core.Entities
{
    public class BuyerPayment
    {
        public BuyerPayment(string paymentKey, DateTime paymentKeyExpiration)
        {
            PaymentKey = paymentKey;
            //PaymentKeyExpiration = DateTime.ParseExact(paymentKeyExpiration, "MMyy", CultureInfo.InvariantCulture);
            PaymentKeyExpiration = paymentKeyExpiration;
        }

        protected BuyerPayment()
        {
        }

        public  string PaymentKey { get; set; }
        public  DateTime PaymentKeyExpiration { get; set; }

        public bool IsValid()
        {
            return this.PaymentKey != null && this.PaymentKeyExpiration > DateTime.UtcNow;
        }
    }
}