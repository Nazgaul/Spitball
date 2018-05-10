using System.Numerics;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockchainProvider
    {

        Task<BigInteger> GetBalanceAsync(string accountAddress);
        string CreateAccount(); //return privete to user
        Task<int> SendTxAsync(string senderAddress, string senderPK, string recipientAddress, string azureUrl);
        Task<T> MessageContractAsync<T>(string contractHash, string azureUrl, string abi, string functionName);
        Task<string> TxContractAsync(string operation, string senderAddress, string senderPK, string contractHash, string abi, string azureUrl, string[] parameters);
        string GetPublicAddress(string privateKey); //Get Public from from privete

        
    }
}
