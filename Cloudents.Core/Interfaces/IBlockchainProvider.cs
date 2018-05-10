using System.Numerics;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockchainProvider
    {

        Task<BigInteger> GetBalanceAsync(string accountAddress);
        string CreateAccount();
        Task<int> SendTxAsync(string senderAddress, string senderPK, string recipientAddress, string azureUrl);
        Task<T> MessageContractAsync<T>(string contractHash, string azureUrl, string abi, string functionName);
       // Task<string> TxContractTxContractAsync(string operation, string senderAddress, string senderPK, string contractHash, string abi, string azureUrl);
        string GetPublicAddress(string privateKey); //Get Public from from privete

        //return privete to user
    }
}
