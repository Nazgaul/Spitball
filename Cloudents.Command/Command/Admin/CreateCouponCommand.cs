using Cloudents.Core.Entities;
using System;
using Cloudents.Core.Enum;

namespace Cloudents.Command.Command.Admin
{
    public class CreateCouponCommand : ICommand
    {
        public CreateCouponCommand(string code, CouponType couponType, decimal value,
             DateTime? expiration)
        {
            Code = code;
            CouponType = couponType;
            Value = value;
            Expiration = expiration;
        }



        public DateTime? Expiration { get; }


        public decimal Value { get; }


        public string Code { get; }
        public CouponType CouponType { get; }
    }
}