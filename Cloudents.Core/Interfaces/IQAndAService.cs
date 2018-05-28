using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IQAndAService
    {
        Task<string> SubmitQuestion(string question, int price, string senderAddress, CancellationToken token);
       
    }
}
