using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class EmailCouponEventHandler : IEventHandler<UseCouponEvent>, IEventHandler<ApplyCouponEvent>,
        IDisposable
    {
        private readonly IQueueProvider _queueProvider;

        public EmailCouponEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public async Task HandleAsync(UseCouponEvent eventMessage, CancellationToken token)
        {
            var message = new CouponActionEmail(eventMessage.UserCoupon.User.Id, eventMessage.UserCoupon.User.PhoneNumber, eventMessage.UserCoupon.User.Email,
                    eventMessage.UserCoupon.Coupon.Code, eventMessage.UserCoupon.Tutor?.User.Name, CouponAction.Use.ToString());
            await _queueProvider.InsertMessageAsync(message, token);
        }

        public async Task HandleAsync(ApplyCouponEvent eventMessage, CancellationToken token)
        {
            var message = new CouponActionEmail(eventMessage.UserCoupon.User.Id, eventMessage.UserCoupon.User.PhoneNumber, eventMessage.UserCoupon.User.Email,
                    eventMessage.UserCoupon.Coupon.Code, eventMessage.UserCoupon.Tutor?.User.Name, CouponAction.Apply.ToString());
            await _queueProvider.InsertMessageAsync(message, token);
           
        }

        public void Dispose()
        {
        }

        private enum CouponAction
        {
            Use,
            Apply
        }
    }
}
