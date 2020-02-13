using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class CreateCouponCommand : ICommand
    {
        public CreateCouponCommand(string code, CouponType couponType, long tutorId, decimal value)
        {
            Code = code;
            CouponType = couponType;
            TutorId = tutorId;
            Value = value;
            
        }

 
        public decimal Value { get; }

        public long TutorId { get; }

        public string Code { get; }
        public CouponType CouponType { get; }
    }
}
