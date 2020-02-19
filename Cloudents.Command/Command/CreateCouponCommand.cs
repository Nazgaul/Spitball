using Cloudents.Core.Entities;
using System;

namespace Cloudents.Command.Command
{
    public class CreateCouponCommand : ICommand
    {
        public CreateCouponCommand(string code, CouponType couponType, long tutorId, decimal value, DateTime expiration)
        {
            Code = code;
            CouponType = couponType;
            TutorId = tutorId;
            Value = value;
            Expiration = expiration;
        }

 
        public decimal Value { get; }

        public long TutorId { get; }

        public string Code { get; }
        public CouponType CouponType { get; }
        public DateTime Expiration { get; }
    }
}
