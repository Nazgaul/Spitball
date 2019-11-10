using Cloudents.Core.Entities;
using System;

namespace Cloudents.Command.Command.Admin
{
    public class CreateCouponCommand : ICommand
    {
        public CreateCouponCommand(string code, CouponType couponType, long? tutorId, decimal value,
             DateTime? expiration, string description, string owner, int? amountOfUsers, int amountOfUsePerUser)
        {
            Code = code;
            CouponType = couponType;
            TutorId = tutorId;
            Value = value;
            Expiration = expiration;
            Description = description;
            Owner = owner;
            AmountOfUsers = amountOfUsers;
            AmountOfUsePerUser = amountOfUsePerUser;
        }

        public string Owner { get; }

        public string Description { get; }

        public DateTime? Expiration { get; }

        public int? AmountOfUsers { get; }
        public int AmountOfUsePerUser { get; }

        public decimal Value { get; }

        public long? TutorId { get; }

        public string Code { get; }
        public CouponType CouponType { get; }
    }
}