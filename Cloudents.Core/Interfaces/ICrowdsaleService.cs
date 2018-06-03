using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ICrowdSaleService
    {
        Task<string> BuyTokensAsync(string senderPK, int amount, CancellationToken token);
        //(string privateKey, string publicAddress) CreateAccount();
    }
}
