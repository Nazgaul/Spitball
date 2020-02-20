using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;

namespace Cloudents.FunctionsV2.Operations
{
    public class CouponAdminEmailOperation : ISystemOperation<CouponActionEmail>
    {
        private readonly IConfigurationKeys _configuration;
        private string _email = "yaniv@spitball.co";

        public CouponAdminEmailOperation(
            IConfigurationKeys configuration)
        {
            _configuration = configuration;
        }

        public async Task DoOperationAsync(CouponActionEmail message2, IBinder binder, CancellationToken token)
        {
            if (_configuration.Search.IsDevelop)
            {
                _email = "elad@cloudents.com";
            }

            var message = new SendGridMessage()
            {
                Subject = $"New coupon action - {message2.CouponAction}",
                HtmlContent = $@"<html><body><table border=""1"">
                                            <tr>
                                                <th>User Id</th> 
                                                <th>Phone Number</th>
                                                <th>Email</th>
                                                <th>Coupon Code</th>
                                                <th>Tutor Name</th>
                                                <th>Coupon Action</th>
                                            </tr>
                                            <tr>
                                                <td>{message2.Id}</td> 
                                                <td>{message2.PhoneNumber}</td>
                                                <td>{message2.Email}</td>
                                                <td>{message2.CouponCode}</td>
                                                <td>{message2.TutorName}</td>
                                                <td>{message2.CouponAction}</td>
                                            </tr>
                                        </table></body></html>"
            };

            message.AddTo(_email);

            var emailProvider =
                await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
                { ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>" }, token);

            await emailProvider.AddAsync(message, token);
            await emailProvider.FlushAsync(token);
        }
    }
}
