using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class EmailCouponEventHandler : IEventHandler<UseCouponEvent>, IEventHandler<ApplyCouponEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public EmailCouponEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(UseCouponEvent eventMessage, CancellationToken token)
        {
            var message = new CouponActionEmail(eventMessage.UserCoupon.User.Id, eventMessage.UserCoupon.User.PhoneNumber, eventMessage.UserCoupon.User.Email,
                    eventMessage.UserCoupon.Coupon.Code, eventMessage.UserCoupon.Coupon.Course.Tutor.User.Name, CouponAction.Use.ToString());
            return _queueProvider.InsertMessageAsync(message, token);
        }

        public Task HandleAsync(ApplyCouponEvent eventMessage, CancellationToken token)
        {
            var message = new CouponActionEmail(eventMessage.UserCoupon.User.Id, eventMessage.UserCoupon.User.PhoneNumber, eventMessage.UserCoupon.User.Email,
                    eventMessage.UserCoupon.Coupon.Code, eventMessage.UserCoupon.Coupon.Course.Tutor.User.Name, CouponAction.Apply.ToString());
            return _queueProvider.InsertMessageAsync(message, token);
        }

       

        private enum CouponAction
        {
            Use,
            Apply
        }
    }
}
