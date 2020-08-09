//using Cloudents.Core.Entities;
//using System;

//namespace Cloudents.Command.Command.Admin
//{
//    public class CreateCouponCommand : ICommand
//    {
//        public CreateCouponCommand(string code, CouponType couponType, long? tutorId, decimal value,
//             DateTime? expiration, string? description)
//        {
//            Code = code;
//            CouponType = couponType;
//            TutorId = tutorId;
//            Value = value;
//            Expiration = expiration;
//            Description = description;
//        }


//        public string? Description { get; }

//        public DateTime? Expiration { get; }


//        public decimal Value { get; }

//        public long? TutorId { get; }

//        public string Code { get; }
//        public CouponType CouponType { get; }
//    }
//}