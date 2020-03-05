using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Entities
{
    public class Payme : Entity<Guid>, IPaymentProvider
    {
        public Payme(decimal studentPay, decimal spitballPay)
        {
            StudentPay = studentPay;
            SpitballPay = spitballPay;
        }
        protected Payme()
        {

        }
        public virtual decimal StudentPay { get; protected set; }
        public virtual decimal SpitballPay { get; protected set; }
    }
}
