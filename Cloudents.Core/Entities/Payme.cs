using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Entities
{
    public class Payme : Entity<Guid>, IPaymentProvider
    {
        public Payme(long adminDuration, decimal studentPay, decimal spitballPay)
        {
            AdminDuration = adminDuration;
            StudentPay = studentPay;
            SpitballPay = spitballPay;
        }
        protected Payme()
        {

        }
        public virtual long AdminDuration { get; protected set; }
        public virtual decimal StudentPay { get; protected set; }
        public virtual decimal SpitballPay { get; protected set; }
    }
}
