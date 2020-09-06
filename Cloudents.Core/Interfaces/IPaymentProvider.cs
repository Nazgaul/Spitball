using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IPaymentProvider
    {
        Task<string> ChargeSessionAsync(StudyRoomPayment sessionPayment,double price, CancellationToken token);
        Task<string> ChargeSessionAsync(Tutor tutor, User user,Guid id, double price, CancellationToken token);

        Task<string> ChargeSessionBySpitballAsync(Tutor tutor, double price, CancellationToken token);

    }
}